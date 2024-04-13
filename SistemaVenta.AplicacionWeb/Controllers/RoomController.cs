using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRoomService _roomService;
        private readonly ILevelService _levelService;
        private readonly IImageService _imageService;

        public RoomController(IMapper mapper, IRoomService roomService, ILevelService levelService, IImageService imageService)
        {
            _mapper = mapper;
            _roomService = roomService;
            _levelService = levelService;
            _imageService = imageService;   
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();

            List<RoomDTO> roomDTOLista = _mapper.Map<List<RoomDTO>>(await _roomService.Listar(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = roomDTOLista });
        }

        [HttpGet]
        public async Task<IActionResult> ListarRoomStatus()
        {
            List<RoomStatus> roomDTOLista = await _roomService.ListarRoomStatus();
            return StatusCode(StatusCodes.Status200OK, new { data = roomDTOLista });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllLevels()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();

            var levels = await _levelService.Lista(idEstablishment);
            return Json(new { niveles = levels });
        }
        
        [HttpGet]
        public async Task<IActionResult> ListImagesById(string roomId)
        {
            int isRoom = int.Parse(roomId);

            List<ImagesRoomDTO> imagesRoomDTOLista = _mapper.Map<List<ImagesRoomDTO>>(await _imageService.ListImagesByRoom(isRoom));
            return StatusCode(StatusCodes.Status200OK, new { data = imagesRoomDTOLista });

        }
        [HttpGet]
        public async Task<IActionResult> ListByLevel(string levelNum)
        {
            int levelNumber = int.Parse(levelNum);
            int idEstablishment = GetEstablishmentIdFromClaims();

            List<RoomDTO> roomDTOLista = _mapper.Map<List<RoomDTO>>(await _roomService.ListByLevel(idEstablishment,levelNumber));
            return StatusCode(StatusCodes.Status200OK, new { data = roomDTOLista });

        }

        [HttpGet]
        public async Task<IActionResult> ListByIdEstablishment()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();

            List<RoomDTO> roomDTOLista = _mapper.Map<List<RoomDTO>>(await _roomService.GetByIdEstablishment(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = roomDTOLista });
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] List<IFormFile> newImagenes,
            [FromForm] string modelo)
        {
            GenericResponse<RoomDTO> response = new GenericResponse<RoomDTO>();
            int idEstablishment = GetEstablishmentIdFromClaims();

            try
            {
                RoomDTO roomAndImagesDto = JsonConvert.DeserializeObject<RoomDTO>(modelo);

                roomAndImagesDto.IdEstablishment = idEstablishment;
                Room room_creado = await _roomService.CreateRoom(_mapper.Map<Room>(roomAndImagesDto));

                for (int i = 0; i < newImagenes.Count; i++)
                {
                    string nombreImagen = "";
                    Stream streamImagen = null;
                    ImagesRoomDTO imagesRoomDTO = new ImagesRoomDTO();

                    if (newImagenes[i] != null)
                    {
                        imagesRoomDTO.ImageNumber = i+1;
                        imagesRoomDTO.IdRoom = room_creado.IdRoom;
                        string nombre_en_codigo = Guid.NewGuid().ToString("N");
                        string extension = Path.GetExtension(newImagenes[i].FileName);
                        imagesRoomDTO.NameImage = string.Concat(nombre_en_codigo, extension);
                        streamImagen = newImagenes[i].OpenReadStream();

                        ImagesRoom image_room_creado = await _imageService.CreateImages(_mapper.Map<ImagesRoom>(imagesRoomDTO), streamImagen);
                    }

                }

                roomAndImagesDto = _mapper.Map<RoomDTO>(room_creado);
                response.Estado = true;
                response.Objeto = roomAndImagesDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Edit(
            [FromForm] List<IFormFile> newImagenes,
            [FromForm] List<string> oldImagesUrl,
            [FromForm] List<string> imageIds,
            [FromForm] string modelo)
        {
            GenericResponse<RoomDTO> response = new GenericResponse<RoomDTO>();
            int idEstablishment = GetEstablishmentIdFromClaims();

            try
            {
                RoomDTO roomDto = JsonConvert.DeserializeObject<RoomDTO>(modelo);
                
                roomDto.IdEstablishment = idEstablishment;

                Room room_editado = await _roomService.Editar(_mapper.Map<Room>(roomDto));

                roomDto = _mapper.Map<RoomDTO>(room_editado);

                await _imageService.HandleRoomImages(newImagenes, oldImagesUrl, imageIds, room_editado);

                response.Estado = true;
                response.Objeto = roomDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int idRoom)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _roomService.Eliminar(idRoom);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        private int GetEstablishmentIdFromClaims()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            return idEstablishment;
        }
    }
}

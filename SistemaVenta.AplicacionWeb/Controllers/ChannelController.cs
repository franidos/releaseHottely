using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;
        private readonly IRoomService _roomService;

        public ChannelController(IChannelService channelService, IMapper mapper, IRoomService roomService)
        {
            _channelService = channelService;
            _mapper = mapper;
            _roomService = roomService; 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListChannels()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();
            List<RoomMapOriginDTO> channelDTOLista = _mapper.Map<List<RoomMapOriginDTO>>(await _channelService.ListRoomMaps(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = channelDTOLista });  
        }

        [HttpGet]
        public async Task<IActionResult> ListOrigins()
        {
            List<Origin> OriginsList = await _channelService.ListOrigins();
            return StatusCode(StatusCodes.Status200OK, new { data = OriginsList });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RoomMapOriginDTO modelo)
        {
            GenericResponse<RoomMapOriginDTO> response = new GenericResponse<RoomMapOriginDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                RoomMapOrigin map_created = await _channelService.Crear(_mapper.Map<RoomMapOrigin>(modelo));
                modelo = _mapper.Map<RoomMapOriginDTO>(map_created);
                Room room = await _roomService.GetRoomById(modelo.IdRoom);
                Origin origin = await _channelService.GetOriginById(modelo.IdOrigin);

                modelo.RoomName = room.Number;
                modelo.ChannelName = origin.Name;

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] RoomMapOriginDTO modelo)
        {
            GenericResponse<RoomMapOriginDTO> response = new GenericResponse<RoomMapOriginDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                RoomMapOrigin channel_editada = await _channelService.Editar(_mapper.Map<RoomMapOrigin>(modelo));
                modelo = _mapper.Map<RoomMapOriginDTO>(channel_editada);
                Room room = await _roomService.GetRoomById(modelo.IdRoom);
                Origin origin = await _channelService.GetOriginById(modelo.IdOrigin);
                
                modelo.RoomName = room.Number;
                modelo.ChannelName = origin.Name;

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idMap)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _channelService.Eliminar(idMap);
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
        private string GetUserFromClaims()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string idUsuario = claimUser.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();

            return idUsuario;
        }
    }
}

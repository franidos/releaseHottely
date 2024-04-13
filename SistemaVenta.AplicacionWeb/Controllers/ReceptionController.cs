using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;



namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReceptionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly IDetailRoomService _detailRoomService;
        private readonly IRoomService _roomService;

        public ReceptionController(IBookService bookService, IMapper mapper, IDetailRoomService detailRoomService, IRoomService roomService)
        {
            _bookService = bookService;
            _mapper = mapper;
            _detailRoomService = detailRoomService;
            _roomService = roomService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovement([FromBody] DetailRoom modelo)
        {
            GenericResponse<RoomDTO> genericResponse = new GenericResponse<RoomDTO>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                modelo.IdUsuario = int.Parse(idUsuario);

                var resDet = await _detailRoomService.Create(modelo);

                Room room_found = await _roomService.GetRoomById(resDet.IdRoom);
                room_found.IdRoomStatus = resDet.IdRoomStatus;

                RoomDTO room_editado = _mapper.Map<RoomDTO>(await _roomService.Editar(room_found));

                genericResponse.Estado = true;
                //.Mensaje = room_editado.StatusName;
                genericResponse.Objeto = room_editado;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetEvents()
        //{
        //    List<BookDTO> BookDTOLista = _mapper.Map<List<BookDTO>>(await _bookService.Listar());        
        //    return StatusCode(StatusCodes.Status200OK, new { data = BookDTOLista }); 

        //}
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                List<BookDTO> bookDTOList = _mapper.Map<List<BookDTO>>(await _bookService.Listar());
                List<object> eventList = new List<object>();

                foreach (var bookDTO in bookDTOList)
                {
                    var eventData = new
                    {
                        id = bookDTO.IdBook,
                        title = bookDTO.Reason,
                        start = bookDTO.CheckIn.ToString("yyyy-MM-dd"),
                        end = bookDTO.CheckOut.ToString("yyyy-MM-dd")
                    };

                    eventList.Add(eventData);
                }

                return new JsonResult(eventList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] BookDTO modelo)
        {
            GenericResponse<BookDTO> response = new GenericResponse<BookDTO>();
            try
            {
                Book book_creado = await _bookService.Crear(_mapper.Map<Book>(modelo));
                modelo = _mapper.Map<BookDTO>(book_creado);

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
        public async Task<IActionResult> UpdateEvent([FromBody] BookDTO modelo)
        {

            GenericResponse<BookDTO> response = new GenericResponse<BookDTO>();
            try
            {
                Book book_editado = await _bookService.Editar(_mapper.Map<Book>(modelo));
                modelo = _mapper.Map<BookDTO>(book_editado);

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
        public async Task<IActionResult> DeleteEvent(int idBook)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _bookService.Eliminar(idBook);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CheckinController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;

        public CheckinController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

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
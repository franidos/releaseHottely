using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Net;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly IChannelService _roomService;
        private readonly IBookingService _bookingService;
        private readonly IConfiguration _configuration;

        public CalendarController(IBookService bookService, IChannelService roomService, IBookingService bookingService, IMapper mapper,
            IConfiguration configuration)
        {
            _bookService = bookService;
            _roomService = roomService;
            _bookingService = bookingService;
            _mapper = mapper;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> GetEvents()
        //{
        //    List<BookDTO> BookDTOLista = _mapper.Map<List<BookDTO>>(await _bookService.Listar());        
        //    return StatusCode(StatusCodes.Status200OK, new { data = BookDTOLista }); 

        //}
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery]  int idRoom, [FromQuery] string dateCalendar)
        {
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstabl = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                // Si no hay idRoom consulta reservas del 1er Room x defecto
                if(idRoom == 0)
                {                   
                    List<Room> lista =await _bookingService.ObtainRooms(idEstabl.ToString(), DateTime.Today, DateTime.Today, false);
                    lista = lista.OrderBy(x=>x.IdRoom).ToList();
                    idRoom = lista.Any()? lista.First().IdRoom : 0;
                }               

                // Consulta si el Room tiene origenes externos mapeados para consultar reservas
                List<RoomMapOrigin> origins =  await _roomService.GetRoomMapOriginByIdRoom(idRoom);

                // 1. BOOKINGS HOTTELY
                double daysPrev =Convert.ToDouble(_configuration.GetSection("ConfigurationValues:daysPrevEventsCalendar").Value);
                List<BookDTO> bookDTOList = _mapper.Map<List<BookDTO>>(await _bookService.ListByRoom(idRoom, idEstabl, DateTime.Today.AddDays(-daysPrev)));
                List<object> eventList = new List<object>();

                foreach (var bookDTO in bookDTOList)
                {

                    var eventData = new
                    {
                        id = bookDTO.IdBook,
                        title = $"{bookDTO.IdMovimientoNavigation.NombreCliente} ({bookDTO.StatusName})",
                        desc1 = "Total Reserva $" + bookDTO.IdMovimientoNavigation?.Total?.ToString("N0"),
                        start = bookDTO.CheckIn.ToString("yyyy-MM-dd"),
                        end = bookDTO.CheckOut.ToString("yyyy-MM-dd"),
                        backgroundColor = bookDTO.statusBackground,                       
                        allow = true
                    };

                    eventList.Add(eventData);
                }

                //2. BOOKINGS EXTERNOS (Agencias)
                if(origins.Any())
                {
                    foreach (RoomMapOrigin d in origins)
                      eventList.AddRange(this.GetEventsOrigin(d.UrlCalendar,d.IdOriginNavigation?.BackgroundColor, d.IdOriginNavigation?.EventTitle, daysPrev)); 
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

        public List<object> GetEventsOrigin(string urlCalendar, string backColor, string eventTitle, double daysPrevios)
        {
            List<object> eventList = new List<object>();
            try
            {                
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(urlCalendar);
                rq.Method = "GET";
                WebResponse rs = rq.GetResponse();
                StreamReader sr = new StreamReader(rs.GetResponseStream(), System.Text.Encoding.UTF8);
                string resultEvents = sr.ReadToEnd();
                rs.Close();
                sr.Close();
                Calendar calendar = Calendar.Load(resultEvents);
                //-Recorre las fechas encontradas mayores al parametro de días requerido
                foreach (CalendarEvent eventDet in calendar.Events.Where(x=>x.Start.Date >= x.Start.Date.AddDays(-daysPrevios)))
                {
                    var eventData = new
                    {
                        id = eventDet.Uid,
                        title = eventTitle,
                        desc1 = eventDet.Description,
                        desc2 = eventDet.Summary,
                        desc3 = eventDet.Location,
                        start = eventDet.Start.Date.ToString("yyyy-MM-dd"),
                        end = eventDet.End.Date.ToString("yyyy-MM-dd"),
                        backgroundColor = "white",
                        textColor = "black",
                        borderColor = backColor,
                        totalPaid = 0,
                        status = eventDet.Status,
                        allow = false,
                    };
                    eventList.Add(eventData);
                }
                return eventList;
            }
            catch (Exception ex)
            {
                return eventList;
            }
        }

    }
}
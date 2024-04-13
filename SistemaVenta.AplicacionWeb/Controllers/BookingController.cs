using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Security.Policy;
using Azure.Core;
using SistemaVenta.BLL.Implementacion;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IEstablishmentService _establishmentService;
        private readonly IGuestService _guestService;
        private readonly IBookingService _bookingService;
        private readonly IMovimientoService _movimientoService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ConfigDTO _apiConfigs;
        private readonly IConverter _converter;
        private readonly IPriceService _priceService;
        private readonly IRoomService _roomService;
        private readonly IServicesService _servicesService;
        private readonly IBookService _bookService;

        public BookingController(IEstablishmentService establishmentService, IMovimientoService movimientoService,
            IGuestService guestService, IMapper mapper, IConverter converter, IBookingService bookingService,
            IConfiguration configuration, IPriceService priceService, IRoomService roomService, IServicesService servicesService,
            IBookService bookService)
        {
            _establishmentService = establishmentService;
            _guestService = guestService;
            _mapper = mapper;
            _converter = converter;
            _bookingService = bookingService;
            _movimientoService = movimientoService;
            _configuration = configuration;
            _apiConfigs = new ConfigDTO();
            _configuration.GetSection("ConfigurationValues").Bind(_apiConfigs);
            _priceService = priceService;
            _roomService = roomService;
            _servicesService = servicesService;
            _bookService = bookService;
        }
        public IActionResult NewBooking()
        {
            return View();
        }
        public IActionResult HistoryBookings()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetEstablishment(string busqueda)
        {
            List<EstablishmentDTO> lista = _mapper.Map<List<EstablishmentDTO>>(await _bookingService.ObtainEstablishments(busqueda));
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        [HttpPost]
        public async Task<IActionResult> GetRooms([FromBody] RequestRoomsBookingDTO data)
        {
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                data.IdEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                List<RoomDTO> lista = _mapper.Map<List<RoomDTO>>(await _bookingService.ObtainRooms(data.IdEstablishment.ToString(), data.CheckIn, data.CheckOut,data.CkeckDates));
                //var lista2 = await _bookingService.ObtainRooms3("3");

                return StatusCode(StatusCodes.Status200OK, lista);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> getGuests(string busqueda)
        {

            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

            List<GuestDTO> lista = _mapper.Map<List<GuestDTO>>(await _guestService.getGuestsByParam(busqueda, idEstablishment));
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        [HttpPost]
        public async Task<IActionResult> GetRangePrices([FromBody] BookDTO data)
        {
            try
            {
                List<Room> tempRooms = _mapper.Map<List<Room>>(data.TempRoom);
                var totalPrice = await _priceService.CalculateTotalPrice(tempRooms, data.CheckIn, data.CheckOut, data.Adults, data.AgeChildren);
                return Ok(totalPrice);

            }
            catch (Exception e)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var idestablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

            List<ServiceDTO> serviceDTOLista = _mapper.Map<List<ServiceDTO>>(await _servicesService.GetServicesByEstablishment(idestablishment));

            return StatusCode(StatusCodes.Status200OK, new { data = serviceDTOLista });

        }

        [HttpGet]
        public async Task<IActionResult> GetBookStatusList()
        {
            List<BookStatusDTO> list = _mapper.Map<List<BookStatusDTO>>(await _bookService.GetBookStatusList());
            return StatusCode(StatusCodes.Status200OK, new { data = list });
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingsByRoomNumber(string number)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            List<VentaBookingDTO> result = new List<VentaBookingDTO>();

            //Obtiene habitaciones por el número de habitacion y estado de reserva
            List<RoomDTO> rooms = _mapper.Map<List<RoomDTO>>(await _roomService.GetRoomsByNumberAndStatus(number, (int)Enums.StatusBooking.Ingreso, idEstablishment));

            if(rooms.Any())
            {
                //Obtiene reservas con estado de "Ingreso"
                List<BookDTO> bookings = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookingsByStatus((int)Enums.StatusBooking.Ingreso, idEstablishment));
                if(bookings.Any())
                {
                    foreach (RoomDTO room in rooms)
                    {
                        VentaBookingDTO venta = new VentaBookingDTO();
                        venta.RoomMain = room;
                        venta.Booking = bookings.Find(x => x.DetailBook.Any(y => y.IdRoom == room.IdRoom));
                        result.Add(venta);
                    }                    
                }  
            }


            return StatusCode(StatusCodes.Status200OK, result);
        }


        [HttpGet]
        public async Task<IActionResult> getBookbyId(string eventId)
        {
            try
            {

                int id = int.Parse(eventId);

                List<BookDTO> bookDTOLista = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookById(id));
                return StatusCode(StatusCodes.Status200OK, new { data = bookDTOLista });
            }
            catch (Exception e)
            {

                throw;
            }
        }
        //public async Task<IActionResult> getBookbyId(int busqueda)
        //{
        //    List<BookDTO> lista = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookById(busqueda));
        //    return StatusCode(StatusCodes.Status200OK, lista);
        //}


        [HttpPost]
        public async Task<IActionResult> SaveBook([FromBody] ResponseBookingDTO data)
        {
            GenericResponse<ResponseBookingDTO> genericResponse = new GenericResponse<ResponseBookingDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                var aa = "aaa";

                int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                Task<bool> task = _bookingService.CheckBookings(data.Guests[0].Document, data.Book.CheckIn, data.Book.CheckOut, idEstablishment);
                await task;
                if (!task.Result)
                {

                    //ClaimsPrincipal claimUser = HttpContext.User;
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value).SingleOrDefault();

                    data.Movement.IdUsuario = int.Parse(idUsuario);
                    data.Movement.IdEstablishment = idEstablishment;

                    Movimiento movement_created = await _movimientoService.Registrar(_mapper.Map<Movimiento>(data.Movement));
                    if(data.Movement.AbonoReserva != null && data.Movement.AbonoReserva != 0)
                    {
                        data.Movement.IdMovimientoRel = movement_created.IdMovimiento;
                        data.Movement.Total = data.Movement.AbonoReserva; //Abono
                        Movimiento movementRel_created = await _movimientoService.Registrar(_mapper.Map<Movimiento>(data.Movement));
                    }

                    var code = 0;

                    Establishment establishment_Found = await _establishmentService.getEstablishmentById(idEstablishment);

                    ICollection<DetailBookDTO> listDetail = new List<DetailBookDTO>();

                    foreach (GuestDTO guestDTO in data.Guests)
                    {
                        DetailBookDTO detailBook = new DetailBookDTO();

                        Room room = await _roomService.GetRoomById(guestDTO.IdRoom);
                        guestDTO.IdEstablishment = idEstablishment;

                        Guest exist = await _guestService.getGuestByDocument(guestDTO.Document);
                        Guest guest_creado = exist == null? await _guestService.Crear(_mapper.Map<Guest>(guestDTO)) : exist;

                        detailBook.IdGuest = guest_creado.IdGuest;
                        detailBook.IdRoom = guestDTO.IdRoom;
                        detailBook.IdCategoria = room.IdCategoria;
                        detailBook.Price = guestDTO.Price;

                        listDetail.Add(detailBook);

                        var client = new HttpClient();

                        //var responseContent = string.Empty;

                        if (_apiConfigs.flagSendApiMinisterio == "1")
                        {


                            if (!guest_creado.IsMain && code > 1)
                            {
                                var url = "https://pms.mincit.gov.co/two/";
                                var content = new StringContent(
                                           JsonSerializer.Serialize(new
                                           {
                                               tipo_identificacion = guest_creado.DocumentType,
                                               numero_identificacion = guest_creado.Document,
                                               nombres = guest_creado.Name,
                                               apellidos = guest_creado.LastName,
                                               cuidad_residencia = guest_creado.RecidenceCity,
                                               cuidad_procedencia = guest_creado.OriginCity,
                                               numero_habitacion = guestDTO.Room,
                                               check_in = data.Book.CheckIn.ToString("yyyy-MM-dd"),
                                               check_out = data.Book.CheckOut.ToString("yyyy-MM-dd"),
                                               padre = code.ToString()
                                           }),
                                           Encoding.UTF8,
                                           "application/json"
                                       );

                                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", " " + establishment_Found.Token);

                                var response = await client.PostAsync(url, content);

                                var responseContent = await response.Content.ReadAsStringAsync();
                                //statuscode 201 is ok
                                var responseObject = JsonSerializer.Deserialize<ResponseMCITDTO>(responseContent);

                                guest_creado.IdMainGuest = code;
                                guest_creado.IdEstablishment = idEstablishment;
                                Guest producto_editado = await _guestService.Editar(guest_creado);

                            }
                            else
                            {

                                var url = "https://pms.mincit.gov.co/one/";

                                var content = new StringContent(
                                            JsonSerializer.Serialize(new
                                            {
                                                tipo_identificacion = guest_creado.DocumentType,
                                                numero_identificacion = guest_creado.Document,
                                                nombres = guest_creado.Name,
                                                apellidos = guest_creado.LastName,
                                                cuidad_residencia = guest_creado.RecidenceCity,
                                                cuidad_procedencia = guest_creado.OriginCity,
                                                numero_habitacion = guestDTO.Room,
                                                motivo = data.Book.Reason,
                                                numero_acompanantes = guest_creado.NumberCompanions.ToString(),
                                                check_in = data.Book.CheckIn.ToString("yyyy-MM-dd"),
                                                check_out = data.Book.CheckOut.ToString("yyyy-MM-dd"),
                                                tipo_acomodacion = establishment_Found.EstablishmentType,
                                                costo = movement_created.Total.ToString(),
                                                nombre_establecimiento = establishment_Found.EstablishmentName,
                                                rnt_establecimiento = establishment_Found.Rnt,

                                            }),
                                            Encoding.UTF8,
                                            "application/json"
                                        );

                                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", " " + establishment_Found.Token);

                                var response = await client.PostAsync(url, content);

                                var responseContent = await response.Content.ReadAsStringAsync();
                                var responseObject = JsonSerializer.Deserialize<ResponseMCITDTO>(responseContent);
                                code = responseObject.code;



                            }
                        }

                        //if (responseObject.code)
                        //{
                        //    return StatusCode(StatusCodes.Status200OK, genericResponse);
                        //}
                        //else
                        //{
                        //    return StatusCode(StatusCodes.Status500InternalServerError, genericResponse);
                        //}

                    }

                    data.Book.IdMovimiento = movement_created.IdMovimiento;
                    data.Book.DetailBook = listDetail;
                    data.Book.IdOrigin = "1";// todo cod: 1 = interno, 2 = Motor, 3 = Airmnb, 4 = Booking
                    data.Book.IdEstablishment = idEstablishment;
                    Book book_created = await _bookingService.Save(_mapper.Map<Book>(data.Book));

                    data.Book = _mapper.Map<BookDTO>(book_created);
                    data.Movement = _mapper.Map<MovimientoDTO>(movement_created);
                    //data.Guests = _mapper.Map<GuestDTO>(movement_created);
                    //modelo = _mapper.Map<BookDTO>(book_created);

                    genericResponse.Estado = true;
                    genericResponse.Objeto = data;

                }
                else
                {
                    genericResponse.Estado = false;
                    genericResponse.Mensaje = "El huesped ya cuenta con reservas en fechas dadas";
                }

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpGet]
        public async Task<IActionResult> HistoryBooking(string bookingNumber, string searchBy, string dateIni, string dateFin)
        {
            try
            {

                ClaimsPrincipal claimUser = HttpContext.User;
                var idCompany = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                var listaventa = await _bookingService.History(bookingNumber, dateIni, dateFin, idCompany);
                List<BookDTO> lista = _mapper.Map<List<BookDTO>>(listaventa);
                return StatusCode(StatusCodes.Status200OK, lista);

            }
            catch (Exception e)
            {

                throw;
            }
        }


        public IActionResult MostrarPDFBooking(string numeroMovimiento)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFMovimiento?numeroMovimiento={numeroMovimiento}&idCompany={idEstablishment}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },

                Objects =  {
                    new ObjectSettings()
                    {
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");

        }

        ///todo unificar en utilidades 
        public EstablishmentObject FormatEstablishmentData(Establishment establishment, string checkin, string checkout, string
          nrooms, string nadults, string nchildren, int idCategory = 0)
        {

            // Hora de llegada
            var checkInTimeSQL = establishment.CheckInTime;
            var checkInTime = string.Format("{0:HH:mm tt}", checkInTimeSQL);

            // Hora de salida
            var checkOutTimeSQL = establishment.CheckOutTime;
            var checkOutTime = string.Format("{0:HH:mm tt}", checkOutTimeSQL);

            DateTime CheckIn = DateTime.Parse(checkin);
            DateTime CheckOut = DateTime.Parse(checkout);

            // Total de dias de la reserva
            var totalDays = CheckIn.Date.Subtract(CheckOut).TotalDays;
            totalDays = (totalDays * -1) + 1;

            var textTotalDays = "noche";
            if (totalDays > 1)
            {
                textTotalDays = "noches";
            }

            // Fecha inicio en formato MM dd, AAAA
            var checkInFormated = CheckIn.ToString("MMMM dd, yyyy");
            var checkInAltFormated = CheckIn.ToString("yyyy/MM/dd");

            // Fecha final en formato MM dd, AAAA
            var checkOutFormated = CheckOut.ToString("MMMM dd, yyyy");
            var checkOutAltFormated = CheckOut.ToString("yyyy/MM/dd");

            // Validacion de contadores y texto
            var numAdult = Int32.Parse(nadults);
            var numChild = nchildren == null ? 0 : Int32.Parse(nchildren);
            var totalPeople = numAdult + numChild;
            var textTotalPeople = "persona";

            if (totalPeople > 1)
            {
                textTotalPeople = "personas";
            }

            var textAdult = "";
            if (numAdult > 1)
            {
                textAdult = "adultos";
            }
            else
            {
                textAdult = "adulto";
            }

            var textChild = "";
            if (numChild > 1)
            {
                textChild = "niños";
            }
            else
            {
                if (numChild == 1)
                {
                    textChild = "niño";
                }
            }

            // Validate 0 on child counter
            var countChild = "";
            if (nchildren != "0")
            {
                countChild = nchildren;
            }

            // EstablishmentObject
            var result = new EstablishmentObject
            {
                IdEstablishment = establishment.IdEstablishment,
                EstablishmentName = establishment.EstablishmentName,
                EstablishmentUrlImg = establishment.UrlImage,
                EstablishmentAddress = establishment.Address,
                CheckInFormated = checkInFormated,
                CheckOutFormated = checkOutFormated,
                CheckIn = checkInAltFormated,
                CheckOut = checkOutAltFormated,
                CheckInTime = checkInTime,
                CheckOutTime = checkOutTime,
                TotalDaysBooking = totalDays,
                TextTotalDays = textTotalDays,
                NumRoom = nrooms,
                NumAdult = nadults,
                NumChildren = countChild,
                TextAdult = textAdult,
                TextChild = textChild,
                TotalPeople = totalPeople,
                TxtTotalPeople = textTotalPeople
            };
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> ListarBooksCheckout()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            List<ConfirmBookingDTO> bookingsCheckout = new List<ConfirmBookingDTO>();

            //Obtiene reservas actuales con estado de "Ingreso"
            List<BookDTO> bookings = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookingsByStatus((int)Enums.StatusBooking.Ingreso, idEstablishment));
            if (bookings.Any())
            {
                foreach (BookDTO book in bookings)
                {
                    foreach (var detbook in book.DetailBook)
                    {
                        ConfirmBookingDTO item = new ConfirmBookingDTO();
                        item.Book = book;
                        //COnsulta mvtos relacionados a la reserva
                        var mvtosRel =  await this._movimientoService.GetMovimientosBooking(book.IdMovimiento.Value);
                        //Filtra solo los mvtos credito
                        var mvtosRelFilter = _mapper.Map<List<MovimientoDTO>>(mvtosRel).FindAll(x => x.IdTipoDocumentoMovimiento != (int)Enums.TipoDocMov.Credito);
                        //Suma los abonos
                        item.Book.IdMovimientoNavigation.AbonoReserva = mvtosRelFilter.Sum(x => x.Total);
                        item.Room = _mapper.Map<RoomDTO>(await _roomService.GetRoomById(detbook.IdRoom));
                        bookingsCheckout.Add(item);
                    }
                }
            }
            //}
            return StatusCode(StatusCodes.Status200OK, new { data = bookingsCheckout });
        }



    }
}

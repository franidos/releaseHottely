using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Text;

using Newtonsoft.Json.Linq;
using System.Globalization;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using System.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace nextadvisordotnet.AppWeb.Controllers
{
    public class MotorReservasController : Controller
    {
        private readonly IGenericRepository<Room> _repositorioRoom;
        private readonly IMapper? _mapper;
        //private readonly INegocioService _negocioService;
        //private readonly ICompanyService? _companyService;
        private readonly IEstablishmentService? _establishmentService;
        private readonly IBookingService _bookingService;
        private readonly IImageService? _imageService;
        private readonly IRoomService? _roomService;
        private readonly IPriceService _priceService;
        private readonly IServicesService _servicesService;
        private readonly IPedidoService _movimientoService;
        private readonly IGuestService _guestService;
        private readonly ICorreoService _correoService;
        private readonly IBookService _bookService;
        private readonly IConfiguration _configuration;
        private readonly IUtilidadesService _utils;

        public MotorReservasController(
            IGenericRepository<Room> repositorioRoom,
            IMapper mapper,
            IEstablishmentService establishmentService,
            IBookingService bookingService,
            IImageService imageService,
            IRoomService roomService,
            IPriceService priceService,
            IServicesService servicesService,
            IPedidoService movimientoService,
            IGuestService guestService,
            ICorreoService correoService, 
            IBookService bookService, 
            IConfiguration configuration,
            IUtilidadesService utils)
        {
            _repositorioRoom = repositorioRoom;
            _mapper = mapper;
            _establishmentService = establishmentService;
            _bookingService = bookingService;
            _roomService = roomService;
            _priceService = priceService;
            _servicesService = servicesService;
            _movimientoService = movimientoService;
            _guestService = guestService;
            _correoService = correoService;
            _bookService = bookService;
            _configuration = configuration;
            _utils = utils;
            _imageService = imageService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            // Obtengo parametros URL
            string parametroIdEstablecimiento = HttpContext.Request.Query["establecimientoid"];

            if (parametroIdEstablecimiento == null)
            {
                ViewData["Mensaje"] = "Establecimiento no encontrado...";
                return View("Index");
            }

            // Obtener datos
            try
            {
                int idEstablecimientoParaBuscar = Int32.Parse(parametroIdEstablecimiento);

                if (idEstablecimientoParaBuscar > 0)
                {
                    // Consulto la empresa 
                    var datosEstablecimiento = await _establishmentService.getEstablishmentById(idEstablecimientoParaBuscar);

                    if (datosEstablecimiento == null)
                    {
                        return View("Index");
                    }

                    // Imagenes del establecimiento ListImagesByEstablishment

                    // Lleno el input establishmentIdParam para luego enviarlo a ListadoCuartosAsync en el submit
                    ViewBag.establishmentIdParam = idEstablecimientoParaBuscar;
                    ViewBag.establishmentNameParam = datosEstablecimiento.EstablishmentName;
                    ViewBag.establishmentUrlImgParam = datosEstablecimiento.UrlImage;

                    ViewData["NombreEstablecimiento"] = datosEstablecimiento.EstablishmentName;
                    ViewData["Address"] = datosEstablecimiento.Address;
                    ViewData["UrlImage"] = datosEstablecimiento.UrlImage;
                    ViewData["Email"] = datosEstablecimiento.Email;
                    ViewData["Phone"] = datosEstablecimiento.PhoneNumber;
                    ViewData["Contact"] = datosEstablecimiento.Contact;
                    ViewData["Descripcion"] = datosEstablecimiento.Descripcion;
                    ViewData["gps"] = datosEstablecimiento.Geolocation;
                }
            }
            catch (Exception ex)
            {
                return View("Index");
            }
            
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetRooms([FromBody] RequestRoomsBookingDTO data)
        {
            List<RoomDTO> lista = _mapper.Map<List<RoomDTO>>(await _bookingService.ObtainRooms(data.IdEstablishment.ToString(), data.CheckIn, data.CheckOut));

            // return View(lista);

            return StatusCode(StatusCodes.Status200OK, lista);
        }

        // Paso 2, Seleccion de Habitacion
        public async Task<IActionResult> ListadoCuartosAsync(string daterange, string establishmentId, string establishmentName,
            string countRoomsInput, string countAdultsInput, string countChildrenInput, string establishmentUrlImg)
        {

            // FALTA VALIDAR ATRIBUTOS
            // Edades, como obtener dinamicamente ????

            // Obtener data del establecimiento:
            var selectedEstablishment = await _establishmentService.getEstablishmentById(Int32.Parse(establishmentId));

            var dates = daterange.Split(" - ");
            DateTime CheckIn = DateTime.Parse(dates[0]);
            DateTime CheckOut = DateTime.Parse(dates[1]);
            if(CheckIn == CheckOut)            
                CheckOut = CheckOut.AddDays(1);
            
            // Lista de cuartos
            //List<RoomDTO> roomsListSQL = _mapper.Map<List<RoomDTO>>(await _bookingService.ObtainRooms(establishmentId, CheckIn, CheckOut));
            //var roomList = roomsListSQL.ToList();
            //var numAvailableRooms = roomList.Count;

            int idEstablishment = int.Parse(establishmentId);

            /*
            var roomsOld = await _repositorioRoom.Consultar(p =>
                p.IsActive == true &&
                p.Status == true &&
                p.IdEstablishment == idEstablishment &&
                !p.DetailBook.Any(d =>
                    (CheckIn >= d.IdBookNavigation.CheckIn && CheckIn < d.IdBookNavigation.CheckOut) ||
                    (CheckOut > d.IdBookNavigation.CheckIn && CheckOut <= d.IdBookNavigation.CheckOut) ||
                    (CheckIn <= d.IdBookNavigation.CheckIn && CheckOut >= d.IdBookNavigation.CheckOut)
                )
            );
            
            var rooms2 = await roomsOld
            .Include(r => r.IdCategoriaNavigation)
            .ToListAsync();
            */

            // Obtener info de Rooms + Categoria
            var listOfRooms = await _bookingService.ObtainRoomsWithCategory(idEstablishment, CheckIn, CheckOut);

            /*****************************************************
            // Obtener los datos de la tabla "Categoria"
            IQueryable<Categoria> categorias = await _categoriaRepository.Consultar();

            // Obtener los datos de la tabla "RoomPrice"
            IQueryable<RoomPrice> roomPrices = await _roomPriceRepository.Consultar();

            // Obtener los datos de la tabla "Room"
            IQueryable<Room> rooms = await _roomRepository.Consultar();
            *****************************************************/

            // Validar si existen cuartos disponibles
            var numAvailableRooms = listOfRooms.Count;

            if (numAvailableRooms == 0 )
            {
                ViewData["Mensaje"] = "No hay habitaciones para la fecha seleccionada...";
                var establecimientoid = establishmentId;

                return RedirectToAction(nameof(Index), new { establecimientoid });
            }

            var establishmentData = FormatEstablishmentData(selectedEstablishment, dates[0], dates[1], countRoomsInput, countAdultsInput, countChildrenInput);

            ViewBag.numAvailableRooms = numAvailableRooms;
            ViewBag.selectedEstablishment = establishmentData;

            return View("AvailableRooms", listOfRooms);
        }

        public async Task<IActionResult> GetRoomImages(int idRoom)
        {
           var images =await _imageService.ListImagesByRoom(idRoom);
            return Json(images);
        }

        public IActionResult GetEstablishmentImages(int idEstablishment)
        {
            var images = _imageService.ListImagesByEstablishment(idEstablishment);
            return Json(images.Result);
        }

        public async Task<IActionResult> GetServicesInfoEstablishment(int idEstablishment)
        {
            List<ServiceInfoDTO> res = new List<ServiceInfoDTO>();
            List<ServiceInfo> listServiceInfo = await _servicesService.GetServiceInfo();
            List<ServiceInfoEstablishment> listServiceEstabl = await _servicesService.GetServiceInfoByEstablishment(idEstablishment);

            foreach (ServiceInfoEstablishment srv in listServiceEstabl)
            {
                ServiceInfo servBase = listServiceInfo.Find(x => x.IdServiceInfo == srv.IdServiceInfo);
                ServiceInfoDTO d = new ServiceInfoDTO() { IdServiceInfo = srv.IdServiceInfo.Value, 
                                                          Icon = servBase.Icon,
                                                          Descripcion = srv.DescripcionOpc ?? servBase.Descripcion, 
                                                          On = true};
                res.Add(d);
            }  
            return Json(res);
        }

        public async Task<ActionResult> ServiciosAsync(string idestablishment, int idroom, string checkin, string checkout, string nrooms, string nadults, string nchildren)
        {

            /*
            var url = '/MotorReservas/Servicios?idestablishment=' + idestabl + '&idroom=' + idroom + '&checkin=' + checkin +
                '&checkout=' + checkout + '&nrooms=' + nrooms + '&nadults=' + nadults + '&nchildren=' + nchildren;
            */


            if (idestablishment == null || idroom == 0 || checkin == null || checkout == null || nrooms == null || nadults == null || nchildren == null )
            {
                ViewData["Mensaje"] = "Establecimiento no encontrado...";
                return View("ListadoCuartos");
            }

            // Id Establecimiento
            var idestablishmentInteger = int.Parse(idestablishment);

            // Consultar data del establecimiento
            var selectedEstablishment = await _establishmentService.getEstablishmentById(idestablishmentInteger);


            // Consultar data del Room
            var selectedRoom = await _roomService.GetRoomById(idroom);

            if (selectedEstablishment == null || selectedRoom == null)
            {
                ViewData["Mensaje"] = "Establecimiento no encontrado...";
                return View("ListadoCuartos");
            }

            var idCategory = selectedRoom.IdCategoria;
            var establishmentData = FormatEstablishmentData(selectedEstablishment, checkin, checkout, nrooms, nadults, nchildren, idCategory);


            // Obtener el precio de la habitacion
            DateTime CheckIn = DateTime.Parse(checkin);
            decimal? priceByDate = await _priceService.GetPriceForDate(CheckIn, idCategory, idestablishmentInteger);

            if (priceByDate == null)
            {
                priceByDate = 0;
            }
            else
            {
                priceByDate = Math.Round((decimal)priceByDate, 2);
            }

            // Listar servicios disponibles
            var listService = await _servicesService.GetServicesByEstablishment(idestablishmentInteger);

            ViewBag.selectedEstablishment = establishmentData;
            ViewBag.selectedRoom = selectedRoom;
            ViewBag.priceOfSelectedRoom = priceByDate;
            ViewBag.isTheForm = false;
            //ViewBag.listService = listService;

            return View("Services", listService);
        }

        public async Task<ActionResult> FormAsync(int idestablishment, int idroom, string checkin, string checkout, string nrooms, 
            string nadults, string nchildren, string totalservices, string priceroom)
        {

            /*
              var url = '/MotorReservas/Form?idestablishment=' + idestabl + '&idroom=' + idroom + '&checkin=' + checkin +
                    '&checkout=' + checkout + '&nrooms=' + nrooms + '&nadults=' + nadults + '&nchildren=' + nchildren + '&services=' + base64String;
            */

            try
            {

                if (idestablishment == null || idroom == 0 || checkin == null || checkout == null || nrooms == null || nadults == null)
                {
                    ViewData["Mensaje"] = "Establecimiento no encontrado...";
                    return View("ListadoCuartos");
                }

                // Consultar data del establecimiento
                var selectedEstablishment = await _establishmentService.getEstablishmentById(idestablishment);

                // Consultar data del Room
                var selectedRoom = await _roomService.GetRoomById(idroom);

                if (selectedEstablishment == null || selectedRoom == null)
                {
                    ViewData["Mensaje"] = "Establecimiento no encontrado...";
                    return View("ListadoCuartos");
                }

                var idCategory = selectedRoom.IdCategoria;
                var establishmentData = FormatEstablishmentData(selectedEstablishment, checkin, checkout, nrooms, nadults, nchildren, idCategory);

                /*
                // Base64
                // Paso 1: Decodificar la cadena Base64 a una cadena JSON
                string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(services));

                // Paso 2: Deserializar la cadena JSON a un objeto de tipo JsonToObject
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                JsonToObject objeto = JsonSerializer.Deserialize<JsonToObject>(jsonString, options);

                // Paso 3: Utilizar la lista "servicios" del objeto deserializado como una lista de JsonService
                List<JsonService> serviciosList = objeto.servicios;

                // Valor total de todos los servicios
                long totalPorServicio = (long)objeto.totalValue;
                */

                // Convertir las cadenas a números flotantes
                float floatValue1;
                float floatValue2;
                var total = 0.0;

                if (float.TryParse(priceroom, out floatValue1) && float.TryParse(totalservices, out floatValue2))
                {
                    total = floatValue1 + floatValue2;
                }

                ViewBag.selectedEstablishment = establishmentData;
                ViewBag.selectedRoom = selectedRoom;
                ViewBag.priceOfSelectedRoom = priceroom;
                // ViewBag.servicesList = serviciosList;
                ViewBag.totalServices = totalservices;
                ViewBag.isTheForm = true;
                // ViewBag.services64 = services;
                ViewBag.total = total;

                return View("Form");
            }

            catch (Exception ex)
            {
                return View("Index");
            }
        }

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


        public async Task<IActionResult> ValidarFormularioAsync(int idestabl, int idroom, string checkin, 
            string checkout, string nrooms, string nadults, string nchildren, float priceroom, string services64, 
            string DocumentType, string Document, string Name, string LastName, string OriginCity, string RecidenceCity, 
            string PhoneNumber, string Email, string Reason, float totalservices, string nationality, string country,string codeCountry, string tratamiento)
        {

            var nchildrenVal = nchildren == null ? "0" : nchildren;
            GenericResponse<ResponseBookingDTO> genericResponse = new GenericResponse<ResponseBookingDTO>();

            try
            {

                if (idestabl == null || idroom == null || checkin == null || checkout == null || nrooms == null || nadults == null ||
                   priceroom == null)
                {
                    ViewData["Mensaje"] = "Establecimiento no encontrado...";
                    return View("ListadoCuartos");
                }

                // Consultar data del establecimiento
                var selectedEstablishment = await _establishmentService.getEstablishmentById(idestabl);

                // Consultar data del Room
                var selectedRoom = await _roomService.GetRoomById(idroom);

                if (selectedEstablishment == null || selectedRoom == null)
                {
                    ViewData["Mensaje"] = "Establecimiento no encontrado...";
                    return View("ListadoCuartos");
                }

                var idCategory = selectedRoom.IdCategoria;
                var establishmentData = FormatEstablishmentData(selectedEstablishment, checkin, checkout, nrooms, nadults, nchildrenVal, idCategory);
                var total = priceroom + totalservices;

                ViewBag.selectedEstablishment = establishmentData;
                ViewBag.selectedRoom = selectedRoom;
                // BUG::: ViewBag.priceOfSelectedRoom = priceroom;
                ViewBag.priceOfSelectedRoom = priceroom;
                ViewBag.priceRoom = priceroom;
                ViewBag.services64 = services64;
                // ViewBag.totalValueServices = totalPorServicio; por totalServices
                ViewBag.totalServices = totalservices;
                ViewBag.isTheForm = true;
                ViewBag.total = total;
                ViewBag.numdoc = Document;
                ViewBag.emailGuest = Email;
                ViewBag.nameServiceToPay = "Reserva: " + selectedEstablishment.EstablishmentName + ' ' + selectedRoom.CategoryName;
                ViewBag.urlResponsePay = $"{this.Request.Scheme}://{this.Request.Host}/MotorReservas/RespuestaPago";
                ViewBag.urlConfirmPay = $"{this.Request.Scheme}://{this.Request.Host}/MotorReservas/ConfirmacionPago";

                string format = "yyyy/MM/dd";  // El formato de las cadenas de fecha

                try
                {
                    DateTime checkinDateTime  = DateTime.ParseExact(checkin, format, CultureInfo.InvariantCulture);
                    DateTime checkoutDateTime = DateTime.ParseExact(checkout, format, CultureInfo.InvariantCulture);

                    Task<bool> task = _bookingService.CheckBookings(Document, checkinDateTime, checkoutDateTime, establishmentData.IdEstablishment);

                    //
                    await task;
                    if (!task.Result)
                    {
                        // Movement
                        Movimiento newMovement = new()
                        {
                            DocumentoCliente = Document,
                            NombreCliente = Name + " " + LastName,
                            Total = (int)total,
                            IdEstablishment = idestabl
                        };

                        Movimiento movement_created = await _movimientoService.Registrar(_mapper.Map<Movimiento>(newMovement));

                        List<DetailBookDTO> listDetail = new List<DetailBookDTO>();
                        DetailBookDTO detailBook = new DetailBookDTO();

                        GuestDTO newGuest = new()
                        {
                            IdEstablishment = establishmentData.IdEstablishment,
                            DocumentType = DocumentType,
                            Document = Document,
                            Name = Name,
                            LastName = LastName,
                            RecidenceCity = RecidenceCity,
                            OriginCity = OriginCity,
                            NumberCompanions = 0,                          
                            IsChild = false,
                            IsMain = 1,
                            PhoneNumber = codeCountry + PhoneNumber,
                            Email = Email,
                            Nationality = nationality, 
                            OriginCountry = country
                        }; 

                        Guest guest_creado = await _guestService.Crear(_mapper.Map<Guest>(newGuest));                     

                        detailBook.IdGuest = guest_creado.IdGuest;
                        detailBook.IdRoom = idroom;
                        detailBook.IdCategoria = selectedRoom.IdCategoria;
                        //detailBook.Price = (decimal?)newGuest.Price;

                        listDetail.Add(detailBook);

                        string adultString = nadults != null? string.Concat(Enumerable.Repeat("A,", Convert.ToInt32(nadults))) : "A";
                        string childrenString = nchildren != null? string.Concat(Enumerable.Repeat("1,", Convert.ToInt32(nchildren))) : "";//No vienen edades y es fuerte el cambio p
                        BookDTO newO = new()
                        {
                            IdMovimiento = movement_created.IdMovimiento,
                            IdOrigin = "2", // todo cod: 1 = interno, 2 = Motor, 3 = Airmnb, 4 = Booking
                            Reason = Reason,
                            CheckIn = checkinDateTime,
                            CheckOut = checkoutDateTime,
                            Adults = adultString.Substring(0, adultString.Length - 1),
                            AgeChildren = nchildren != null ? childrenString.Substring(0, childrenString.Length - 1) : "",
                            DetailBook = listDetail,
                            IdEstablishment = establishmentData.IdEstablishment,
                            IdBookStatus = (int)Enums.StatusBooking.ReservaSinConfirmar
                        };

                        Book book_created = await _bookingService.Save(_mapper.Map<Book>(newO));
                        ViewBag.idBook = book_created.IdBook;

                        //Datos temporales 
                        TempData["tratamiento"] = tratamiento;
                        TempData["idBook"]  = book_created.IdBook;
                        TempData["emailGuest"] = Email;
                        TempData["idEstableishment"] = selectedEstablishment.IdEstablishment;
                        TempData["idBook"] = book_created.IdBook;
                        TempData["total"] = (int)total;
                    }
                    else
                    {
                        ////TODO: MENSAJE O VISTA PARA AVISAR NO HAY AGENDA EN ESAS FECHAS
                    }

                }
                catch (FormatException)
                {                    
                    // Manejo de errores en caso de que el formato no sea válido
                }

                // accor.yopi@yopmail.com
                return View("Payment");
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }

        public async Task<IActionResult> RespuestaPagoAsync()
        {
            //Referencia de payco que viene por url
            string ref_payco = HttpContext.Request.Query["ref_payco"];
            //Url Rest Metodo get, se pasa la llave y la ref_payco como paremetro
            var url = "https://secure.epayco.co/validation/v1/reference/" + ref_payco;

            ViewBag.isPay = true; //Controla la vista final a mostrar
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string jsonContent = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(jsonContent);

                    var dataOK = data["data"];
                    var responseStatus = dataOK["x_cod_response"];
                    var status = responseStatus != null? (int)responseStatus : 0;

                    if (status == 1)
                    {
                        //-Obtiene datos del pago y Actualiza Reserva y realiza Envio de Correo
                        string? documentClient = dataOK["x_extra1"]?.ToString();
                        string? idEstableishment = dataOK["x_extra2"]?.ToString();
                        string? idBook = dataOK["x_extra3"]?.ToString();
                        string? emailGuest = dataOK["x_extra4"]?.ToString();
                        string? tratamiento = TempData["tratamiento"]?.ToString() ?? null;

                        List<BookDTO> books = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookById(Convert.ToInt32(idBook)));
                        BookDTO book = books.FirstOrDefault();                        
                        book.IdBookStatus = Convert.ToDecimal(dataOK["x_amount_ok"]) == book?.IdMovimientoNavigation?.Total? 
                                           (int)Enums.StatusBooking.ReservaConfirmada : (int)Enums.StatusBooking.ReservaParcial;
                        
                        Book book_editado = await _bookService.Editar(_mapper.Map<Book>(book));

                        bool sendOk = await this.SendMailBooking(idEstableishment, idBook, emailGuest, tratamiento);
                       if(sendOk)
                            return View("Response");
                       else
                            return RedirectToAction("Error");
                    }
                    else
                        return RedirectToAction("Error");
                    
                }
                catch (HttpRequestException e)
                {
                    return RedirectToAction("Error");
                }
            }
        }

        public async Task<IActionResult> ConfirmBookingNotPay()
        {
            ViewBag.isPay = false; //Controla la vista final a mostrar
            if (TempData.Any())
            {
                ViewBag.totalPay = TempData["total"]?.ToString();
                bool senMail = await SendMailBooking(TempData["idEstableishment"]?.ToString(), TempData["idBook"]?.ToString(), TempData["emailGuest"]?.ToString(),
                                                     TempData["tratamiento"]?.ToString());
                if (senMail)
                    return View("Response");
                else
                    return RedirectToAction("Error");
            }
            else
                return RedirectToAction("Error");
        }

        public async Task<IActionResult> ConfirmacionPagoAsync()
        {
            return View("Payment");
        }


        /// <summary>
        /// Metodo para enviar correo de la reserva
        /// </summary>
        /// <returns></returns>
        private async Task<bool>  SendMailBooking(string? idEstableishment, string? idBook, string? emailGuest, string? tratamiento) 
        {  
            //Solicitud a la plantilla de correo
            string urlPLantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/BookingConfirm?idBook={idBook}&idCompany={idEstableishment}&tratamiento={tratamiento}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPLantillaCorreo);
            HttpWebResponse responseHtml = (HttpWebResponse)request.GetResponse();
            string htmlCorreo = "";
            bool correo_enviado = false;
            if (responseHtml.StatusCode == HttpStatusCode.OK)
            {
                using (Stream dataStream = responseHtml.GetResponseStream())
                {
                    StreamReader readerStream = null;
                    if (responseHtml.CharacterSet == null)
                    {
                        readerStream = new StreamReader(dataStream);
                    }
                    else
                    {
                        readerStream = new StreamReader(dataStream, Encoding.GetEncoding(responseHtml.CharacterSet));
                    }
                    htmlCorreo = readerStream.ReadToEnd();
                    responseHtml.Close();
                    readerStream.Close();
                }
            }
            else            
                return false;            
           
            //Envia Correo
            if (htmlCorreo != null && emailGuest != "")
                correo_enviado = await _correoService.EnviarCorreo(emailGuest, "Hotelly - Confirmación de su reserva", htmlCorreo);

            if (!correo_enviado)
                throw new TaskCanceledException("Error: Intentalo de nuevo o mas tarde");

            return true;
        }

        public async Task<IActionResult> CalendarEvents(string e, string r)
        {
            var calendar = new Ical.Net.Calendar();
            const string defaultPrdId = "-//admin.hotelly.com//NONSGML ical.net 4.2//EN";//(PRODID)      
            calendar.ProductId = "-//NextGenCorp//Hotelly Calendar//ES";
            calendar.Name = "//Hotelly Calendar V1.0//";//(BEGIN)
            calendar.Method = "PUBLISH"; //(METHOD)

            int idEst = Convert.ToInt32(e); //Convert.ToInt32(_utils.Decrypt(e));
            int idRoom = Convert.ToInt32(r); //Convert.ToInt32(_utils.Decrypt(r));

            // 1. BOOKINGS HOTTELY
            double daysPrev = Convert.ToDouble(_configuration.GetSection("ConfigurationValues:daysPrevEventsCalendar").Value);
            List<Book> bookDTOList = await _bookService.ListByRoom(idRoom, idEst, DateTime.Today.AddDays(-daysPrev));
            List<object> eventList = new List<object>();

            foreach (var bookDTO in bookDTOList)
            {              
                var icalEvent = new CalendarEvent
                {
                    Summary = "HOTELLY - RESERVA",
                    Description = $"{bookDTO.IdMovimientoNavigation.NombreCliente}",
                    Start = new CalDateTime(bookDTO.CheckIn),                                                          
                    End = new CalDateTime(bookDTO.CheckOut),
                    Uid = Guid.NewGuid().ToString() + "@hotelly.com",
                    Status = bookDTO?.IdBookStatusNavigation?.StatusName
                };
                calendar.Events.Add(icalEvent);
            }           

            var iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(calendar).Replace(defaultPrdId, calendar.ProductId); ;

            //string idEstEncr = _utils.Encrypt(e);
            //string idRoomEncr = _utils.Encrypt(r);

            return File(Encoding.ASCII.GetBytes(result), "text/calendar", $"calendar-hotelly_{idEst}_{idRoom}.ics");
        }

    }
}

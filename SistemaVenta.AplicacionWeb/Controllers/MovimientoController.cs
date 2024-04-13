using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Firebase.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using nextadvisordotnet.AppWeb.Utilidades.Extensiones;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Principal;
using static System.Reflection.Metadata.BlobBuilder;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class MovimientoController : Controller
    {
        private readonly ITipoDocumentoMovimientoService _tipoDocumentoMovimientoService;
        private readonly IMovimientoService _ventaService;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;
        private readonly ICajaService _cajaService;
        private readonly IDetalleCajaService _detalleCajaService;
        private readonly IBookService _bookService;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly IMedioPagoService _medioPagoService;
        private readonly IEstablishmentService? _establishmentService;
        private readonly IAreaFisicaService? _areaFisicaService;
        public MovimientoController(ITipoDocumentoMovimientoService tipoDocumentoMovimientoService, 
            IMovimientoService ventaService, IMapper mapper, IConverter converter,
            ICajaService cajaService, IDetalleCajaService detalleCajaService, IBookService bookService, 
            IRoomService roomService, IBookingService bookingService, IMedioPagoService medioPagoService, 
            IEstablishmentService establishmentService, IAreaFisicaService areaFisicaService)
        {
            _tipoDocumentoMovimientoService = tipoDocumentoMovimientoService;
            _ventaService = ventaService;
            _mapper = mapper;
            _converter = converter;
            _cajaService = cajaService;
            _detalleCajaService = detalleCajaService;
            _bookService = bookService;
            _roomService = roomService;
            _bookingService = bookingService;
            _medioPagoService = medioPagoService;
            _establishmentService = establishmentService;
            _areaFisicaService = areaFisicaService;

        }
        public IActionResult NuevoMovimiento()
        {
            return View();
        }
        public IActionResult HistorialMovimientos()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoMovimiento()
        {
            List<TipoDocumentoMovimientoDTO> lista = _mapper.Map<List<TipoDocumentoMovimientoDTO>>(await _tipoDocumentoMovimientoService.Lista());
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            List<ProductoDTO> lista = _mapper.Map<List<ProductoDTO>>(await _ventaService.ObtenerProductos(idEstablishment,busqueda));
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarMovimiento([FromBody]MovimientoDTO modelo)
        {
            GenericResponse<MovimientoDTO> genericResponse = new GenericResponse<MovimientoDTO>();
            MovimientoDTO resultVenta = null;
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario =  claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();               
                int idEstablecimiento = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                modelo.IdUsuario = int.Parse(idUsuario);
                modelo.IdEstablishment = idEstablecimiento;

                //--Obtiene caja usuario
                Caja cajaUser = _cajaService.ObtenerCajaUsuario(int.Parse(idUsuario));

                //--Crear movimiento Venta
                if(modelo.IdMedioPago != (int)Enums.PayMethod.EntradaEfectivo && modelo.IdMedioPago != (int)Enums.PayMethod.SalidaEfectivo)
                {                    
                    if (modelo.IdBookCheckout.HasValue)
                    {
                        MovimientoDTO movCheckout = _mapper.Map<MovimientoDTO>(modelo);

                        //--Guarda el saldo de la reserva
                        movCheckout.Total = movCheckout.SaldoReserva;
                        Movimiento venta_creada = await _ventaService.Registrar(_mapper.Map<Movimiento>(movCheckout));
                        resultVenta = _mapper.Map<MovimientoDTO>(venta_creada);
                        modelo.NumeroMovimiento = resultVenta.NumeroMovimiento;

                        //--Guarda el valor de caja credito
                        if(movCheckout.ValorCaja.HasValue && movCheckout.ValorCaja != 0)
                        {
                            movCheckout.Total = movCheckout.ValorCaja;
                            venta_creada = await _ventaService.Registrar(_mapper.Map<Movimiento>(movCheckout));
                        }

                        //--Guarda el costo adicional reserva
                        if (movCheckout.CostoAdic.HasValue && movCheckout.CostoAdic != 0)
                        {
                            movCheckout.Total = modelo.CostoAdic;
                            venta_creada = await _ventaService.Registrar(_mapper.Map<Movimiento>(movCheckout));
                        }                            

                        //--Modifica el estado de la reserva
                        var books = await _bookingService.GetBookById(modelo.IdBookCheckout.Value);
                        Book? book = books.FirstOrDefault();
                        book.IdBookStatus = (int)Enums.StatusBooking.ReservaTerminada;
                        var res = await _bookingService.Editar(book, false, false);
                    }
                    else
                    {
                        Movimiento venta_creada = await _ventaService.Registrar(_mapper.Map<Movimiento>(modelo));
                        resultVenta = _mapper.Map<MovimientoDTO>(venta_creada);
                        modelo.NumeroMovimiento = resultVenta.NumeroMovimiento;
                    }                 
                }               

                //--Crear Detalle Caja              
                if(cajaUser != null)
                {
                    DetalleCaja det = new DetalleCaja();
                    det.IdCaja = cajaUser.IdCaja;
                    det.IdMedioPago = modelo.IdMedioPago != null? modelo.IdMedioPago.Value : 0;
                    det.IdMovimiento = resultVenta?.IdMovimiento;
                    //--Si es mixto guarda el valor restante del metodo efectivo registrado
                    det.Valor = det.IdMedioPago == (int)Enums.PayMethod.Mixto? (modelo.Total.Value - modelo.TotalEfectivoMixto.Value) : modelo.Total.Value;
                    det.Observacion = modelo.Observacion;
                    var resDet = await _detalleCajaService.Crear(det);

                    //--Si es mixto guarda el efectivo registrado en un nuevo registro detalle
                    if (det.IdMedioPago == (int)Enums.PayMethod.Mixto)
                    {
                        DetalleCaja detEfectivoMixto = new DetalleCaja();
                        detEfectivoMixto.IdCaja = cajaUser.IdCaja;
                        detEfectivoMixto.IdMedioPago = (int)Enums.PayMethod.Efectivo;
                        detEfectivoMixto.IdMovimiento = resultVenta?.IdMovimiento;
                        detEfectivoMixto.Valor = modelo.TotalEfectivoMixto.Value;
                        detEfectivoMixto.Observacion = modelo.Observacion;
                        var resEfectivoMixto = await _detalleCajaService.Crear(detEfectivoMixto);
                    }
                }

                if(modelo.PrintTicket && cajaUser.IdAreaNavigation != null)
                    await PrintDocument(modelo, cajaUser.IdAreaNavigation.NombreImpresora);

                genericResponse.Estado = true;
                genericResponse.Objeto = resultVenta;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPost]
        public async Task<IActionResult> PrintDocument(MovimientoDTO mov, string? nombreImpresora)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            string idUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            string nombreUsuario = claimUser.Claims
                   .Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();

            var selectedEstablishment = await _establishmentService.getEstablishmentById(idEstablishment);

            GenericResponse<Caja> response = new GenericResponse<Caja>();
            PrintOptions ticket = new PrintOptions();

            ticket.AddRowLine(selectedEstablishment.EstablishmentName, TypeRow.Title);
            ticket.AddRowLine("NIT:"+ selectedEstablishment.NIT, TypeRow.Title);
            ticket.AddRowLine(selectedEstablishment.Address + " - " + selectedEstablishment.City , TypeRow.Header);
            ticket.AddRowLine(selectedEstablishment.Province + " - " + selectedEstablishment.Country, TypeRow.Header);
            ticket.AddRowLine("Contacto:" + selectedEstablishment.Contact, TypeRow.Header);
            ticket.AddRowLine("Tel:" + selectedEstablishment.PhoneNumber, TypeRow.Header);

            ticket.AddRowLine((mov.IdTipoDocumentoMovimiento == 1? "Recibo #": "Factura Venta #") + mov.NumeroMovimiento, TypeRow.SubHeader);
            ticket.AddRowLine("Fecha: " + DateTime.Now.ToString(), TypeRow.SubHeader);
            ticket.AddRowLine("Cliente: " + mov.NombreCliente, TypeRow.SubHeader);
            ticket.AddRowLine("Documento: " + mov.DocumentoCliente, TypeRow.SubHeader);
            ticket.AddRowLine("Caja: " + nombreUsuario, TypeRow.SubHeader);   

            if(mov.IdTipoDocumentoMovimiento == (int)Enums.TipoDocMov.FacturaVenta)
            {
                foreach (var item in mov.DetalleMovimiento)
                {
                    item.SubTotal = Math.Round((item.Total ?? 0) * (Convert.ToDecimal(selectedEstablishment.Tax) / 100), 2);
                    ticket.AddRowLine($"{item.Cantidad}&&{item.DescripcionProducto}&&{item.Total - item.SubTotal.Value}", TypeRow.Detail);
                }                  
                ticket.AddRowLine("SUBTOTAL&&" + (mov.DetalleMovimiento.Sum(x => x.Total) - mov.DetalleMovimiento.Sum(x => x.SubTotal)) , TypeRow.Total);
                ticket.AddRowLine("IVA&&" + mov.DetalleMovimiento.Sum(x => x.SubTotal), TypeRow.Total);
                ticket.AddRowLine("TOTAL&&" + mov.DetalleMovimiento.Sum(x => x.Total), TypeRow.Total);
            }
            else
            {
                foreach (var item in mov.DetalleMovimiento)
                    ticket.AddRowLine($"{item.Cantidad}&&{item.DescripcionProducto}&&{item.Total}", TypeRow.Detail);

                ticket.AddRowLine("SUBTOTAL&&" + mov.DetalleMovimiento.Sum(x => x.Total), TypeRow.Total);              
                ticket.AddRowLine("TOTAL&&" + mov.DetalleMovimiento.Sum(x => x.Total), TypeRow.Total);
            }

            ticket.AddRowLine("-", TypeRow.Total);
            if (mov.TotalRecibido.HasValue && mov.TotalRecibido.Value > 0)
            {
                mov.TotalCambio = mov.TotalCambio ?? 0;
                ticket.AddRowLine("RECIBIDO&&" + mov.TotalRecibido.Value, TypeRow.Total);
                ticket.AddRowLine("CAMBIO&&" + mov.TotalCambio.Value, TypeRow.Total);
            }            
            ticket.AddRowLine("Facturación por computador", TypeRow.Footer);
            ticket.AddRowLine("Sistema Hottely", TypeRow.Footer);
            ticket.AddRowLine("Oscar Moncada NIT: 80479670-4", TypeRow.Footer);
            ticket.AddRowLine("Gracias por preferirnos!", TypeRow.Footer);

            //Iniciar la impresion del ticket en la impresora indicada del área física
            ticket.PrintTicket(nombreImpresora);

            return StatusCode(StatusCodes.Status200OK, response);
        }


        [HttpGet]
        public async Task<IActionResult> HistorialMovimiento(string numeroMovimiento, string buscarPorTipo, string fechaInicio,string fechaFin)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            var listaventa = await _ventaService.Historial(idEstablishment,numeroMovimiento, buscarPorTipo, fechaInicio, fechaFin);
            List<MovimientoDTO> lista = _mapper.Map<List<MovimientoDTO>>(listaventa);
            return StatusCode(StatusCodes.Status200OK, lista);
        }


        public IActionResult MostrarPDFMovimiento(string numeroMovimiento)
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

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            List<MedioPagoDTO> lista = _mapper.Map<List<MedioPagoDTO>>(await _medioPagoService.Listar());
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        [HttpGet]
        public async Task<IActionResult> GetMovimientosBooking(int idMvtoRel)
        {
            var listaventa = await _ventaService.GetMovimientosBooking(idMvtoRel);
            List<MovimientoDTO> lista = _mapper.Map<List<MovimientoDTO>>(listaventa);
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        #region Metodos de CAJA               

        [HttpPost]
        public async Task<IActionResult> OpenCash([FromBody] Caja caja)
        {
            GenericResponse<Caja> response = new GenericResponse<Caja>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                      .Where(c => c.Type == ClaimTypes.NameIdentifier)
                      .Select(c => c.Value).SingleOrDefault();

                Caja nuevaCaja = caja != null ? caja : new Caja();
                nuevaCaja.Estado = true;
                nuevaCaja.FechaInicio = DateTime.Now;
                nuevaCaja.SaldoFinal = 0;
                nuevaCaja.SaldoReal = 0;
                nuevaCaja.IdUsuario = int.Parse(idUsuario);
                nuevaCaja.IdEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                var resCaja = await _cajaService.Crear(nuevaCaja);
                if (resCaja != null)
                {
                    DetalleCaja det = new DetalleCaja();
                    det.IdCaja = resCaja.IdCaja;
                    det.Valor = caja.SaldoInicial;
                    det.IdMedioPago = (int)Enums.PayMethod.EntradaEfectivo;
                    var resDet = await _detalleCajaService.Crear(det);
                    if (resDet != null)
                    {
                        //Actualiza la variable de sesiòn de control de caja
                        var identity = new ClaimsIdentity(claimUser.Identity);
                        identity.RemoveClaim(identity.FindFirst("ToOpenCash"));
                        identity.AddClaim(new Claim("ToOpenCash", false.ToString()));
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(identity.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties() { AllowRefresh = true, IsPersistent = true };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                    }
                    response.Estado = true;
                    response.Objeto = resCaja;
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPost]
        public async Task<IActionResult> GetTotalCash()
        {
            GenericResponse<Caja> response = new GenericResponse<Caja>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                      .Where(c => c.Type == ClaimTypes.NameIdentifier)
                      .Select(c => c.Value).SingleOrDefault();

                Caja cajaUser = _cajaService.ObtenerCajaUsuario(int.Parse(idUsuario));
                if (cajaUser != null)
                {
                    if (!cajaUser.Estado)
                    {
                        response.Estado = false;
                        response.Mensaje = "La caja encontrada ya fue cerrada";
                        return StatusCode(StatusCodes.Status200OK, response);
                    }

                    List<DetalleCaja> dets = await _detalleCajaService.ObtenerDetallesCaja(cajaUser.IdCaja);
                    decimal saldoFinal = dets.FindAll(d => d.IdMedioPago == (int)Enums.PayMethod.Efectivo || d.IdMedioPago == (int)Enums.PayMethod.EntradaEfectivo).Sum(x => x.Valor);
                    saldoFinal -= dets.FindAll(d => d.IdMedioPago == (int)Enums.PayMethod.SalidaEfectivo).Sum(x => x.Valor);
                    response.Estado = true;
                    response.Mensaje = saldoFinal.ToString("N0");
                    response.Objeto = new Caja() { SaldoFinal = saldoFinal };
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPost]
        public async Task<IActionResult> CloseCash([FromBody] Caja close)
        {
            GenericResponse<Caja> response = new GenericResponse<Caja>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                      .Where(c => c.Type == ClaimTypes.NameIdentifier)
                      .Select(c => c.Value).SingleOrDefault();

                //Crear Detalle Caja
                Caja cajaUser = _cajaService.ObtenerCajaUsuario(int.Parse(idUsuario));
                if (cajaUser != null)
                {
                    if (!cajaUser.Estado)
                    {
                        response.Estado = false;
                        response.Mensaje = "La caja encontrada ya fue cerrada";
                        return StatusCode(StatusCodes.Status200OK, response);
                    }

                    List<DetalleCaja> dets = await _detalleCajaService.ObtenerDetallesCaja(cajaUser.IdCaja);
                    cajaUser.SaldoFinal = dets.FindAll(d=>d.IdMedioPago == (int)Enums.PayMethod.Efectivo || d.IdMedioPago == (int)Enums.PayMethod.EntradaEfectivo).Sum(x=>x.Valor);
                    cajaUser.SaldoFinal -= dets.FindAll(d => d.IdMedioPago == (int)Enums.PayMethod.SalidaEfectivo).Sum(x => x.Valor);
                    cajaUser.Estado = false;
                    cajaUser.FechaCierre = DateTime.Now;
                    cajaUser.SaldoReal = close?.SaldoReal;
                    cajaUser.Observacion = close?.Observacion;
                    cajaUser = await _cajaService.Editar(cajaUser);
                    if (cajaUser != null)
                    {
                        //Actualiza la variable de sesiòn de control de caja
                        var identity = new ClaimsIdentity(claimUser.Identity);
                        identity.RemoveClaim(identity.FindFirst("ToOpenCash"));
                        identity.AddClaim(new Claim("ToOpenCash", true.ToString()));
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(identity.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties() { AllowRefresh = true, IsPersistent = true };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        response.Estado = true;
                        response.Objeto = cajaUser;
                    }
                    else
                    {
                        response.Estado = false;
                        response.Mensaje = "Hubo problemas al cerrar la caja, intente más tarde";
                    }
                }
                else
                {
                    response.Estado = false;
                    response.Mensaje ="Hubo problemas al cerrar la caja, intente más tarde";
                }
               
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpGet]
        public async Task<IActionResult> GetAreaFisicaList()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            List<AreaFisica> lista = _mapper.Map<List<AreaFisica>>(await _areaFisicaService.Listar(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, lista);
        }


        #endregion

    }
}

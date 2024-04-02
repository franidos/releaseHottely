using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEstablishmentService _negocioService;
        private readonly IImageService _imageService;
        private readonly IMovimientoService _ventaService;
        private readonly IBookingService _bookingService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;

        public PlantillaController(IMapper mapper, IEstablishmentService negocioService, IMovimientoService ventaService, IBookingService bookingService, IGuestService guestService, IRoomService roomService, IImageService imageService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            _ventaService = ventaService;
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
            _imageService = imageService;
        }

        public async Task<IActionResult> PDFMovimiento(string numeroMovimiento, int idCompany)
        {
            try
            {

                MovimientoDTO dtoMovimiento = _mapper.Map<MovimientoDTO>(await _ventaService.Detalle(numeroMovimiento, idCompany));
                EstablishmentDTO dtoNegocio = _mapper.Map<EstablishmentDTO>(await _negocioService.getEstablishmentById(idCompany));
                PDFMovimientoDTO modelo = new PDFMovimientoDTO();
                modelo.Movimiento = dtoMovimiento;
                modelo.Negocio = dtoNegocio;

                return View(modelo);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;

            return View();
        }

        public async Task<IActionResult> BookingConfirm(int idBook, int idCompany, string tratamiento)
        {
            ConfirmBookingDTO modelo = new ConfirmBookingDTO();
            List<BookDTO> books = _mapper.Map<List<BookDTO>>(await _bookingService.GetBookById(idBook));
            modelo.Book = books.FirstOrDefault();
            modelo.Negocio = _mapper.Map<EstablishmentDTO>(await _negocioService.getEstablishmentById(idCompany));
            if (modelo.Book?.DetailBook != null && modelo.Book.DetailBook.Any())
            {
                modelo.GuestMain = _mapper.Map<GuestDTO>(await _guestService.getGuestById(modelo.Book.DetailBook.First().IdGuest));
                modelo.Room = _mapper.Map<RoomDTO>(await _roomService.GetRoomById(modelo.Book.DetailBook.First().IdRoom));
                var images = await _imageService.ListImagesByEstablishment(idCompany);
                modelo.UrlImageMainEstablishment = images?.Count > 0 ? images.First().UrlImage : modelo.Negocio?.UrlImage;
                modelo.CantNoches = (int)(modelo.Book.CheckOut - modelo.Book.CheckIn).TotalDays;
                modelo.CantAdultos = modelo.Book?.Adults != null ? modelo.Book.Adults.Split(',').Count() : 0;
                modelo.CantNiños = !string.IsNullOrEmpty(modelo.Book?.AgeChildren) ? modelo.Book.AgeChildren.Split(',').Count() : 0;
                modelo.EstatusBookDesc = modelo.Book?.IdBookStatusNavigation != null ? modelo.Book.IdBookStatusNavigation.StatusName.ToUpper() : "NO REGISTRA ESTADO";
            }
            modelo.Tratamiento = tratamiento;

            return View(modelo);
        }
    }
}

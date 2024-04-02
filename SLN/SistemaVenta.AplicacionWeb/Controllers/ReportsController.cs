using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookingService _bookingService;

        public ReportsController(IMapper mapper, IBookingService bookingService)
        {
            _mapper = mapper;
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> BookingReport(string fechaInicio, string fechaFin)
        {
            try
            {

                ClaimsPrincipal claimUser = HttpContext.User;
                var idCompany = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                List<BookingDetailResult> lista = await _bookingService.Reporte(fechaInicio, fechaFin, idCompany);
                return StatusCode(StatusCodes.Status200OK, new { data = lista });

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}

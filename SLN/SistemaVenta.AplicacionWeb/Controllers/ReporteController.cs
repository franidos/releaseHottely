using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoService _ventaService;

        public ReporteController(IMapper mapper, IMovimientoService ventaService)
        {
            _mapper = mapper;
            _ventaService = ventaService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ReporteMovimiento(string fechaInicio , string fechaFin)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            var lista = await _ventaService.Reporte(idEstablishment,fechaInicio, fechaFin);
            List<ReporteMovimientoDTO> listaReportes = _mapper.Map<List<ReporteMovimientoDTO>>(lista);
            return StatusCode(StatusCodes.Status200OK,new { data = listaReportes } );
        }
    }
}

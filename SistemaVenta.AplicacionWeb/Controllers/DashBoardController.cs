using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashBoardService;

        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumenHabitaciones()
        {
            GenericResponse<DashBoardDTO> response = new GenericResponse<DashBoardDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstabl = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                DashBoardDTO dashBoard = new DashBoardDTO();
                List<MovimientosSemanaDTO> listaMovimientosSemana = new List<MovimientosSemanaDTO>();
                List<ProductoSemanaDTO> listProductoSemana = new List<ProductoSemanaDTO>();

                foreach (KeyValuePair<string, int> item in await _dashBoardService.MovimientosUltimaSemana(idEstabl))
                {
                    listaMovimientosSemana.Add(new MovimientosSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                foreach (KeyValuePair<string, int> item in await _dashBoardService.HabitacionesTopUltimaSemana(idEstabl))
                {
                    listProductoSemana.Add(new ProductoSemanaDTO()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                dashBoard.TotalDisponibles = await _dashBoardService.TotalHabitacionesDisponibles(idEstabl);
                dashBoard.TotalOcupadas = await _dashBoardService.TotalHabitacionesOcupadas(idEstabl);
                dashBoard.TotalReservadas = await _dashBoardService.TotalHabitacionesReservadas(idEstabl);
                dashBoard.TotalFueraDeServicio = await _dashBoardService.TotalHabitacionesFueraDeServicio(idEstabl);
                dashBoard.MovimientosHabitacionesUltimaSemana = listaMovimientosSemana;
                dashBoard.HabitacionesTopUltimaSemana = listProductoSemana;

                response.Estado = true;
                response.Objeto = dashBoard;

            }
            catch (Exception ex)
            {

                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<DashBoardDTO> response = new GenericResponse<DashBoardDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstabl = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                DashBoardDTO dashBoard = new DashBoardDTO();
                List<MovimientosSemanaDTO> listaMovimientosSemana = new List<MovimientosSemanaDTO>();
                List<ProductoSemanaDTO> listProductoSemana = new List<ProductoSemanaDTO>();

                foreach(KeyValuePair<string,int> item in await _dashBoardService.MovimientosUltimaSemana(idEstabl))
                {
                    listaMovimientosSemana.Add(new MovimientosSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                foreach (KeyValuePair<string, int> item in await _dashBoardService.HabitacionesTopUltimaSemana(idEstabl))
                {
                    listProductoSemana.Add(new ProductoSemanaDTO()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                dashBoard.TotalVentas = await _dashBoardService.TotalVentasCajaxFecha(idEstabl, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                dashBoard.TotalIngresos = await _dashBoardService.TotalIngresoxFecha(idEstabl,new DateTime(DateTime.Now.Year,DateTime.Now.Month,1));
                dashBoard.TotalProductos = await _dashBoardService.TotalProductos(idEstabl);
                dashBoard.TotalCategorias = await _dashBoardService.TotalCategoriasProductos(idEstabl);
                dashBoard.MovimientosUltimaSemana = listaMovimientosSemana;
                dashBoard.ProductosTopUltimaSemana = listProductoSemana;

                response.Estado = true;
                response.Objeto = dashBoard;

            }
            catch (Exception ex)
            {

                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK,response);
        }
    }
}

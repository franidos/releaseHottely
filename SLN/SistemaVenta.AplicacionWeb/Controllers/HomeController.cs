using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPlanService _planService;

        public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioService, IMapper mapper, ISubscriptionService subscriptionService, IPlanService planService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
            _mapper = mapper;
            _subscriptionService = subscriptionService;
            _planService = planService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Perfil()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<UsuarioDTO> response = new GenericResponse<UsuarioDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario user_obtenido = await _usuarioService.ObtenerPorId(int.Parse(idUsuario));

                UsuarioDTO usuario = _mapper.Map<UsuarioDTO>(user_obtenido);

                usuario.AbrirCaja = Convert.ToBoolean(((ClaimsIdentity)claimUser.Identity).FindFirst("ToOpenCash").Value);

                response.Estado = true;
                response.Objeto = usuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerServicios()
        {
            GenericResponse<SubscriptionDTO> response = new GenericResponse<SubscriptionDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                var idSubscription = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("Subscription").Value);

                Subscription subscription_obtenida = await _subscriptionService.GetSubscriptionById(idSubscription);

                Plan plan_obtenido = await _planService.GetPlanById(subscription_obtenida.IdPlan);

                SubscriptionDTO subscription = _mapper.Map<SubscriptionDTO>(subscription_obtenida);
                subscription.PlanName = plan_obtenido.PlanName;
                subscription.PlanDescription = plan_obtenido.PlanDescription;

                response.Estado = true;
                response.Objeto = subscription;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] UsuarioDTO modelo)
        {
            GenericResponse<UsuarioDTO> response = new GenericResponse<UsuarioDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario usuario = _mapper.Map<Usuario>(modelo);
                usuario.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usuarioService.GuardarPerfil(usuario);

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarClaveDTO modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                bool resultado = await _usuarioService.CambiarClave(int.Parse(idUsuario), modelo.ClaveActual, modelo.ClaveNueva);

                response.Estado = resultado;
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
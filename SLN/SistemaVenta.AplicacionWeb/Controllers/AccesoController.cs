using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;

        private readonly ISubscriptionService _subscriptionService;
        private readonly IEstablishmentService _establishmentService;
        private readonly ICajaService _cajaService;
        public AccesoController(IUsuarioService usuarioServicio, ICajaService cajaService, ISubscriptionService subscriptionService, IEstablishmentService establishmentService)
        {
            _usuarioServicio = usuarioServicio;
            _subscriptionService = subscriptionService;
            _establishmentService = establishmentService;
            _cajaService = cajaService;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDTO modelo)
        {
            Debug.WriteLine("Inicio del método Login");

            Usuario usuario_encontrado = await _usuarioServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            
            Debug.WriteLine("Usuario encontrado: " + usuario_encontrado?.Nombre);

            // Company compania = await _companyService.Obtener();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "Usuario o clave incorrecta, no se encontraron coincidencia";
                return View();
            }
            else
            {
                bool iniciarCaja = _cajaService.CajaCierre(usuario_encontrado.IdUsuario);

                ViewData["Mensaje"] = null;

                Establishment establishment_found = await _establishmentService.getEstablishmentById(usuario_encontrado.IdEstablishment);
                Subscription suscripcion_encontrada = await _subscriptionService.GetSubscriptionByIdCompany(usuario_encontrado.IdEstablishment);


                //Creando lista de reclamación 
                List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_encontrado.UrlFoto),
                new Claim("urlLogo", establishment_found.UrlImage),
                new Claim("IdCompany", establishment_found.IdEstablishment.ToString()),
                new Claim("Subscription", suscripcion_encontrada.IdSubscription.ToString()),
                new Claim("SubscriptionStatus", suscripcion_encontrada.SubscriptionStatus.ToString()),
                new Claim("Plain", suscripcion_encontrada.IdPlan.ToString()),
                new Claim("ExpiryDateSubscription", suscripcion_encontrada.ExpiryDate.ToString()),
                new Claim("ToOpenCash", iniciarCaja.ToString())
            };

                //registrando los claims a la autenticacion por coockies
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //configurar propiedades a la auntenticacion
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true, //refescar la auntenticacion
                    IsPersistent = modelo.MantenerSesion,  //persistir la auntenticacion
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                    );

                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult RestablecerContraseña()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContraseña(UsuarioLoginDTO modelo)
        {
            try
            {
                string urlPLantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _usuarioServicio.RestablecerClave(modelo.Correo, urlPLantillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue reestablecida, revise su correo";
                    ViewData["MensajeError"] = "";
                }
                else
                {
                    ViewData["Mensaje"] = "";
                    ViewData["MensajeError"] = "Tenemos problemas, inténtelo de nuevo más tarde";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "";
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }
}

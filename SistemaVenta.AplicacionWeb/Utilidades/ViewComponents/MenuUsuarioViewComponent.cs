using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Contexto del usuario logeado
            ClaimsPrincipal claimUser = HttpContext.User;

            string nombreUsuario = "";
            string urlFotoUsuario = "";
            List<Establishment> establishmentList = new List<Establishment>();
            if (claimUser.Identity.IsAuthenticated)
            {
                nombreUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;

                //// Obtén el claim "EstablishmentList" de la identidad del usuario actual
                //var establishmentListClaim = ((ClaimsIdentity)claimUser.Identity).FindFirst("EstablishmentList");

                //if (establishmentListClaim != null)
                //{
                //    // Obtén el valor de la cadena JSON del claim
                //    var establishmentListJson = establishmentListClaim.Value;

                //    // Convierte la cadena JSON en una lista de objetos
                //    establishmentList = JsonConvert.DeserializeObject<List<Establishment>>(establishmentListJson);
                //}
            }

            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["urlFotoUsuario"] = urlFotoUsuario;
            //ViewData["establishmentList"] = establishmentList;

            return View();
        }

    }
}

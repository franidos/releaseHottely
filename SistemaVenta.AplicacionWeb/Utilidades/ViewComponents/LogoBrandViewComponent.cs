using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class LogoBrandViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //contexto del usuario logeado
            ClaimsPrincipal claimUser = HttpContext.User;

            string urlLogoBrand = "";

            if (claimUser.Identity.IsAuthenticated)
            {

                urlLogoBrand = "/img/Hottely2.png";
            }

            ViewData["urlLogoBrand"] = urlLogoBrand;

            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //contexto del usuario logeado
            ClaimsPrincipal claimUser = HttpContext.User;

            string ToOpenCash = "";

            if (claimUser.Identity.IsAuthenticated)
            {

                ToOpenCash = ((ClaimsIdentity)claimUser.Identity).FindFirst("ToOpenCash").Value;
            }

            ViewData["ToOpenCash"] = ToOpenCash;
            
            List<string> menus = new List<string>();

            if (ToOpenCash?.ToLower() == "true")
            {
                menus.Add("Abrir Caja");
            }
            else
            {
                menus.Add("Cerrar Caja");
            }


            return View(menus);
        }
    }
}

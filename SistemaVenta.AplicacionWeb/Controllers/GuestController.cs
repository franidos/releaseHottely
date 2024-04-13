using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class GuestController : Controller
    {
        private readonly IGuestService _guestService;
        private readonly IMapper _mapper;

        public GuestController(IGuestService guestService, IMapper mapper)
        {
            _guestService = guestService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            List<GuestDTO> guestDTOLista = _mapper.Map<List<GuestDTO>>(await _guestService.Listar(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = guestDTOLista });  
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] GuestDTO modelo)
        {
            GenericResponse<GuestDTO> response = new GenericResponse<GuestDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                modelo.IdEstablishment = idEstablishment;
                Guest guest_creada = await _guestService.Crear(_mapper.Map<Guest>(modelo));
                modelo = _mapper.Map<GuestDTO>(guest_creada);

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] GuestDTO modelo)
        {
            GenericResponse<GuestDTO> response = new GenericResponse<GuestDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                modelo.IdEstablishment = idEstablishment;
                Guest guest_editada = await _guestService.Editar(_mapper.Map<Guest>(modelo));
                modelo = _mapper.Map<GuestDTO>(guest_editada);

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idGuest)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _guestService.Eliminar(idGuest);
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

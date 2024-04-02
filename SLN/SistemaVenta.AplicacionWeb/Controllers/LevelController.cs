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
    public class LevelController : Controller
    {
        private readonly ILevelService _levelService;
        private readonly IMapper _mapper;

        public LevelController(ILevelService levelService, IMapper mapper)
        {
            _levelService = levelService;
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

            List<LevelDTO> levelDTOLista = _mapper.Map<List<LevelDTO>>(await _levelService.Lista(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = levelDTOLista }); // El DataTable funciona recibiendo un objeto 'data' . 
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] LevelDTO modelo)
        {
            GenericResponse<LevelDTO> response = new GenericResponse<LevelDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                modelo.IdEstablishment = idEstablishment;
                Level level_creada = await _levelService.Crear(_mapper.Map<Level>(modelo));
                modelo = _mapper.Map<LevelDTO>(level_creada);

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
        public async Task<IActionResult> Editar([FromBody] LevelDTO modelo)
        {
            GenericResponse<LevelDTO> response = new GenericResponse<LevelDTO>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                modelo.IdEstablishment = idEstablishment;
                Level level_editada = await _levelService.Editar(_mapper.Map<Level>(modelo));
                modelo = _mapper.Map<LevelDTO>(level_editada);

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
        public async Task<IActionResult> Eliminar(int idLevel)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _levelService.Eliminar(idLevel);
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

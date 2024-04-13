using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class PriceManagerController : Controller
    {
        private readonly IPriceService _priceManagerService;
        private readonly IMapper _mapper;

        public PriceManagerController(IPriceService priceManagerService, IMapper mapper)
        {
            _priceManagerService = priceManagerService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListRegular()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();
            List<RoomPriceDTO> roomPriceDTO = _mapper.Map<List<RoomPriceDTO>>(await _priceManagerService.Lista(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = roomPriceDTO });
        }
        [HttpGet]
        public async Task<IActionResult> ListSpecial()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();
            List<SeasonDTO> SeasonDTOLista = _mapper.Map<List<SeasonDTO>>(await _priceManagerService.ListSeasons(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = SeasonDTOLista });

        }
        [HttpGet]
        public async Task<IActionResult> ListHolidays()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();
            List<HolidayDTO> HolidayDTOLista = _mapper.Map<List<HolidayDTO>>(await _priceManagerService.ListHolidays(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = HolidayDTOLista }); 

        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RoomPriceDTO modelo)
        {
            GenericResponse<RoomPriceDTO> response = new GenericResponse<RoomPriceDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                RoomPrice priceManager_creada = await _priceManagerService.Crear(_mapper.Map<RoomPrice>(modelo));
                modelo = _mapper.Map<RoomPriceDTO>(priceManager_creada);

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
        public async Task<IActionResult> Editar([FromBody] RoomPriceDTO modelo)
        {
            GenericResponse<RoomPriceDTO> response = new GenericResponse<RoomPriceDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                RoomPrice priceManager_editada = await _priceManagerService.Editar(_mapper.Map<RoomPrice>(modelo));
                modelo = _mapper.Map<RoomPriceDTO>(priceManager_editada);

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
        public async Task<IActionResult> Eliminar(int idPriceManager)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _priceManagerService.Eliminar(idPriceManager);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        public async Task<IActionResult> CreateSeason([FromBody] SeasonDTO modelo)
        {
            GenericResponse<SeasonDTO> response = new GenericResponse<SeasonDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                Season season_created = await _priceManagerService.CreateSeason(_mapper.Map<Season>(modelo));
                modelo = _mapper.Map<SeasonDTO>(season_created);

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
        public async Task<IActionResult> EditSeason([FromBody] SeasonDTO modelo)
        {
            GenericResponse<SeasonDTO> response = new GenericResponse<SeasonDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                Season priceManager_editada = await _priceManagerService.EditSeason(_mapper.Map<Season>(modelo));
                modelo = _mapper.Map<SeasonDTO>(priceManager_editada);

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
        public async Task<IActionResult> DeleteSeason(int idSeason)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _priceManagerService.DeleteSeason(idSeason);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        public async Task<IActionResult> CreateHoliday([FromBody] HolidayDTO modelo)
        {
            GenericResponse<HolidayDTO> response = new GenericResponse<HolidayDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                Holiday season_created = await _priceManagerService.CreateHoliday(_mapper.Map<Holiday>(modelo));
                modelo = _mapper.Map<HolidayDTO>(season_created);

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
        public async Task<IActionResult> EditHoliday([FromBody] HolidayDTO modelo)
        {
            GenericResponse<HolidayDTO> response = new GenericResponse<HolidayDTO>();
            try
            {
                modelo.IdEstablishment = GetEstablishmentIdFromClaims();
                modelo.User = GetUserFromClaims();
                Holiday priceManager_editada = await _priceManagerService.EditHoliday(_mapper.Map<Holiday>(modelo));
                modelo = _mapper.Map<HolidayDTO>(priceManager_editada);

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
        public async Task<IActionResult> DeleteHoliday(int idHoliday)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _priceManagerService.DeleteHoliday(idHoliday);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }
        private int GetEstablishmentIdFromClaims()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            int idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            return idEstablishment;
        }

        private string GetUserFromClaims()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string idUsuario = claimUser.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();

            return idUsuario;
        }
    }
}

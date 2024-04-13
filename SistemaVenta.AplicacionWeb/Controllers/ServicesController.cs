using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IServicesService _serviceService;

        public ServicesController(IMapper mapper, IServicesService serviceService)
        {
            _mapper = mapper;
            _serviceService = serviceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            int idEstablishment = GetEstablishmentIdFromClaims();
            List<ServiceDTO> serviceDTOLista = _mapper.Map<List<ServiceDTO>>(await _serviceService.Listar(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = serviceDTOLista }); // El DataTable funciona recibiendo un objeto 'data' . 
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen,[FromForm]string modelo)
        {
            GenericResponse<ServiceDTO> response = new GenericResponse<ServiceDTO>();
            try
            {
                ServiceDTO serviceDto = JsonConvert.DeserializeObject<ServiceDTO>(modelo);
                string nombreImagen = "";
                Stream streamImagen = null;

                if (imagen != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombre_en_codigo,extension);
                    streamImagen = imagen.OpenReadStream();
                }
                serviceDto.IdEstablishment = GetEstablishmentIdFromClaims();
                Service service_creado = await _serviceService.Crear(_mapper.Map<Service>(serviceDto), streamImagen, nombreImagen);

                serviceDto = _mapper.Map<ServiceDTO>(service_creado);
                response.Estado = true;
                response.Objeto = serviceDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<ServiceDTO> response = new GenericResponse<ServiceDTO>();
            try
            {
                ServiceDTO serviceDto = JsonConvert.DeserializeObject<ServiceDTO>(modelo);
                Stream streamImagen = null;
                serviceDto.IdEstablishment = GetEstablishmentIdFromClaims();
                if (imagen != null)
                {
                    streamImagen = imagen.OpenReadStream();
                }

                Service service_editado = await _serviceService.Editar(_mapper.Map<Service>(serviceDto), streamImagen);

                serviceDto = _mapper.Map<ServiceDTO>(service_editado);
                response.Estado = true;
                response.Objeto = serviceDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idService)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _serviceService.Eliminar(idService);
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
    }
}

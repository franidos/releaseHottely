using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using nextadvisordotnet.AppWeb.Models;
using SistemaVenta.AplicacionWeb.Models;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class EstablishmentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEstablishmentService _establishmentService;
        //private readonly IParamPlanService _paramPlanService;
        private readonly IImageService _imageService;

        public EstablishmentController(IMapper mapper, IEstablishmentService establishmentService, IImageService imageService)
        {
            _mapper = mapper;
            _establishmentService = establishmentService;
           // _paramPlanService = paramPlanService;       
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<EstablishmentDTO> response = new GenericResponse<EstablishmentDTO>();

            try
            {

                ClaimsPrincipal claimUser = HttpContext.User;
                var idCompany = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                EstablishmentDTO establishmentDTO = _mapper.Map<EstablishmentDTO>(await _establishmentService.getEstablishmentById(idCompany));

                List<ImagesEstablishmentDTO> imagesEstablishmentDTOLista = _mapper.Map<List<ImagesEstablishmentDTO>>(await _imageService.ListImagesByEstablishment(idCompany));

                establishmentDTO.ImagesEstablishment = imagesEstablishmentDTOLista;

                //return StatusCode(StatusCodes.Status200OK, new { data = imagesRoomDTOLista });


                response.Estado = true;
                response.Objeto = establishmentDTO;
            }
            catch (Exception ex)
            {

                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> ListImagesEstablishmentById(string establishmentId)
        {
            int idEstablishment = int.Parse(establishmentId);

            List<ImagesEstablishmentDTO> imagesRoomDTOLista = _mapper.Map<List<ImagesEstablishmentDTO>>(await _imageService.ListImagesByEstablishment(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = imagesRoomDTOLista });

        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios(
            [FromForm] List<IFormFile> newImagenes,
            [FromForm] List<string> oldImagesUrl,
            [FromForm] List<string> imageIds,
            IFormFile logo, [FromForm]
            string modelo)
        {
            GenericResponse<EstablishmentDTO> response = new GenericResponse<EstablishmentDTO>();

            try
            {
                EstablishmentDTO companyDTO = JsonConvert.DeserializeObject<EstablishmentDTO>(modelo);
                ClaimsPrincipal claimUser = HttpContext.User;

                var idEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);

                companyDTO.IdEstablishment = idEstablishment;

                string nombreLogo = "";
                Stream streamLogo = null;

                if (logo != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombre_en_codigo, extension);
                    streamLogo = logo.OpenReadStream();
                }

                Establishment establishment_editado = await _establishmentService.GuardarCambios(_mapper.Map<Establishment>(companyDTO), streamLogo, nombreLogo);

                await _imageService.HandleImages(newImagenes, oldImagesUrl, imageIds, establishment_editado);

                response.Estado = true;
                response.Objeto = companyDTO;
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

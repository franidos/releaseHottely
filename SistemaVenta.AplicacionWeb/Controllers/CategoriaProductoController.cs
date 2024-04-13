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
    public class CategoriaProductoController : Controller
    {
        private readonly ICategoriaProductoService _categoriaProductoService;
        private readonly IMapper _mapper;

        public CategoriaProductoController(ICategoriaProductoService categoriaProductoService, IMapper mapper)
        {
            _categoriaProductoService = categoriaProductoService;
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
            List<CategoriaProductoDTO> categoriaProductoDTOLista = _mapper.Map<List<CategoriaProductoDTO>>(await _categoriaProductoService.Lista(idEstablishment));
            return StatusCode(StatusCodes.Status200OK, new { data = categoriaProductoDTOLista }); // El DataTable funciona recibiendo un objeto 'data' . 
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CategoriaProductoDTO modelo)
        {
            GenericResponse<CategoriaProductoDTO> response = new GenericResponse<CategoriaProductoDTO>();
            ClaimsPrincipal claimUser = HttpContext.User;
            try
            {
                modelo.IdEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
                CategoriaProducto categoriaProducto_creada = await _categoriaProductoService.Crear(_mapper.Map<CategoriaProducto>(modelo));
                modelo = _mapper.Map<CategoriaProductoDTO>(categoriaProducto_creada);

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
        public async Task<IActionResult> Editar([FromBody] CategoriaProductoDTO modelo)
        {
            GenericResponse<CategoriaProductoDTO> response = new GenericResponse<CategoriaProductoDTO>();
            ClaimsPrincipal claimUser = HttpContext.User;
            modelo.IdEstablishment = int.Parse(((ClaimsIdentity)claimUser.Identity).FindFirst("IdCompany").Value);
            try
            {
                CategoriaProducto categoriaProducto_editada = await _categoriaProductoService.Editar(_mapper.Map<CategoriaProducto>(modelo));
                modelo = _mapper.Map<CategoriaProductoDTO>(categoriaProducto_editada);

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
        public async Task<IActionResult> Eliminar(int idCategoriaProducto)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _categoriaProductoService.Eliminar(idCategoriaProducto);
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

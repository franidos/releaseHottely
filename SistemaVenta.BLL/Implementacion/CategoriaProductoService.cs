using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class CategoriaProductoService : ICategoriaProductoService
    {
        private readonly IGenericRepository<CategoriaProducto> _repositorio;

        public CategoriaProductoService(IGenericRepository<CategoriaProducto> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<CategoriaProducto>> Lista(int idEstablishment)
        {
            IQueryable<CategoriaProducto> query = await _repositorio.Consultar(x => x.IdEstablishment == idEstablishment);
            return query.ToList();
        }
        public async Task<CategoriaProducto> Crear(CategoriaProducto entidad)
        {
            try
            {
                CategoriaProducto categoriaProducto_creada = await _repositorio.Crear(entidad);
                if(categoriaProducto_creada.IdCategoriaProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la categoriaProducto");
                }
                return categoriaProducto_creada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CategoriaProducto> Editar(CategoriaProducto entidad)
        {
            try
            {
                CategoriaProducto categoriaProducto_encontrada = await _repositorio.Obtener(c => c.IdCategoriaProducto == entidad.IdCategoriaProducto && c.IdEstablishment == entidad.IdEstablishment);
                categoriaProducto_encontrada.Descripcion = entidad.Descripcion;
                categoriaProducto_encontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(categoriaProducto_encontrada);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la categoriaProducto");
                }

                return categoriaProducto_encontrada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoriaProducto)
        {
            try
            {
                CategoriaProducto categoriaProducto_encontrada = await _repositorio.Obtener(c => c.IdCategoriaProducto == idCategoriaProducto);
                
                if (categoriaProducto_encontrada == null)
                {
                    throw new TaskCanceledException("La categoriaProducto no existe");
                }
                bool respuesta = await _repositorio.Eliminar(categoriaProducto_encontrada);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

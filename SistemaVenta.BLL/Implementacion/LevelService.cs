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
    public class LevelService : ILevelService
    {
        private readonly IGenericRepository<Level> _repositorio;

        public LevelService(IGenericRepository<Level> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Level>> Lista(int idEstabl)
        {
            IQueryable<Level> query = await _repositorio.Consultar(x=>x.IdEstablishment == idEstabl);
            return query.ToList();
        }
        public async Task<Level> Crear(Level entidad)
        {
            try
            {
                Level level_creada = await _repositorio.Crear(entidad);
                if(level_creada.IdLevel == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el Nivel");
                }
                return level_creada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Level> Editar(Level entidad)
        {
            try
            {
                Level level_encontrada = await _repositorio.Obtener(c => c.IdLevel == entidad.IdLevel);
                level_encontrada.LevelName = entidad.LevelName;
                level_encontrada.IsActive = entidad.IsActive;

                bool respuesta = await _repositorio.Editar(level_encontrada);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Nivel");
                }

                return level_encontrada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idLevel)
        {
            try
            {
                Level level_encontrada = await _repositorio.Obtener(c => c.IdLevel == idLevel);
                
                if (level_encontrada == null)
                {
                    throw new TaskCanceledException("El Nivel no existe");
                }
                bool respuesta = await _repositorio.Eliminar(level_encontrada);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

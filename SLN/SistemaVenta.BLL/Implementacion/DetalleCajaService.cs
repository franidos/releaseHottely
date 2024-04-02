using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class DetalleCajaService : IDetalleCajaService
    {
        private readonly IGenericRepository<DetalleCaja> _repositorio;

        public DetalleCajaService(IGenericRepository<DetalleCaja> repositorio, DbventaContext dbContext)
        {
            _repositorio = repositorio;
        }

        public async Task<List<DetalleCaja>> ObtenerDetallesCaja(int idCaja)
        {
            IQueryable<DetalleCaja> query = await _repositorio.Consultar(x=>x.IdCaja == idCaja);
            return query.Include(x=>x.IdMovimientoNavigation).ToList();
        }

        public async Task<DetalleCaja> Crear(DetalleCaja entidad)
        {
            try
            {
                DetalleCaja caja_creado = await _repositorio.Crear(entidad);

                if (caja_creado.IdDetalleCaja == 0)
                {
                    throw new TaskCanceledException("No se pudo crear detalle caja");
                }

                IQueryable<DetalleCaja> query = await _repositorio.Consultar(p => p.IdDetalleCaja == caja_creado.IdDetalleCaja);
                caja_creado = query.First();
                return caja_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}

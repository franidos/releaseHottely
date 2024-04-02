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
    public class CajaService : ICajaService
    {
        private readonly IGenericRepository<Caja> _repositorio;
       // private readonly IFireBaseService _fireBaseService;
        private readonly DbventaContext _dbContext;

        public CajaService(IGenericRepository<Caja> repositorio, DbventaContext dbContext)
        {
            _repositorio = repositorio;
           // _fireBaseService = fireBaseService;
           _dbContext = dbContext;  
        }

        public async Task<List<Caja>> Listar()
        {
            IQueryable<Caja> query = await _repositorio.Consultar();
            return query.ToList(); //.Include(p => p.IdCategoriaNavigation).ToList();
        }
        public  bool CajaCierre(int idUsuario)
        {
            bool seDebeIniciarCaja = false;

            var ultimaCaja = _dbContext.Caja
                .Where(c => c.IdUsuario == idUsuario)
                .OrderByDescending(c => c.FechaInicio)
                .FirstOrDefault();

            seDebeIniciarCaja = ultimaCaja == null || (ultimaCaja.FechaCierre.HasValue && ultimaCaja.SaldoFinal == 0) || ! ultimaCaja.Estado;

            return seDebeIniciarCaja;
        }

        public Caja? ObtenerCajaUsuario(int idUsuario)
        {
            Caja ultimaCaja = _dbContext.Caja
                .Where(c => c.IdUsuario == idUsuario).Include(x=>x.IdAreaNavigation)
                .OrderByDescending(c => c.FechaInicio)
                .FirstOrDefault();

            return ultimaCaja;
        }

        public async Task<Caja> Crear(Caja entidad)
        {

            try
            {

                Caja caja_creado = await _repositorio.Crear(entidad);

                if (caja_creado.IdCaja == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la caja");
                }

                IQueryable<Caja> query = await _repositorio.Consultar(p => p.IdCaja == caja_creado.IdCaja);
                caja_creado = query.First();
                return caja_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Caja> Editar(Caja entidad)
        {
            try
            {
                bool respuesta = await _repositorio.Editar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Huesped");
                }

                IQueryable<Caja> queryCaja = await _repositorio.Consultar(p => p.IdCaja == entidad.IdCaja);
                Caja caja_editado = queryCaja.First();
                return caja_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

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
    public class MedioPagoService : IMedioPagoService
    {
        private readonly IGenericRepository<MedioPago> _repositorio;
       // private readonly IFireBaseService _fireBaseService;
        private readonly DbventaContext _dbContext;

        public MedioPagoService(IGenericRepository<MedioPago> repositorio, DbventaContext dbContext)
        {
            _repositorio = repositorio;
           // _fireBaseService = fireBaseService;
           _dbContext = dbContext;  
        }

        public async Task<List<MedioPago>> Listar()
        {
            IQueryable<MedioPago> query = await _repositorio.Consultar();
            return query.ToList(); //.Include(p => p.IdCategoriaNavigation).ToList();
        }
       
        //public async Task<Caja> Crear(Caja entidad)
        //{

        //    try
        //    {

        //        Caja caja_creado = await _repositorio.Crear(entidad);

        //        if (caja_creado.IdCaja == 0)
        //        {
        //            throw new TaskCanceledException("No se pudo crear la caja");
        //        }

        //        IQueryable<Caja> query = await _repositorio.Consultar(p => p.IdCaja == caja_creado.IdCaja);
        //        caja_creado = query.First();
        //        return caja_creado;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        //public async Task<Caja> Editar(Caja entidad)
        //{
        //    try
        //    {
        //        bool respuesta = await _repositorio.Editar(entidad);
        //        if (!respuesta)
        //        {
        //            throw new TaskCanceledException("No se pudo editar el Huesped");
        //        }

        //        IQueryable<Caja> queryCaja = await _repositorio.Consultar(p => p.IdCaja == entidad.IdCaja);
        //        Caja caja_editado = queryCaja.First();
        //        return caja_editado;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}

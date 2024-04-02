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
    public class AreaFisicaService : IAreaFisicaService
    {
        private readonly IGenericRepository<AreaFisica> _repositorio;
       // private readonly IFireBaseService _fireBaseService;
        private readonly DbventaContext _dbContext;

        public AreaFisicaService(IGenericRepository<AreaFisica> repositorio, DbventaContext dbContext)
        {
            _repositorio = repositorio;
           // _fireBaseService = fireBaseService;
           _dbContext = dbContext;  
        }

        public async Task<List<AreaFisica>> Listar(int idEstablishment)
        {
            IQueryable<AreaFisica> query = await _repositorio.Consultar(x=>x.IdEstablishment == idEstablishment);
            return query.ToList();
        }       
    }
}

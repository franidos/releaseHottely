using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaVenta.BLL.Implementacion
{
    public class ParamPlanService : IParamPlanService
    {
        private readonly IGenericRepository<ParamPlan> _repositorio;

        public ParamPlanService(IGenericRepository<ParamPlan> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<ParamPlan>> GetParamPlanByIdPlan(int idPlan)
        {
            IQueryable<ParamPlan> query = await _repositorio.Consultar(c => c.IdPlan == idPlan);
            return query.ToList();
        }
        public async Task<List<ParamPlan>> Lista()
        {
            IQueryable<ParamPlan> query = await _repositorio.Consultar();
            return query.ToList();

        }
    }
}

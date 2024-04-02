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
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _repositorio;

        public PlanService(IGenericRepository<Plan> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<Plan> GetPlanById(int Id)
        {
            Plan plain_found = await _repositorio.Obtener(c => c.IdPlan == Id);
            return plain_found;
        }
        public async Task<List<Plan>> Lista()
        {
            IQueryable<Plan> query = await _repositorio.Consultar();
            return query.ToList();

        }
    }
}

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
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IGenericRepository<Subscription> _repositorio;

        public SubscriptionService(IGenericRepository<Subscription> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<Subscription> GetSubscriptionByIdCompany(int idCompany)
        {
            Subscription subcription_found = await _repositorio.Obtener(c => c.IdEstablishment == idCompany);
            return subcription_found;
        }
        public async Task<Subscription> GetSubscriptionById(int Id)
        {
            Subscription company_found = await _repositorio.Obtener(c => c.IdEstablishment == Id);
            return company_found;
        }
        public async Task<List<Subscription>> Lista()
        {
            IQueryable<Subscription> query = await _repositorio.Consultar();
            return query.ToList();

        }
    }
}

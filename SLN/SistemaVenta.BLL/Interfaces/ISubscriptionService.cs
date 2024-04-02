using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<Subscription>> Lista();
        Task<Subscription> GetSubscriptionByIdCompany(int idCompany);
        Task<Subscription> GetSubscriptionById(int id);
        //Task<Subscription> Crear(Subscription entidad, Stream imagen = null, string nombreImagen = "" );
        //Task<Subscription> Editar(Subscription entidad, Stream imagen = null);
        //Task<bool> Eliminar(int idSubscription);

    }
}

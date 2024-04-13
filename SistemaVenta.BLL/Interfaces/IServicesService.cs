using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IServicesService
    {

        Task<List<Service>> Lista();
        Task<List<Service>> Listar(int idEstablishment);
        Task<Service> Crear(Service entidad, Stream imagen = null, string nombreImagen = "");
        Task<Service> Editar(Service entidad, Stream imagen = null);
        Task<bool> Eliminar(int idService);
        Task<List<Service>> GetServicesByEstablishment(int idEstablishment);
        Task<List<ServiceInfo>> GetServiceInfo();
        Task<List<ServiceInfoEstablishment>> GetServiceInfoByEstablishment(int idEstablishment);
    }
}

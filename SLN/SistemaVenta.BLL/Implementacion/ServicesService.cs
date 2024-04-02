using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaVenta.BLL.Implementacion
{

    public class ServicesService : IServicesService
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IGenericRepository<ServiceInfoEstablishment> _serviceInfoEstabRepository;
        private readonly IGenericRepository<ServiceInfo> _serviceInfoRepository;
        private readonly IFireBaseService _firebaseSerice;


        public ServicesService(
            IGenericRepository<Service> serviceRepository, 
            IGenericRepository<ServiceInfoEstablishment> serviceInfoEstabRepository, 
            IGenericRepository<ServiceInfo> serviceInfoRepository, 
            IFireBaseService firebaseSerice)
        {
            _serviceRepository = serviceRepository;
            _serviceInfoEstabRepository = serviceInfoEstabRepository;
            _serviceInfoRepository = serviceInfoRepository;
            _firebaseSerice = firebaseSerice;   
        }
        public async Task<List<Service>> Listar(int idEstablishment)
        {
            IQueryable<Service> query = await _serviceRepository.Consultar(x => x.IdEstablishment == idEstablishment);
            return query.ToList();
        }

        public async Task<Service> Crear(Service entidad, Stream imagen = null, string nombreImagen = "")
        {
            Service service_existe = await _serviceRepository.Obtener(p => p.ServiceName == entidad.ServiceName && p.IdEstablishment == entidad.IdEstablishment);
            if (service_existe != null)
            {
                throw new TaskCanceledException("El Nombre del servicio ya existe");
            }

            try
            {
                entidad.ServiceImageName = nombreImagen;
                if (imagen != null)
                {
                    string urlIMagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_service", nombreImagen);
                    entidad.ServiceUrlImage = urlIMagen;
                }

                Service service_creado = await _serviceRepository.Crear(entidad);

                if (service_creado.IdService == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el service");
                }

                IQueryable<Service> query = await _serviceRepository.Consultar(p => p.IdService == service_creado.IdService);
                service_creado = query.First();
                return service_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Service> Editar(Service entidad, Stream imagen = null)
        {
            Service service_existe = await _serviceRepository.Obtener(p => p.ServiceName == entidad.ServiceName && p.IdEstablishment == entidad.IdEstablishment && p.IdService != entidad.IdService);
            if (service_existe != null)
            {
                throw new TaskCanceledException("El nombre del servicio ya existe");
            }
            try
            {
                IQueryable<Service> queryService = await _serviceRepository.Consultar(p => p.IdService == entidad.IdService);
                Service service_para_editar = queryService.First();
                service_para_editar.ServiceName = entidad.ServiceName;
                service_para_editar.ServiceInfo = entidad.ServiceInfo;
                service_para_editar.ServiceInfoQuantity = entidad.ServiceInfoQuantity;
                service_para_editar.ServiceMaximumAmount = entidad.ServiceMaximumAmount;
                service_para_editar.ServiceConditions = entidad.ServiceConditions;
                service_para_editar.ServicePrice = entidad.ServicePrice;
                service_para_editar.IsAdditionalValue = entidad.IsAdditionalValue;
                service_para_editar.ServiceIsActive = entidad.ServiceIsActive;

                if (imagen != null)
                {
                    string urlImagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_service", service_para_editar.ServiceImageName);
                    service_para_editar.ServiceUrlImage = urlImagen;
                }

                bool respuesta = await _serviceRepository.Editar(service_para_editar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el service");
                }

                Service service_editado = queryService.First();
                return service_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idService)
        {
            try
            {
                Service service_encontrado = await _serviceRepository.Obtener(p => p.IdService == idService);
                if (service_encontrado == null)
                {
                    throw new TaskCanceledException("El service no existe");
                }

                string nombreImagen = service_encontrado.ServiceImageName;
                bool respuesta = await _serviceRepository.Eliminar(service_encontrado);
                if (respuesta)
                {
                    await _firebaseSerice.EliminarStorage("carpeta_service", nombreImagen);
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Service>> Lista()
        {
            IQueryable<Service> query = await _serviceRepository.Consultar();
            return query.ToList();
        }
        public async Task<List<Service>> GetServicesByEstablishment(int idEstablishment)
        {
            try
            {
                IQueryable<Service> service = await _serviceRepository.Consultar(c => c.IdEstablishment == idEstablishment);
                return service.ToList();
            }
            catch (Exception)
            {
                return new List<Service>(); // Devuelve una lista vacía en caso de error.
            }
        }

        public async Task<List<ServiceInfo>> GetServiceInfo()
        {
            try
            {
                IQueryable<ServiceInfo> service = await _serviceInfoRepository.Consultar(c => c.IsActive == true);
                return service.ToList();
            }
            catch (Exception)
            {
                return new List<ServiceInfo>();
            }
        }

        public async Task<List<ServiceInfoEstablishment>> GetServiceInfoByEstablishment(int idEstablishment)
        {
            try
            {
                IQueryable<ServiceInfoEstablishment> service = await _serviceInfoEstabRepository.Consultar(c => c.IdEstablishment == idEstablishment);
                return service.ToList();
            }
            catch (Exception)
            {
                return new List<ServiceInfoEstablishment>();
            }
        }
    }
}

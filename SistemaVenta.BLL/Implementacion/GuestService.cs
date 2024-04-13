using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SistemaVenta.BLL.Implementacion
{
    public class GuestService : IGuestService
    {
        private readonly IGenericRepository<Guest> _repositorio;
        private readonly IFireBaseService _firebaseSerice;
        private readonly IUtilidadesService _utilidadesService;
        private readonly IGenericRepository<Configuracion> _repositorioConfiguracion;

        public GuestService(IGenericRepository<Guest> repositorio, IFireBaseService firebaseSerice, IUtilidadesService utilidadesService, IGenericRepository<Configuracion> repositorioConfiguracion)
        {
            _repositorio = repositorio;
            _firebaseSerice = firebaseSerice;
            _utilidadesService = utilidadesService;
            _repositorioConfiguracion = repositorioConfiguracion;
        }

        public async Task<List<Guest>> getGuestsByParam(string busqueda, int idEstabl)
        {
            IQueryable<Configuracion> queryConfiguracion = await _repositorioConfiguracion.Consultar(c => c.Recurso.Equals("ReservaConsulta"));

            Dictionary<string, string> config = queryConfiguracion.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

            IQueryable<Guest> query;

            if (config["IncluyeAcompaniantes"] == "False")
            {
                query = await _repositorio.Consultar(
                p => p.IdEstablishment == idEstabl && p.IsMain == true && string.Concat(p.Document, p.Name, p.LastName).Contains(busqueda));
            }
            else
            {
                query = await _repositorio.Consultar(
                p => p.IdEstablishment == idEstabl && string.Concat(p.Document, p.Name, p.LastName).Contains(busqueda));
            }

            return query.ToList();
        }

        public async Task<Guest> getGuestById(int idGuest)
        {
            Guest query = await _repositorio.Obtener( p => p.IdGuest == idGuest);
            return query;
        }
        public async Task<List<Guest>> Listar(int idEstabl)
        {
            IQueryable<Guest> query = await _repositorio.Consultar(x => x.IdEstablishment == idEstabl);
            return query.ToList();
        }
        public async Task<Guest> getGuestByDocument(string document)
        {
            Guest query = await _repositorio.Obtener(p => p.Document == document.Trim());
            return query;
        }

        public async Task<Guest> Crear(Guest entidad)
        {
            Guest guest_existe = await _repositorio.Obtener(p => p.Document == entidad.Document && p.DocumentType == entidad.DocumentType);
            if (guest_existe != null)
            {
                throw new TaskCanceledException("El Cliente ya existe, valide la informacion");
            }

            try
            {
                Guest guest_creado = await _repositorio.Crear(entidad);

                if (guest_creado.IdGuest == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el Huesped");
                }

                IQueryable<Guest> query = await _repositorio.Consultar(p => p.IdGuest == guest_creado.IdGuest);
                guest_creado = query.First();
                return guest_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Guest> Editar(Guest entidad)
        {

            try
            {

                bool respuesta = await _repositorio.Editar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Huesped");
                }

                IQueryable<Guest> queryGuest = await _repositorio.Consultar(p => p.IdGuest == entidad.IdGuest);
                Guest guest_editado = queryGuest.First();
                return guest_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idGuest)
        {
            try
            {
                Guest guest_encontrado = await _repositorio.Obtener(p => p.IdGuest == idGuest);
                if (guest_encontrado == null)
                {
                    throw new TaskCanceledException("El huesped no existe");
                }

                bool respuesta = await _repositorio.Eliminar(guest_encontrado);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }

}

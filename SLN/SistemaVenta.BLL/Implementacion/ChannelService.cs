using Microsoft.EntityFrameworkCore;
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
    public class ChannelService : IChannelService
    {
        private readonly IGenericRepository<RoomMapOrigin> _repositorio;
        private readonly IGenericRepository<Origin> _repository_origin;


        public ChannelService(IGenericRepository<RoomMapOrigin> repositorio, IGenericRepository<Origin> repository_origin)
        {
            _repositorio = repositorio;
            _repository_origin = repository_origin;
        }

        public async Task<List<RoomMapOrigin>> ListRoomMaps(int idEstabl)
        {
            IQueryable<RoomMapOrigin> query = await _repositorio.Consultar(x => x.IdEstablishment == idEstabl);
            return query.Include(p => p.IdRoomNavigation).Include(q => q.IdOriginNavigation).ToList();
        }

        public async Task<List<Origin>> ListOrigins()
        {
            IQueryable<Origin> query = await _repository_origin.Consultar();
            return query.ToList();
        }
        public async Task<RoomMapOrigin> Crear(RoomMapOrigin entidad)
        {
            try
            {
                RoomMapOrigin map_created = await _repositorio.Crear(entidad);
                if(map_created.IdRoomMap == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el canal");
                }
                return map_created;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<RoomMapOrigin> Editar(RoomMapOrigin entidad)
        {
            try
            {
                RoomMapOrigin map_encontrado = await _repositorio.Obtener(c => c.IdRoomMap == entidad.IdRoomMap);
                map_encontrado.ChannelName = entidad.ChannelName;
                map_encontrado.IdRoom = entidad.IdRoom;
                map_encontrado.IdOrigin = entidad.IdOrigin;
                map_encontrado.IdEstablishmentOrigin = entidad.IdEstablishmentOrigin;
                map_encontrado.IdRoomOrigin = entidad.IdRoomOrigin;
                map_encontrado.UrlCalendar = entidad.UrlCalendar;
                map_encontrado.IsActive = entidad.IsActive;
                map_encontrado.User = entidad.User;

                bool respuesta = await _repositorio.Editar(map_encontrado);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la channel");
                }

                return map_encontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idMap)
        {
            try
            {
                RoomMapOrigin map_encontrado = await _repositorio.Obtener(c => c.IdRoomMap == idMap);
                
                if (map_encontrado == null)
                {
                    throw new TaskCanceledException("La channel no existe");
                }
                bool respuesta = await _repositorio.Eliminar(map_encontrado);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<RoomMapOrigin>> GetRoomMapOriginByIdRoom(int idRoom)
        {
            //IQueryable<RoomMapOrigin> query = await _repositorioRoomOrigin.ConsultarLista(c => c.IdRoom == idRoom);
            //return query.ToList();
            IQueryable<RoomMapOrigin> query = await _repositorio.ConsultarLista(c => c.IdRoom == idRoom);
            return query.Include(p => p.IdOriginNavigation).Include(q => q.IdRoomNavigation).ToList();
        }
        public async Task<Origin> GetOriginById(int Id)
        {
            Origin room_found = await _repository_origin.Obtener(c => c.IdOrigin == Id);
            return room_found;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SistemaVenta.BLL.Implementacion
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _repositorio;
        //private readonly IGenericRepository<RoomMapOrigin> _repositorioRoomOrigin;
        private readonly IFireBaseService _firebaseSerice;
        private readonly IUtilidadesService _utilidadesService;
        private readonly IGenericRepository<RoomStatus> _repositorioRoomStatus;

        public RoomService(IGenericRepository<Room> repositorio,
            IFireBaseService firebaseSerice,
            IUtilidadesService utilidadesService,
            IGenericRepository<RoomStatus> repositorioStatus)
        {
            _repositorio = repositorio;
            _firebaseSerice = firebaseSerice;
            _utilidadesService = utilidadesService;
            _repositorioRoomStatus = repositorioStatus;
        }

        public async Task<List<Room>> Listar(int idEstablishment)
        {
            IQueryable<Room> query = await _repositorio.Consultar(x => x.IdEstablishment == idEstablishment);
            return query.Include(p => p.IdCategoriaNavigation).Include(x=>x.IdRoomStatusNavigation).ToList();
        }
        public async Task<List<Room>> ListByLevel(int idEstablishment, int idLevel)
        {
            IQueryable<Room> query = await _repositorio.ConsultarLista(c => c.IdLevel == idLevel && c.IdEstablishment == idEstablishment);
            return query.Include(p => p.IdCategoriaNavigation).Include(x => x.IdRoomStatusNavigation).ToList();
        }

        public async Task<List<Room>> GetByIdEstablishment(int idCompany)
        {
            IQueryable<Room> query = await _repositorio.ConsultarLista(c => c.IdEstablishment == idCompany);
            return query.Include(p => p.IdCategoriaNavigation).ToList();
        }

        public async Task<Room> GetRoomById(int Id)
        {
            Room room_found = await _repositorio.Obtener(c => c.IdRoom == Id);
            return room_found;
        }

        public async Task<Room> CreateRoom(Room entidad)
        {
            Room room_existe = await _repositorio.Obtener(p => p.Number == entidad.Number && p.IdEstablishment == entidad.IdEstablishment);
            if (room_existe != null)
            {
                throw new TaskCanceledException("El Numero de Habitacion ya existe");
            }

            try
            {
                Room room_creado = await _repositorio.Crear(entidad);

                if (room_creado.IdRoom == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el room");
                }

                IQueryable<Room> query = await _repositorio.Consultar(p => p.IdRoom == room_creado.IdRoom);
                room_creado = query.Include(c => c.IdCategoriaNavigation).First();
                return room_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Room> Editar(Room entidad, Stream imagen = null)
        {
            Room room_existe = await _repositorio.Obtener(p => p.Number == entidad.Number && p.IdRoom != entidad.IdRoom && p.IdEstablishment == entidad.IdEstablishment);
            if (room_existe != null)
            {
                throw new TaskCanceledException("El numero de habitacion ya existe");
            }
            try
            {
                IQueryable<Room> queryRoom = await _repositorio.Consultar(p => p.IdRoom == entidad.IdRoom);
                Room room_para_editar = queryRoom.First();
                room_para_editar.Number = entidad.Number;
                room_para_editar.Description = entidad.Description;
                room_para_editar.IdCategoria = entidad.IdCategoria;
                room_para_editar.CategoryName = entidad.CategoryName;
                room_para_editar.Capacity = entidad.Capacity;
                room_para_editar.SizeRoom = entidad.SizeRoom;
                room_para_editar.IdRoomStatus = entidad.IdRoomStatus;
                room_para_editar.IdLevel = entidad.IdLevel;
                // room_para_editar.Price = entidad.Price;
                room_para_editar.IsActive = entidad.IsActive;


                //if (imagen != null)
                //{
                //    string urlImagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_room", room_para_editar.NameImage);
                //    room_para_editar.UrlImage = urlImagen;
                //}

                bool respuesta = await _repositorio.Editar(room_para_editar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el producto");
                }

                //Room room_editado = queryRoom.Include(c => c.IdCategoriaNavigation).Include(x => x.IdRoomStatusNavigation).First();
                Room room_editado = queryRoom.Include(x => x.IdRoomStatusNavigation).First();
                return room_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idRoom)
        {
            try
            {
                Room room_encontrado = await _repositorio.Obtener(p => p.IdRoom == idRoom);
                if (room_encontrado == null)
                {
                    throw new TaskCanceledException("El room no existe");
                }

                //string nombreImagen = room_encontrado.NameImage;
                bool respuesta = await _repositorio.Eliminar(room_encontrado);
                //if (respuesta)
                //{
                //    await _firebaseSerice.EliminarStorage("carpeta_room", nombreImagen);
                //}

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public async Task<List<RoomMapOrigin>> GetRoomMapOriginByIdRoom(int idRoom)
        //{
        //    //IQueryable<RoomMapOrigin> query = await _repositorioRoomOrigin.ConsultarLista(c => c.IdRoom == idRoom);
        //    //return query.ToList();
        //    IQueryable<RoomMapOrigin> query = await _repositorioRoomOrigin.ConsultarLista(c => c.IdRoom == idRoom);
        //    return query.Include(p => p.IdOriginNavigation).Include(q => q.IdRoomNavigation).ToList();
        //}

        public async Task<List<Room>> GetRoomsByNumberAndStatus(string number, int idBookStatus, int idEstablishment)
        {
            IQueryable<Room> query = await _repositorio.Consultar(p => p.IsActive == true &&
                                                                  p.IdEstablishment == idEstablishment &&
                                                                  p.Number.Contains(number) &&
                                                                  p.DetailBook.Any(d => d.IdBookNavigation.IdBookStatus == idBookStatus));// && 
                                                                                                                                          //(d.IdBookNavigation.CheckIn <= DateTime.Now && d.IdBookNavigation.CheckOut >= DateTime.Now)));
            return query.ToList();
        }

        public async Task<List<RoomStatus>> ListarRoomStatus()
        {
            IQueryable<RoomStatus> query = await _repositorioRoomStatus.Consultar();
            return query.ToList();
        }

    }
}

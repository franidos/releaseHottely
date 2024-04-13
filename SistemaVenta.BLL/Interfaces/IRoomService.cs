using SistemaVenta.BLL.Implementacion;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IRoomService
    {
        Task<List<Room>> Listar(int idEstablishment);
        Task<List<Room>> GetByIdEstablishment(int idCompany);
        Task<List<Room>> ListByLevel(int idEstablishment,int level);
        Task<Room> CreateRoom(Room entidad );
        Task<Room> Editar(Room entidad, Stream imagen = null);
        Task<bool> Eliminar(int idRoom);
        Task<Room> GetRoomById(int idRoom);
        //Task<List<RoomMapOrigin>> GetRoomMapOriginByIdRoom(int idRoom);
        Task<List<Room>> GetRoomsByNumberAndStatus(string number, int idBookStatus,int idEstablishment);
        Task<List<RoomStatus>> ListarRoomStatus();
    }
}

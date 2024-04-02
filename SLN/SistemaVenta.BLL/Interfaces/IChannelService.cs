using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IChannelService
    {
        Task<List<RoomMapOrigin>> ListRoomMaps(int idEstabl);
        Task<List<Origin>> ListOrigins();
        Task<RoomMapOrigin> Crear(RoomMapOrigin entidad);
        Task<RoomMapOrigin> Editar(RoomMapOrigin entidad);
        Task<bool> Eliminar(int idMap);
        Task<List<RoomMapOrigin>> GetRoomMapOriginByIdRoom(int idRoom);
        Task<Origin> GetOriginById(int Id);


    }
}

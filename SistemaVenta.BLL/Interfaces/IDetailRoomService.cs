using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IDetailRoomService
    {
        Task<List<DetailRoom>> GetDetailRoom(int idRoom);
        Task<DetailRoom> Create(DetailRoom entidad);
    }
}

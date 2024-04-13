using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class DetailRoomService : IDetailRoomService
    {
        private readonly IGenericRepository<DetailRoom> _repositorio;

        public DetailRoomService(IGenericRepository<DetailRoom> repositorio, DbventaContext dbContext)
        {
            _repositorio = repositorio;
        }

        public async Task<List<DetailRoom>> GetDetailRoom(int idRoom)
        {
            IQueryable<DetailRoom> query = await _repositorio.Consultar(x=>x.IdRoom == idRoom);
            return query.ToList();
        }

        public async Task<DetailRoom> Create(DetailRoom entidad)
        {
            try
            {

                DetailRoom detail_Room_created = await _repositorio.Crear(entidad);

                if (detail_Room_created.IdRoom == 0)
                {
                    throw new TaskCanceledException("No se pudo crear detalle Habitacion");
                }

                return detail_Room_created;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}

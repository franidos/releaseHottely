using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IMovimientoRepository: IGenericRepository<Movimiento>
    {
        Task<Movimiento> Registrar(Movimiento entidad);
        Task<List<DetalleMovimiento>> Reporte(int idEstablishment, DateTime fechaInicio,DateTime fechaFin);
    }
}

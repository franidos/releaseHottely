using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IPedidoRepository: IGenericRepository<Movimiento>
    {
        Task<Movimiento> Registrar(Movimiento entidad);//todo volar
       // Task<List<DetalleMovimiento>> Reporte(DateTime fechaInicio,DateTime fechaFin);
    }
}

using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IMovimientoService
    {
        Task<List<Producto>> ObtenerProductos(int idEstablishment, string busqueda);
        Task<Movimiento> Registrar(Movimiento entidad);
        Task<List<Movimiento>> Historial(int idEstablishment,string numeroMovimiento, string buscarPorTipo, string fechaInicio, string fechaFin);
        Task<Movimiento> Detalle(string numeroMovimiento, int idEstablishment);
        Task<List<DetalleMovimiento>> Reporte(int idEstablishment, string fechaInicio, string fechaFin);
        Task<List<Movimiento>> GetMovimientosBooking(int idMvtoRel);
        Task<Movimiento> Editar(Movimiento entidad);

    }
}

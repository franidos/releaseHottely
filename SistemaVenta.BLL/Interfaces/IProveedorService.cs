using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IProveedorService
    {
        Task<List<Proveedor>> Listar(int idEstablishment);
        Task<Proveedor> Crear(Proveedor entidad);
        Task<Proveedor> Editar(Proveedor entidad);
        Task<bool> Eliminar(int idProveedor);
    }
}

using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICategoriaProductoService
    {
        Task<List<CategoriaProducto>> Lista(int idEstablishment);
        Task<CategoriaProducto> Crear(CategoriaProducto entidad);
        Task<CategoriaProducto> Editar(CategoriaProducto entidad);
        Task<bool> Eliminar(int idCategoriaProducto);
    }
}

using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IEstablishmentService
    {
        Task<Establishment> GuardarCambios(Establishment entidad, Stream logo = null, string nombreLogo = "");
        Task<Establishment> getEstablishmentById(int Id);
        Task<Establishment> Crear(Establishment entidad, Stream imagen = null, string nombreImagen = ""); 
        Task<Establishment> Editar(Establishment entidad, Stream imagen = null);
        Task<bool> Eliminar(int idEstablishment);
    }
}

using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IGuestService
    {
        Task<List<Guest>> getGuestsByParam(string busqueda, int idEstabl);
        Task<Guest> getGuestById(int idGuest);
        Task<List<Guest>> Listar(int idEstabl);
        Task<Guest> getGuestByDocument(string document);
        Task<Guest> Crear(Guest entidad);
        Task<Guest> Editar(Guest entidad);
        Task<bool> Eliminar(int idGuest);

    }
}

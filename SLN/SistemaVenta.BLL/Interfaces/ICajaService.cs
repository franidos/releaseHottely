using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICajaService
    {
        Task<List<Caja>> Listar();
        Task<Caja> Crear(Caja entidad);
        Task<Caja> Editar(Caja entidad);
        bool CajaCierre(int idUsuario);
        Caja? ObtenerCajaUsuario(int idUsuario);
    }
}

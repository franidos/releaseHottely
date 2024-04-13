using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IMedioPagoService
    {
        Task<List<MedioPago>> Listar();
        //Task<Caja> Crear(Caja entidad);
        //Task<Caja> Editar(Caja entidad);
        //bool CajaCierre(int idUsuario);
    }
}

using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ILevelService
    {
        Task<List<Level>> Lista(int idEstabl);
        Task<Level> Crear(Level entidad);
        Task<Level> Editar(Level entidad);
        Task<bool> Eliminar(int idLevel);
    }
}

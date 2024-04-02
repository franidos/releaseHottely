using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IDashBoardService
    {       
        Task<int> TotalHabitacionesDisponibles(int idEstabl);
        Task<int> TotalHabitacionesOcupadas(int idEstabl);
        Task<int> TotalHabitacionesReservadas(int idEstabl);
        Task<int> TotalHabitacionesFueraDeServicio(int idEstabl);
        Task<int> TotalVentasCajaxFecha(int idEstabl, DateTime? fechaInicio);
        Task<int> TotalIngresoxFecha(int idEstabl, DateTime? fechaInicio);
        Task<int> TotalHabitaciones(int idEstabl);
        Task<int> TotalProductos(int idEstabl);
        Task<int> TotalCategoriasProductos(int idEstabl);
        Task<Dictionary<string, int>> MovimientosUltimaSemana(int idEstabl);
        Task<Dictionary<string, int>> HabitacionesTopUltimaSemana(int idEstabl);

    }
}

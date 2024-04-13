using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IPlanService
    {
        Task<List<Plan>> Lista();
        Task<Plan> GetPlanById(int id);
    }
}

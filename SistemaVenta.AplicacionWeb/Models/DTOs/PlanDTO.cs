using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class PlanDTO
    {
        public int IdPlan { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}

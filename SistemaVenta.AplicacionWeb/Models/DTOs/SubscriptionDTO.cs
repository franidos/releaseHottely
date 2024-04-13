using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class SubscriptionDTO
    {
        public int IdSubscription { get; set; }
        public int IdCompany { get; set; }
        public int IdPlan { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SubscriptionStatus { get; set; }
        public string PaymentReceipt { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DateIni { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
    }
}


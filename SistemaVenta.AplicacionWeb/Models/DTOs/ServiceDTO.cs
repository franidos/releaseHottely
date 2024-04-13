namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ServiceDTO
    {
        public int IdService { get; set; }
        public int? IdEstablishment { get; set; }

        public string ServiceName { get; set; }
        public string? ServiceInfo { get; set; }
        public string? ServiceInfoQuantity { get; set; }
        public int? ServiceMaximumAmount { get; set; }
        public string? ServiceConditions { get; set; }
        public decimal? ServicePrice { get; set; }
        public string? ServiceUrlImage { get; set; }
        public int? ServiceIsActive { get; set; }
        public int? IsAdditionalValue { get; set; }
    }
}

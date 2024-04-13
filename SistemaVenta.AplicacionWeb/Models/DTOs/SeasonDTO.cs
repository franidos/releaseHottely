namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class SeasonDTO
    {
        public int IdSeason { get; set; }
        public int IdEstablishment { get; set; }
        public string Name { get; set; }
        public DayOfWeek DayOfTheWeek { get; set; }
        public DateTime Date { get; set; }
        public decimal Increment { get; set; }
        public string User { get; set; }
        public int? IsActive { get; set; }
    }
}

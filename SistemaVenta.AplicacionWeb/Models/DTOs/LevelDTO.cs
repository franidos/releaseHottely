namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class LevelDTO
    {
        public int IdLevel { get; set; }
        public int IdEstablishment { get; set; }
        public int LevelNumber { get; set; }
        public string LevelName { get; set; }
        public int? IsActive { get; set; }
    }
}

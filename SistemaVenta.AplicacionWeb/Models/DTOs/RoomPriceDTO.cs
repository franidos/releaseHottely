namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class RoomPriceDTO
    {
        public int IdRoomPrice { get; set; }
        public int IdEstablishment { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string Name { get; set; }
        public decimal Monday { get; set; }
        public decimal Tuesday { get; set; }
        public decimal Wednesday { get; set; }
        public decimal Thursday { get; set; }
        public decimal Friday { get; set; }
        public decimal Saturday { get; set; }
        public decimal Sunday { get; set; }
        public string User { get; set; }
        public int? IsActive { get; set; }
    }
}

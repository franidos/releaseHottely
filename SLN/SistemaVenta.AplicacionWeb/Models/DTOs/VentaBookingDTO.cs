using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class VentaBookingDTO
    {
        public BookDTO? Booking { get; set; }
        public RoomDTO? RoomMain { get; set; }

    }
}

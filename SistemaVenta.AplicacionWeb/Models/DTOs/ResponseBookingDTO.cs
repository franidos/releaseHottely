using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ResponseBookingDTO
    {
        public MovimientoDTO Movement { get; set; }
        public BookDTO Book { get; set; }
        public virtual List<GuestDTO> Guests { get; set; }
        //public virtual List<DetailBookDTO> Details { get; set; }

    }
}

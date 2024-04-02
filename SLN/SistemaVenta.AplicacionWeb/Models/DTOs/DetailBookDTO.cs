using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class DetailBookDTO
    {
        public int IdDetailBook { get; set; }
        public int IdBook { get; set; }
        public int? IdCategoria { get; set; }
        public int IdRoom { get; set; }
        public int IdGuest { get; set; }
        public decimal? Price { get; set; }
        public virtual RoomDTO? IdRoomNavigation { get; set; }
        public virtual GuestDTO? IdGuestNavigation { get; set; }


    }
}

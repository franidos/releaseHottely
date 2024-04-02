using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class RequestRoomsBookingDTO
    {
        public int IdEstablishment { get; set; }
        public int IdRoom { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int CountAdult { get; set; }
        public int CountChildren { get; set; }
        public int ChildrenAge { get; set; }
        public int CountRooms { get; set; }
        public int RoomId { get; set; }
        public int CategoryRoom { get; set; }
        public bool CkeckDates { get; set; }

    }
}

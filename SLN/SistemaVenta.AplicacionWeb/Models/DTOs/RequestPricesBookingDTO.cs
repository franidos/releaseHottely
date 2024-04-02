using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class RequestPricesBookingDTO
    {
        public string Adults { get; set; }
        public string AgeChildren { get; set; }
        public List<RoomDTO> TempRoom { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }

    }

    //public class TempRoomDTO
    //{

    //    public string NumberRoom { get; set; }
    //    public int IdCategoria { get; set; }
    //}

}

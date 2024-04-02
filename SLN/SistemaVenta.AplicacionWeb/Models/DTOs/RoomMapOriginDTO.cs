using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class RoomMapOriginDTO
    {
        public int IdRoomMap { get; set; }
        public int IdEstablishment { get; set; }
        public int IdRoom { get; set; }
        public int IdOrigin { get; set; }
        public string? ChannelName { get; set; }
        public string User { get; set; }
        public string? OriginName { get; set; }
        public string? RoomName { get; set; }
        public string? IdEstablishmentOrigin { get; set; }
        public string? IdRoomOrigin { get; set; }
        public string? UrlCalendar { get; set; }
        public int? IsActive { get; set; }

    }
}

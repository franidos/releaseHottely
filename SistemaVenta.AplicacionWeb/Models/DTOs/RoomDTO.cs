using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class RoomDTO
    {
        public int IdRoom { get; set; }
        public int IdEstablishment { get; set; }
        public int IdCategoria { get; set; }
        public int IdLevel { get; set; }
        public string Number { get; set; }
        public string? CategoryName { get; set; }
        public string? StatusName { get; set; }
        public string? StatusBackgroud { get; set; }
        public string? RoomTitle { get; set; }
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public int? SizeRoom { get; set; }
        public int? IdRoomStatus { get; set; }
        public int? IsActive { get; set; }
        //public virtual ICollection<DetailBookDTO>? DetailBook { get; set; }
        //public virtual ICollection<ImagesRoomDTO>? ImagesRoom { get; set; }
        //  public virtual RoomStatus? IdRoomStatusNavigation { get; set; }

    }
}

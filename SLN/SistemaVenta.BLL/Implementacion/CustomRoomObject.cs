using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class RoomsAndCategoryObject 
    {
        public int IdRoom { get; internal set; }
        public int IdEstablishment { get; internal set; }
        public int FkCategoria { get; internal set; }
        public string Number { get; internal set; }
        public string? Title { get; internal set; }
        public string? Info { get; internal set; }
        public int? Capacity { get; internal set; }
        public int? Size { get; internal set; }
        public string UrlImage { get; internal set; }
        public decimal? Price { get; internal set; }
        public int? IdRoomStatus { get; internal set; }
        public bool? IsActive { get; internal set; }

        public string? CategoryName { get; internal set; }
        public string? RoomTypeInfo { get; internal set; }
        public bool? RoomTypeStatus { get; internal set; }
        public List<ImagesRoom>? RoomImages { get; set; }
    }
}
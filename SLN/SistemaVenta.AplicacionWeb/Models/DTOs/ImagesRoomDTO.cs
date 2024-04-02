using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ImagesRoomDTO
    {
        public int IdImagesRoom { get; set; }
        public int IdRoom { get; set; }
        public int ImageNumber { get; set; }
        public string NameImage { get; set; }
        public string UrlImage { get; set; }

    }
}

using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ImagesEstablishmentDTO
    {
        public int IdImagesEstablishment { get; set; }
        public int IdEstablishment { get; set; }
        public int ImageNumber { get; set; }
        public string NameImage { get; set; }
        public string UrlImage { get; set; }

    }
}

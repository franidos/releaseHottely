using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class MedioPagoDTO
    {
        public int IdMedioPago { get; set; }
        public string? Descripcion { get; set; }
        public string Naturaleza { get; set; }
        public string? UrlImagen { get; set; }

    }
}

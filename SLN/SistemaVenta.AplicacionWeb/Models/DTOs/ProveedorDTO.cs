using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ProveedorDTO
    {
        public int IdProveedor { get; set; }
        public int IdEstablishment { get; set; }
        public string NIT { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int? EsActivo { get; set; }


    }
}

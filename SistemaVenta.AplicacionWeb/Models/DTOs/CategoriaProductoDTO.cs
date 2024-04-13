namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class CategoriaProductoDTO
    {
        public int IdCategoriaProducto { get; set; }
        public int IdEstablishment { get; set; }
        public string NombreCategoriaProducto { get; set; }
        public string? Descripcion { get; set; }
        public int? EsActivo { get; set; }
    }
}

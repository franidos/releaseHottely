namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public int IdEstablishment { get; set; }

        public string? CodigoBarra { get; set; }

        public string? Marca { get; set; }

        public string? Descripcion { get; set; }

        public int? IdCategoriaProducto { get; set; }
        public int? IdProveedor { get; set; }
        public string? NombreCategoria { get; set; }

        public int? Stock { get; set; }

        public string? UrlImagen { get; set; }
        public decimal? PrecioCompra { get; set; }
        public decimal? Imp { get; set; }

        public decimal? Precio { get; set; }

        public int? EsActivo { get; set; }
    }
}

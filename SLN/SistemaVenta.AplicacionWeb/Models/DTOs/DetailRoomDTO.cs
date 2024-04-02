namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class DetailRoomDTO
    {
        public int? IdProducto { get; set; }

        public string? DescripcionProducto { get; set; }

        public int? Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public decimal? Total { get; set; }

        //Custom Fields
        public decimal? SubTotal { get; set; }
    }
}

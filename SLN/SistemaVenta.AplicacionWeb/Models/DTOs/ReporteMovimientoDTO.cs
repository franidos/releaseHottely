namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ReporteMovimientoDTO
    {
        public string? FechaRegistro { get; set; }
        public string? NumeroMovimiento { get; set; }
        public string? TipoDocumento { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? SubTotalMovimiento { get; set; }
        public string? ImpuestoTotalMovimiento { get; set; }
        public string? TotalMovimiento { get; set; }
        public string? Producto { get; set; }
        public int Cantidad { get; set; }
        public string? Precio { get; set; }
        public string? Total { get; set; }
    }
}

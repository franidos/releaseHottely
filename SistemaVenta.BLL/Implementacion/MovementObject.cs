using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class MovementObject 
    {

        public int IdMovimiento { get; set; }

        public string? NumeroMovimiento { get; set; }

        public int? IdTipoDocumentoMovimiento { get; set; }

        public int? IdUsuario { get; set; }
        public int? IdProveedor { get; set; }
        public string? NumeroDocumentoExterno { get; set; }

        public string? DocumentoCliente { get; set; }

        public string? NombreCliente { get; set; }

        public decimal? SubTotal { get; set; }

        public decimal? ImpuestoTotal { get; set; }
        public decimal? TotalRoom { get; set; }
        public decimal? Total { get; set; }
    }
}
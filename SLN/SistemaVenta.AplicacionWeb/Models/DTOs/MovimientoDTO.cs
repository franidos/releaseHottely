using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class MovimientoDTO
    {
        public int IdMovimiento { get; set; }
        public int IdEstablishment { get; set; }

        public string? NumeroMovimiento { get; set; }

        public int? IdTipoDocumentoMovimiento { get; set; }

        public string? TipoDocumentoMovimiento { get; set; }

        public int? IdUsuario { get; set; }

        public string? Usuario { get; set; }
        public int? IdProveedor { get; set; }

        public int? IdMovimientoRel { get; set; }

        public string? NumeroDocumentoExterno { get; set; }

        public string? DocumentoCliente { get; set; }

        public string? NombreCliente { get; set; }

        public decimal? SubTotal { get; set; }

        public decimal? ImpuestoTotal { get; set; }

        public decimal? Total { get; set; }
        public decimal? TotalRoom { get; set; }

        public string? FechaRegistro { get; set; }
        public virtual ICollection<DetalleMovimientoDTO>? DetalleMovimiento { get; set; }
        public virtual ICollection<DetalleCaja>? DetalleCaja { get; set; }

        
        // -- Fields Custom --
        public int? IdMedioPago { get; set; }
        public decimal? TotalEfectivoMixto { get; set; }
        public string? Observacion { get; set; }
        public decimal? TotalRecibido { get; set; }
        public decimal? TotalCambio { get; set; }
        public bool PrintTicket { get; set; }

        // -- Checkout --
        public int? IdBookCheckout { get; set; }
        public decimal? SaldoReserva { get; set; }
        public decimal? AbonoReserva { get; set; }
        public decimal? ValorCaja { get; set; }
        public decimal? CostoAdic { get; set; }


    }
}

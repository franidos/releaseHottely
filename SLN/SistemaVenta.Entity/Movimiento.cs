using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Movimiento
{
    public int IdMovimiento { get; set; }

    public int IdEstablishment { get; set; }

    public string? NumeroMovimiento { get; set; }

    public int? IdTipoDocumentoMovimiento { get; set; }

    public int? IdUsuario { get; set; }
    public int? IdProveedor { get; set; }
    public int? IdMovimientoRel { get; set; } //Referencia o dependencia a otro movimiento existente
    public string? NumeroDocumentoExterno { get; set; }

    public string? DocumentoCliente { get; set; }

    public string? NombreCliente { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? ImpuestoTotal { get; set; }
    public decimal? TotalRoom { get; set; }
    public decimal? Total { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<DetalleMovimiento>? DetalleMovimiento { get; } = new List<DetalleMovimiento>();

    public virtual TipoDocumentoMovimiento? IdTipoDocumentoMovimientoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
    public virtual Proveedor? IdProveedorNavigation { get; set; }
    public virtual ICollection<Book>? Book { get; set; }
    public virtual ICollection<DetalleCaja>? DetalleCaja { get; set; }
    public virtual Establishment IdEstablishmentNavigation { get; set; }
}

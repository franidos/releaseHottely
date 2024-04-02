using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class DetalleMovimiento
{
    public int IdDetalleMovimiento { get; set; }

    public int? IdMovimiento { get; set; }

    public int IdProducto { get; set; }

    public string? DescripcionProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Total { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Movimiento? IdMovimientoNavigation { get; set; }
}

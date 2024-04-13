using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class DetalleCaja
{
    public int IdDetalleCaja { get; set; }
    public int IdCaja { get; set; }
    public int? IdMovimiento { get; set; }
    public int IdMedioPago { get; set; }
    public string? Observacion { get; set; }
    public decimal Valor { get; set; }
    public virtual Caja? IdCajaNavigation { get; set; }
    public virtual Movimiento? IdMovimientoNavigation { get; set; }
    public virtual MedioPago? IdMedioPagoNavigation { get; set; }
}

using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Caja
{
    public int IdCaja { get; set; }
    public int IdEstablishment { get; set; }
    public int IdUsuario { get; set; }
    public int? IdAreaFisica { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaCierre { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public decimal? SaldoReal { get; set; }
    public bool Estado { get; set; }
    public string? Observacion { get; set; }
    public virtual Establishment IdEstablishmentNavigation { get; set; }
    public virtual Usuario IdUsuarioNavigation { get; set; }
    public virtual AreaFisica? IdAreaNavigation { get; set; }
    public virtual ICollection<DetalleCaja>? DetalleCaja { get; } = new List<DetalleCaja>();
}

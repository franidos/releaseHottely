using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class TipoDocumentoMovimiento
{
    public int IdTipoDocumentoMovimiento { get; set; }

    public string? Descripcion { get; set; }
    public string Naturaleza { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Movimiento> Movimiento { get; } = new List<Movimiento>();
}

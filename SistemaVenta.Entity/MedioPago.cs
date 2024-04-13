using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class MedioPago
{
    public int IdMedioPago { get; set; }
    public string? Descripcion { get; set; }
    public string Naturaleza { get; set; }
    public string? UrlImagen { get; set; }
    public virtual ICollection<DetalleCaja>? DetalleCaja { get; set; }
}

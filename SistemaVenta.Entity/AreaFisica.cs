using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class AreaFisica
{
    public int IdAreaFisica { get; set; }
    public int IdEstablishment { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? NombreImpresora { get; set; }
    public bool Estado { get; set; }  
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
    public virtual ICollection<Caja> CajaNavigation { get; } = new List<Caja>();
}

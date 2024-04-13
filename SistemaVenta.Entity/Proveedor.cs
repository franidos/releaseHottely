using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Proveedor
{
    public int IdProveedor { get; set; }
    public int IdEstablishment { get; set; }
    public string NIT { get; set; }
    public string Nombre { get; set; }
    public string Contacto { get; set; }
    public string Telefono { get; set; }
    public string? Correo { get; set; }
    public bool? EsActivo { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
    public virtual ICollection<Producto> Productos { get; } = new List<Producto>();
    public virtual ICollection<Movimiento> Movimiento { get; } = new List<Movimiento>();
}
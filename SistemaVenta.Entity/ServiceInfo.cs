using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class ServiceInfo
{
    public int IdServiceInfo { get; set; }
    public string? Descripcion { get; set; }
    public string? Icon { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public virtual ICollection<ServiceInfoEstablishment>? ServiceInfoEstablNavigation { get; } = new List<ServiceInfoEstablishment>();
}
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class ServiceInfoEstablishment
{
    public int IdServiceInfoEstab { get; set; }
    public int IdEstablishment { get; set; }
    public int? IdServiceInfo { get; set; }
    public string? DescripcionOpc { get; set; }
    public DateTime CreationDate { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
    public virtual ServiceInfo? IdServiceInfoNavigation { get; set; }
}
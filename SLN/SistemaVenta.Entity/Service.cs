using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Service
{
    public int IdService { get; set; }
    public int? IdEstablishment { get; set; }
    public int? IdRoom { get; set; }
    public string ServiceName { get; set; }
    public string? ServiceInfo { get; set; }
    public string? ServiceInfoQuantity { get; set; }
    public int? ServiceMaximumAmount { get; set; }
    public string? ServiceConditions { get; set; }
    public decimal? ServicePrice { get; set; }
    public string? ServiceImageName { get; set; }
    public string? ServiceUrlImage { get; set; }
    public bool? ServiceIsActive { get; set; }
    public bool? IsAdditionalValue { get; set; } // Para indicar si el servicio esta incluido en el precio o no (es free)
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
    public virtual Room? IdRoomNavigation { get; set; }
}
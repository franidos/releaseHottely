using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Subscription
{
    public int IdSubscription { get; set; }
    public int IdEstablishment { get; set; }
    public int IdPlan { get; set; }
    public string? PaymentMethod { get; set; }
    public string? SubscriptionStatus { get; set; }
    public string PaymentReceipt { get; set; }
    public int? IdUsuario { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DateIni { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public Establishment Establishment { get; set; }
    public Plan Plan { get; set; }
}

using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Plan
{
    public int IdPlan { get; set; }
    public string PlanName { get; set; }
    public string PlanDescription { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public List<Subscription> Subscriptions { get; set; }
    public List<ParamPlan> ParamPlans { get; set; }
}
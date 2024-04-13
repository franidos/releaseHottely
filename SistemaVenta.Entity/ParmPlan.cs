using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class ParamPlan
{
    public int IdParamPlan { get; set; }
    public int IdPlan { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual Plan IdPlanNavigation { get; set; }
}
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Holiday
{
    public int IdHoliday { get; set; }
    public int IdEstablishment { get; set; }
    public string Name { get; set; }
    public DayOfWeek DayOfTheWeek { get; set; }
    public DateTime Date { get; set; }
    public decimal Increment { get; set; }
    public string User { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public virtual Establishment IdEstablishmentNavigation { get; set; }

}

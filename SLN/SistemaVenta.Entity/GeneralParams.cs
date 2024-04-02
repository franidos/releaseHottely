using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class GeneralParams
{
    public int Id { get; set; }
    public string Item1 { get; set; }
    public string Item2 { get; set; }
    public string Item3 { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
}
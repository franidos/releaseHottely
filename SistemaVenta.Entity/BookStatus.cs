using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class BookStatus
{
    public int IdBookStatus { get; set; }
    public string StatusName { get; set; }
    public string Background { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual ICollection<Book>? Book { get; set; }
}
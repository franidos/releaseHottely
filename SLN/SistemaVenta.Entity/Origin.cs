using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Origin
{
    public int IdOrigin { get; set; }
    public string Name { get; set; }
    public string EventTitle { get; set; }
    public string BackgroundColor { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual ICollection<Book>? Book { get; } = new List<Book>();
    public virtual ICollection<RoomMapOrigin>? RoomMapOrigin { get; } = new List<RoomMapOrigin>();
}
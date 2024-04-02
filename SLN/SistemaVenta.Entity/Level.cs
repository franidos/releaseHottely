using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Level
{

    public int IdLevel { get; set; }
    public int IdEstablishment { get; set; }
    public int LevelNumber { get; set; }
    public string LevelName { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
    public virtual Establishment IdEstablishmentNavigation { get; set; }
}


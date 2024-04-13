using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class RoomStatus
{

    public int IdRoomStatus { get; set; }
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string TitlePor { get; set; }
    public string Background { get; set; }
    public bool? IsActive { get; set; }
    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
    public virtual ICollection<DetailRoom>? DetailRoom { get; } = new List<DetailRoom>();

}


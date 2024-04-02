using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class DetailRoom
{
    public int IdDetailRoom { get; set; }
    public int IdRoom { get; set; }
    public int IdRoomStatus { get; set; }
    public int? IdUsuario { get; set; }
    public string? Observation { get; set; }
    public virtual Usuario? IdUsuarioNavigation { get; set; }
    public virtual Room? IdRoomNavigation { get; set; }
    public virtual RoomStatus? IdRoomStatusNavigation { get; set; }

}

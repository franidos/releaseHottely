using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class RoomMapOrigin
{
    public int IdRoomMap { get; set; }
    public int IdEstablishment { get; set; }
    public int? IdRoom { get; set; }
    public int? IdOrigin { get; set; }
    public string? ChannelName { get; set; }
    public string? IdEstablishmentOrigin { get; set; }
    public string? IdRoomOrigin { get; set; }
    public string? UrlCalendar { get; set; }
    public string User { get; set; }
    public DateTime CreationDate { get; set; }
    public bool? IsActive { get; set; }
    public virtual Room? IdRoomNavigation { get; set; }
    public virtual Origin? IdOriginNavigation { get; set; }
    public virtual Establishment IdEstablishmentNavigation { get; set; }


}
using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Room
{

    public int IdRoom { get; set; }
    public int IdEstablishment { get; set; }
    public int IdCategoria { get; set; }
    public int IdLevel { get; set; }
    public string Number { get; set; }
    public string? CategoryName { get; set; }
    public string? RoomTitle { get; set; }
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public int? SizeRoom { get; set; }
    public int? IdRoomStatus { get; set; }
    public bool? IsActive { get; set; }   
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public virtual Categoria? IdCategoriaNavigation { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
    public virtual Level? IdLevelNavigation { get; set; }
    public virtual ICollection<DetailBook> DetailBook { get; } = new List<DetailBook>();
    public virtual ICollection<ImagesRoom>? ImagesRoom { get; } = new List<ImagesRoom>();
    public virtual ICollection<Service>? Service { get; } = new List<Service>();
    public virtual ICollection<RoomMapOrigin>? RoomMapOrigin { get; } = new List<RoomMapOrigin>();
    public virtual RoomStatus? IdRoomStatusNavigation { get; set; }
    public virtual ICollection<DetailRoom>? DetailRoom { get; } = new List<DetailRoom>();

}


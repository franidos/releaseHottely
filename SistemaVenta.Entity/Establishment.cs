using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SistemaVenta.Entity;

public partial class Establishment
{

    public int IdEstablishment { get; set; }
    public string NIT { get; set; }
    public string EstablishmentName { get; set; }
    public string Email { get; set; }
    public string EstablishmentType { get; set; }
    public string Contact { get; set; }
    public string Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string PhoneNumber { get; set; }
    public string? Descripcion { get; set; }
    public string? Geolocation { get; set; }
    public string Token { get; set; }
    public string Rnt { get; set; }
    public string? NameImage { get; set; }
    public string? UrlImage { get; set; }
    public string Tax { get; set; }
    public string Currency { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public List<Usuario> Usuarios { get; set; }
    public List<Room> Rooms { get; set; }
    public List<Subscription> Subscriptions { get; set; }
    public virtual ICollection<ImagesEstablishment>? ImagesEstablishment { get; } = new List<ImagesEstablishment>();
    public virtual ICollection<Service>? Service { get; } = new List<Service>();
    public virtual ICollection<ServiceInfoEstablishment>? ServiceEstabNavigation { get; } = new List<ServiceInfoEstablishment>();
    public virtual ICollection<Caja>? Cajas { get; } = new List<Caja>();
    public virtual ICollection<Proveedor>? Proveedores { get; } = new List<Proveedor>();
    public virtual ICollection<Producto>? Productos { get; } = new List<Producto>();
    public virtual ICollection<CategoriaProducto> CategoriaProductos { get; } = new List<CategoriaProducto>();
    public virtual ICollection<AreaFisica> AreaFisica { get; } = new List<AreaFisica>();
    public virtual ICollection<Guest> Guests { get; } = new List<Guest>();
    public virtual ICollection<Level> Levels { get; } = new List<Level>();
    public virtual ICollection<RoomPrice> RoomPrices { get; } = new List<RoomPrice>();
    public virtual ICollection<Season> Seasons { get; } = new List<Season>();
    public virtual ICollection<Holiday> Holidays { get; } = new List<Holiday>();
    public virtual ICollection<Movimiento> Movimientos { get; } = new List<Movimiento>();
    public virtual ICollection<RoomMapOrigin> RoomMapOrigins { get; } = new List<RoomMapOrigin>();
    public virtual ICollection<NumeroCorrelativo> NumeroCorrelativos { get; } = new List<NumeroCorrelativo>();
}
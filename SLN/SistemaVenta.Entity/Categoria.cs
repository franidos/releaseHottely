using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Categoria
{
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; }
    public string? Descripcion { get; set; }
    public bool? EsActivo { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
    public virtual ICollection<RoomPrice>? RoomPrice { get; } = new List<RoomPrice>();
    public virtual ICollection<Season>? SeasonPrice { get; } = new List<Season>();
    public virtual ICollection<DetailBook>? DetailBook { get; } = new List<DetailBook>();
}
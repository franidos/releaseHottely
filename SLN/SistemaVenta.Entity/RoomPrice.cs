using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class RoomPrice
{
    public int IdRoomPrice { get; set; }
    public int IdEstablishment { get; set; }
    public int IdCategoria { get; set; }
    public decimal Monday { get; set; }
    public decimal Tuesday { get; set; }
    public decimal Wednesday { get; set; }
    public decimal Thursday { get; set; }
    public decimal Friday { get; set; }
    public decimal Saturday { get; set; }
    public decimal Sunday { get; set; }
    public string User { get; set; }
    public bool? IsActive { get; set; }
    public DateTime ModificationDate { get; set; }
    public DateTime? CreationDate { get; set; }
    public virtual Categoria? IdCategoriaNavigation { get; set; }
    public virtual Establishment IdEstablishmentNavigation { get; set; }

}

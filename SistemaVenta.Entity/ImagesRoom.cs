using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class ImagesRoom 
{
    public int IdImagesRoom { get; set; }
    public int IdRoom { get; set; }
    public int ImageNumber { get; set; }
    public string NameImage { get; set; }
    public string UrlImage { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual Room? IdRoomNavigation { get; set; }
}
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class ImagesEstablishment
{
    public int IdImagesEstablishment { get; set; }
    public int IdEstablishment { get; set; }
    public int ImageNumber { get; set; }
    public string NameImage { get; set; }
    public string UrlImage { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }
}
namespace SistemaVenta.Entity;

public partial class Producto
{
    public int IdProducto { get; set; }
    public int IdEstablishment { get; set; }

    public string? CodigoBarra { get; set; }

    public string? Marca { get; set; }

    public string? Descripcion { get; set; }

    public int? IdCategoriaProducto { get; set; }
    public int? IdProveedor { get; set; }

    public int? Stock { get; set; }

    public string? UrlImagen { get; set; }

    public string? NombreImagen { get; set; }
    public decimal? PrecioCompra { get; set; }
    public decimal? Imp { get; set; }

    public decimal? Precio { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }
    public virtual Establishment? IdEstablishmentNavigation { get; set; }

    public virtual ICollection<DetalleMovimiento> DetalleMovimiento { get; } = new List<DetalleMovimiento>();

    public virtual CategoriaProducto? IdCategoriaProductoNavigation { get; set; }
    public virtual Proveedor? IdProveedorNavigation { get; set; }

}

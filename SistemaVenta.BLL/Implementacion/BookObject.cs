using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class BookObject
    {
        public int? IdMovimiento { get; set; }
        public string? IdOrigin { get; set; }
        public string Reason { get; set; }
        public string Adults { get; set; }
        public string? AgeChildren { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public virtual ICollection<DetailBook>? DetailBook { get; set; } = new List<DetailBook>();
    }
}
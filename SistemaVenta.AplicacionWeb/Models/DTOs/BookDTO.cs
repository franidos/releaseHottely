using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class BookDTO
    {

        public int IdBook { get; set; }
        public int? IdMovimiento { get; set; }
        public string? IdOrigin { get; set; }
        public int? IdBookStatus { get; set; }
        public string StatusName { get; set; }
        public string statusBackground { get; set; }
        public string Reason { get; set; }
        public string Adults { get; set; }
        public string? AgeChildren { get; set; }
        public List<RoomDTO>? TempRoom { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdEstablishment { get; set; }
        public DateTime? CreationDate { get; set; }
        public virtual ICollection<DetailBookDTO>? DetailBook { get; set; }
        public virtual MovimientoDTO? IdMovimientoNavigation { get; set; }
        public virtual BookStatusDTO? IdBookStatusNavigation { get; set; }

    }
}

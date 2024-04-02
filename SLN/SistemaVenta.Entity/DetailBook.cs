using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class DetailBook
    {
        public int IdDetailBook { get; set; }
        public int IdBook { get; set; }
        //public int IdCategoria { get; set; }
        public int IdRoom { get; set; }
        public int IdGuest { get; set; }
        public decimal? Price { get; set; }
        public virtual Room? IdRoomNavigation { get; set; }
        public virtual Guest? IdGuestNavigation { get; set; }
        public virtual Book? IdBookNavigation { get; set; }
        public virtual Categoria? IdCategoriaNavigation { get; set; }
    }

}

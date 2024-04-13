namespace SistemaVenta.AplicacionWeb.Models
{
    public class BookEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        // Agrega otras propiedades necesarias para la vista del calendario
    }
}
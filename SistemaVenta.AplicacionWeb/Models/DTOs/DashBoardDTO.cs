namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class DashBoardDTO
    {
        public int TotalVentas { get; set; }
        public int TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalDisponibles { get; set; }
        public int? TotalOcupadas { get; set; }
        public int? TotalReservadas { get; set; }
        public int? TotalFueraDeServicio { get; set; }

        public List<MovimientosSemanaDTO> MovimientosUltimaSemana { get; set; }
        public List<ProductoSemanaDTO> ProductosTopUltimaSemana { get; set; }
        public List<MovimientosSemanaDTO> MovimientosHabitacionesUltimaSemana { get; set; }
        public List<ProductoSemanaDTO> HabitacionesTopUltimaSemana { get; set; }



    }
}

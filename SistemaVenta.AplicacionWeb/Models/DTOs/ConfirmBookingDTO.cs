using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class ConfirmBookingDTO
    {
        public EstablishmentDTO Negocio { get; set; }
        public BookDTO Book { get; set; }
        public RoomDTO Room { get; set; }
        public GuestDTO GuestMain { get; set; }

        //Campos cuerpo mail
        public string? UrlImageMainEstablishment { get; set; }
        public string? Tratamiento { get; set; }
        public int CantNoches { get; set; }
        public int CantAdultos { get; set; }
        public int CantNiños { get; set; }
        public string? EstatusBookDesc { get; set; }

    }
}

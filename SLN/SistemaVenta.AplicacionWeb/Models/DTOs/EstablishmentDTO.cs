namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class EstablishmentDTO
    {
        public int IdEstablishment { get; set; }
        public string NIT { get; set; }
        //public int IdCompany { get; set; }
        public string EstablishmentName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string? City { get; set; } //Ciudad establecimiento
        public string? Province { get; set; } //Dpto o provincia establecimiento
        public string? Country { get; set; } //Pais establecimiento
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Descripcion { get; set; }
        public string? Geolocation { get; set; }
        public string Token { get; set; }
        public string Rnt { get; set; }
        public string EstablishmentType { get; set; }
        public string Tax { get; set; }
        public string Currency { get; set; }
        //public int? IsActive { get; set; }
        public string? NameImage { get; set; }
        public string? UrlImage { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<ImagesEstablishmentDTO> ImagesEstablishment { get; set; }
    }

}

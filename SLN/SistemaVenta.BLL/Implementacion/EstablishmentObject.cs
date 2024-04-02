namespace SistemaVenta.BLL.Implementacion
{
    public class EstablishmentObject
    {
        public int IdEstablishment { get; set; }
        public string EstablishmentName { get; set; }
        public string? EstablishmentUrlImg { get; set; }
        public string EstablishmentAddress { get; set; }
        public string CheckInFormated { get; set; }
        public string CheckOutFormated { get; set; }

        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }

        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public double TotalDaysBooking { get; set; }
        public string TextTotalDays { get; set; }
        public string? NumRoom { get; set; }
        public string NumAdult { get; set; }
        public string NumChildren { get; set; }
        public string TextAdult { get; set; }
        public string TextChild { get; set; }
        public int TotalPeople { get; set; }
        public string TxtTotalPeople { get; set; }

    }
}
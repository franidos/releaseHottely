using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class BookingDetailResult
{
    public string CreationDate { get; set; }
    public string BookNumber { get; set; }
    public string DocumentType { get; set; }
    public string GuestDocument { get; set; }
    public string GuestName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalBook { get; set; }
    public string RoomNumber { get; set; }
    public string CheckIn { get; set; }
    public string CheckOut { get; set; }
    public string EstablishmentName { get; set; }
}


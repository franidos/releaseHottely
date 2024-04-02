using SistemaVenta.AplicacionWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;
public class RequestPricesBooking
{
    public string Adults { get; set; }
    public string AgeChildren { get; set; }
    public List<Room> TempRoom { get; set; }
    public string CheckIn { get; set; }
    public string CheckOut { get; set; }

}

//public class TempRoom
//{
//    public string NumberRoom { get; set; }
//    public int IdCategoria { get; set; }
//}



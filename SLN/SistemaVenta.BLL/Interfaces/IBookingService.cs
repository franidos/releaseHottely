using SistemaVenta.BLL.Implementacion;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<List<Establishment>> ObtainEstablishments(string busqueda);
        Task<List<Room>> ObtainRooms(string idEstablec, DateTime ci, DateTime co, bool ckeckDates = true);
       // Task<List<Room>> ObtainRooms3(string busqueda);
        Task<bool> CheckBookings(string document,DateTime ci, DateTime co,int idEstablishment);
        Task<Book> Save(Book entidad);
        Task<List<BookingDetailResult>> Reporte(string fechaInicio, string fechaFin, int idCompany);
        Task<List<Book>> History(string movementNumber, string fechaInicio, string fechaFin, int idCompany);
        Task<List<RoomsAndCategoryObject>> ObtainRoomsWithCategory(int idEstablecimiento, DateTime checkIn, DateTime checkOut);
        Task<List<Book>> GetBookById(int id);
        Task<List<Book>> GetBookingsByStatus(int idBookStatus, int idEstablec);
        Task<Book> Editar(Book entidad, bool editCheckIn, bool editCheckOut);
    }
}

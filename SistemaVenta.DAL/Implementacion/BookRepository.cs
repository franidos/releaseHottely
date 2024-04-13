using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Implementacion
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly DbventaContext _dbContext;

        public BookRepository(DbventaContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CheckBookings(string identification, DateTime checkIn, DateTime checkOut, int idEstablishment)
        {
            try
            {
                var hasBookings = _dbContext.Guests
                .Where(g => g.Document == identification)
                .SelectMany(g => g.DetailBook)
                .Select(d => d.IdBookNavigation)
                .Any(b => b.CheckIn <= checkOut && b.CheckOut >= checkIn && b.IdEstablishment == idEstablishment);

                return hasBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public async Task<Book> Registrar(Book entidad)
        {
            Book bookGenerada = new Book();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var tipoDoc = string.Empty;
                    List<int> distinctRoomIds = entidad.DetailBook.Select(r => r.IdRoom).Distinct().ToList();

                    //foreach (int itemDV in distinctRoomIds)
                    //{
                    //    Room roomEncontrado = new Room();
                    //    roomEncontrado = _dbContext.Rooms.Where(p => p.IdRoom == itemDV).First();

                    //    if (roomEncontrado.Status == true)
                    //    {
                    //        roomEncontrado.Status = false;
                    //    }

                    //    _dbContext.Rooms.Update(roomEncontrado);
                    //}

                    //await _dbContext.SaveChangesAsync();

                    //entidad.CreationDate = DateTime.Now;
                    await _dbContext.Books.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    bookGenerada = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return bookGenerada;
        }

        public async Task<List<BookingDetailResult>> Reporte(DateTime fechaIni, DateTime fechaFin, int idCompany)
        {
            IEnumerable<BookingDetailResult> bookingResult = _dbContext.GetBookingsByDateRange(fechaIni, fechaFin, idCompany);
            return bookingResult.ToList();
        }

    }
}

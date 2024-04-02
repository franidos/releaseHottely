using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IBookRepository: IGenericRepository<Book>
    {
        Task<bool> CheckBookings(string document, DateTime ci, DateTime co, int idEstablishment);
        Task<Book> Registrar(Book entidad);
        Task<List<BookingDetailResult>> Reporte(DateTime fechaIni, DateTime fechaFin, int idCompany);
    }
}

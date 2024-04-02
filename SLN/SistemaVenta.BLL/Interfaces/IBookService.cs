using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> Listar();
        Task<List<Book>> ListByRoom(int idRoom, int idEstablec, DateTime dateLimit);
        Task<List<BookStatus>> GetBookStatusList();
        Task<Book> Crear(Book entidad);
        Task<Book> Editar(Book entidad);
        Task<bool> Eliminar(int idBook);
    }
}

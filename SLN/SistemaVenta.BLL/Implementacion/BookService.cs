using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SistemaVenta.BLL.Implementacion
{

    public class BookService : IBookService
    {
       // public readonly DbventaContext _dbContext; // Reemplaza "YourDbContext" con el nombre de tu contexto de base de datos
        private readonly IGenericRepository<Book> _bookRepositorio;
        private readonly IGenericRepository<BookStatus> _bookStatusRepositorio;

        public BookService(DbventaContext dbContext, IGenericRepository<Book> bookRepositorio, IGenericRepository<BookStatus> bookStatusRepositorio)
        {
            _bookRepositorio = bookRepositorio;
            _bookStatusRepositorio = bookStatusRepositorio;
        }

        public async Task<List<Book>> Listar()
        {
            IQueryable<Book> query = await _bookRepositorio.Consultar();
            return query.Include(d => d.IdMovimientoNavigation).Include(t => t.IdBookStatusNavigation).ToList();
        }

        public async Task<List<Book>> ListByRoom(int idRoom, int idEstablec, DateTime dateLimit)
        {
            IQueryable<Book> query = await _bookRepositorio.Consultar(p => p.IdEstablishment ==idEstablec && p.DetailBook.Any(
                                                                        d => d.IdBookNavigation.CheckIn >= dateLimit && 
                                                                        d.IdRoom == idRoom));

            return query.Include(d => d.IdMovimientoNavigation).Include(t => t.DetailBook).Include(t => t.IdBookStatusNavigation).ToList();
        }

        public async Task<List<BookStatus>> GetBookStatusList()
        {
            IQueryable<BookStatus> query = await _bookStatusRepositorio.Consultar();
            return query.ToList();
        }
        

        public async Task<Book> Crear(Book entidad)
        {
            try
            {

                Book book_creado = await _bookRepositorio.Crear(entidad);

                if (book_creado.IdBook == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la reserva");
                }

                IQueryable<Book> query = await _bookRepositorio.Consultar(p => p.IdBook == book_creado.IdBook);
                book_creado = query.First();
                return book_creado;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<Book> Editar(Book entidad)
        {
            try
            {

                bool respuesta = await _bookRepositorio.Editar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la Reserva");
                }

                IQueryable<Book> queryGuest = await _bookRepositorio.Consultar(p => p.IdBook == entidad.IdBook);
                Book book_editado = queryGuest.First();
                return book_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idBook)
        {
            try
            {
                Book book_encontrado = await _bookRepositorio.Obtener(p => p.IdBook == idBook);
                if (book_encontrado == null)
                {
                    throw new TaskCanceledException("El huesped no existe");
                }

                bool respuesta = await _bookRepositorio.Eliminar(book_encontrado);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }


}

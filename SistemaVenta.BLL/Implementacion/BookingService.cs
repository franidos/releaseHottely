using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Establishment> _repositorioEstablishment;
        private readonly IGenericRepository<Room> _repositorioRoom;
        private readonly IGenericRepository<Movimiento> _repositorioMovement;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IBookRepository _repositorioBooking;
        private readonly IPriceService _priceService;
        private readonly IGenericRepository<Book> _repositorioBook;
        public readonly DbventaContext _dbContext;

        // private readonly IMovimientoRepository _repositorioMovimiento;
        public BookingService(IGenericRepository<Establishment> repositorioEstablishment, IGenericRepository<Room> repositorioRoom,
            IBookRepository repositorioBooking, IGenericRepository<Movimiento> repositorioMovement, IMovimientoRepository repositorioMovimiento,
            IPriceService priceService, IGenericRepository<Book> repositorioBook, DbventaContext dbContext)
        {
            _repositorioEstablishment = repositorioEstablishment;
            _repositorioRoom = repositorioRoom;
            _repositorioMovement = repositorioMovement;
            _repositorioBooking = repositorioBooking;
            _priceService = priceService;
            _repositorioBook = repositorioBook;
            _dbContext = dbContext;
        }

        public async Task<List<Establishment>> ObtainEstablishments(string busqueda)
        {
            IQueryable<Establishment> query = await _repositorioEstablishment.Consultar(//todo mover a Establishments
                p => //p.IsActive == true &&
                string.Concat(p.NIT, p.EstablishmentName, p.Contact).Contains(busqueda)
                );

            return query.ToList();
        }

        public async Task<List<Room>> ObtainRooms(string idEstablec, DateTime checkIn, DateTime checkOut, bool ckeckDates = true)
        {
            var rooms = ckeckDates ?
                await _repositorioRoom.Consultar(p => //todo mover a Rooms
                        p.IsActive == true &&
                        p.IdEstablishment == Int32.Parse(idEstablec) &&
                        !p.DetailBook.Any(d =>
                            (checkIn >= d.IdBookNavigation.CheckIn && checkIn < d.IdBookNavigation.CheckOut) ||
                            (checkOut > d.IdBookNavigation.CheckIn && checkOut <= d.IdBookNavigation.CheckOut) ||
                            (checkIn <= d.IdBookNavigation.CheckIn && checkOut >= d.IdBookNavigation.CheckOut)))
                :
                await _repositorioRoom.Consultar(p => p.IsActive == true && p.IdEstablishment == Int32.Parse(idEstablec));


            return rooms.ToList();
        }

        // Consultar las habitaciones disponibles Todo, mover a rooms
        public async Task<List<RoomsAndCategoryObject>> ObtainRoomsWithCategory(int idEstablecimiento, DateTime checkIn, DateTime checkOut)
        {
            var queryRooms = await _repositorioRoom.Consultar(r =>
                    r.IsActive == true &&
                    //r.Status != "RESERVADO" &&
                    r.IdEstablishment == idEstablecimiento &&
                    !r.DetailBook.Any(d =>
                            (checkIn >= d.IdBookNavigation.CheckIn && checkIn < d.IdBookNavigation.CheckOut) ||
                            (checkOut > d.IdBookNavigation.CheckIn && checkOut <= d.IdBookNavigation.CheckOut) ||
                            (checkOut <= d.IdBookNavigation.CheckIn && checkOut >= d.IdBookNavigation.CheckOut)));

            var rooms = await queryRooms
                .Include(r => r.IdCategoriaNavigation)
                .ToListAsync();

            var listQueryRooms = queryRooms.ToList();

            List<string> myList = new List<string>();
            List<Room> originalList = rooms; // Obtén la lista original de alguna manera
            List<RoomsAndCategoryObject> newList = new List<RoomsAndCategoryObject>();

            foreach (Room room in originalList)
            {

                var idCategory = room.IdCategoria;

                // Obtener el precio de la habitacion
                decimal? priceByDate = await _priceService.GetPriceForDate(checkIn, idCategory, idEstablecimiento);
                if (priceByDate == null)
                {
                    priceByDate = 0;
                }
                else
                {
                    priceByDate = Math.Round((decimal)priceByDate, 2);
                }

                var imagesList = await _dbContext.ImagesRoom
                    .Where(ir => ir.IdRoom == room.IdRoom)
                    .ToListAsync();

                RoomsAndCategoryObject customObject = new RoomsAndCategoryObject
                {
                    IdRoom = room.IdRoom,
                    IdEstablishment = room.IdEstablishment,
                    FkCategoria = room.IdCategoria,
                    Number = room.Number,
                    Title = room.RoomTitle,
                    Info = room.Description,
                    Capacity = room.Capacity,
                    Size = room.SizeRoom,
                    // UrlImage = room.UrlImage,
                    Price = priceByDate,
                    //Status = room.Status,
                    IsActive = room.IsActive,

                    CategoryName = room.CategoryName,
                    RoomTypeInfo = room.IdCategoriaNavigation.Descripcion,
                    RoomTypeStatus = room.IdCategoriaNavigation.EsActivo,
                    RoomImages = imagesList.OrderBy(x=>x.ImageNumber).ToList(),
                };

                newList.Add(customObject);
            }

            var list = newList;

            return newList;
        }

        public async Task<bool> CheckBookings(string document, DateTime ci, DateTime co, int idEstablishment)
        {
            try
            {
                return await _repositorioBooking.CheckBookings(document, ci, co, idEstablishment);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Book> Save(Book entidad)
        {

            try
            {
                return await _repositorioBooking.Registrar(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BookingDetailResult>> Reporte(string fechaInicio, string fechaFin, int idCompany)
        {
            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

            List<BookingDetailResult> lista = await _repositorioBooking.Reporte(fecha_inicio, fecha_fin, idCompany);
            return lista;

        }

        public async Task<List<Book>> History(string movementNumber, string fechaInicio, string fechaFin, int idCompany)
        {
            try
            {

                IQueryable<Book> query = await _repositorioBooking.Consultar();
                fechaInicio = fechaInicio is null ? "" : fechaInicio;
                fechaFin = fechaFin is null ? "" : fechaFin;
                Movimiento movement_found = await _repositorioMovement.Obtener(n => n.NumeroMovimiento == movementNumber);

                if (fechaInicio != "" && fechaFin != "")
                {
                    DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                    DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                    return query.Where(v =>
                            v.CreationDate.Date >= fecha_inicio.Date &&
                            v.CreationDate.Date <= fecha_fin.Date
                        )
                        .Include(tDoc => tDoc.IdMovimientoNavigation)
                        //.Include(usu => usu.IdUsuarioNavigation)
                        .Include(det => det.DetailBook)
                        .ToList();
                }
                else
                {
                    return query.Where(v => v.IdMovimiento == movement_found.IdMovimiento)
                        .Include(tDoc => tDoc.IdMovimientoNavigation)
                        .Include(det => det.DetailBook)
                       .ToList();
                }

            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<List<Book>> GetBookById(int id)
        {
            try
            {
                IQueryable<Book> book_found = await _repositorioBooking.Consultar(c => c.IdBook == id);
                return book_found.Include(mov => mov.IdMovimientoNavigation)
                                 .Include(det => det.DetailBook)
                                 .Include(est => est.IdBookStatusNavigation).ToList();

            }
            catch (Exception e) 
            {
                throw;
            }
        }

        public async Task<List<Book>> GetBookingsByStatus(int idBookStatus, int idEstablec)
        {
            try
            {
                IQueryable<Book> book_found = await _repositorioBooking.Consultar(c => c.IdBookStatus == idBookStatus && c.IdEstablishment == idEstablec);
                return book_found.Include(mov => mov.IdMovimientoNavigation)
                                 .Include(det => det.DetailBook)
                                 .Include(est => est.IdBookStatusNavigation).ToList();

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Book> Editar(Book entidad, bool editCheckIn, bool editCheckOut)
        {
            try
            {
                Book registro = await _repositorioBook.Obtener(c => c.IdBook == entidad.IdBook);
                registro.IdBookStatus = entidad.IdBookStatus == null ? registro.IdBookStatus : entidad.IdBookStatus;
                registro.Reason = string.IsNullOrEmpty(entidad.Reason) ? registro.Reason : entidad.Reason;
                registro.Adults = string.IsNullOrEmpty(entidad.Adults) ? registro.Adults : entidad.Adults;
                registro.CheckIn = editCheckIn ? entidad.CheckIn : registro.CheckIn;
                registro.CheckOut = editCheckOut ? entidad.CheckOut : registro.CheckOut;
                bool respuesta = await _repositorioBook.Editar(registro);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Movimiento");
                }

                return registro;
            }
            catch (Exception)
            {

                throw;
            }
        }



    }
}

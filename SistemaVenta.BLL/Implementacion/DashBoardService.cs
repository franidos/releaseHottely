using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IMovimientoRepository _repositorioMovimiento;
        private readonly IGenericRepository<DetalleMovimiento> _repositorioDetalleMovimiento;
        private readonly IGenericRepository<DetailBook> _repositorioDetailBook;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IGenericRepository<CategoriaProducto> _repositorioCategoriaProducto;
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IGenericRepository<Room> _repositorioRoom;
        private readonly IGenericRepository<Book> _repositorioBook;
        private readonly IGenericRepository<DetalleCaja> _repositorioDetailCaja;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardService(IMovimientoRepository repositorioMovimiento, 
            IGenericRepository<DetalleMovimiento> repositorioDetalleMovimiento,
             IGenericRepository<DetailBook> repositorioDetailBook,
            IGenericRepository<CategoriaProducto> repositorioCategoriaProducto, 
            IGenericRepository<Producto> repositorioProducto,
            IGenericRepository<Room> repositorioRoom,
            IGenericRepository<Book> repositorioBook,
            IGenericRepository<DetalleCaja> repositorioDetailCaja)
        {
            _repositorioMovimiento = repositorioMovimiento;
            _repositorioDetalleMovimiento = repositorioDetalleMovimiento;
            _repositorioDetailBook = repositorioDetailBook;
            _repositorioCategoriaProducto = repositorioCategoriaProducto;
            _repositorioProducto = repositorioProducto;
            _repositorioRoom = repositorioRoom;
            _repositorioBook = repositorioBook;
            _repositorioDetailCaja = repositorioDetailCaja;
            FechaInicio = FechaInicio.AddDays(-7);
        }

        public async Task<int> TotalHabitacionesDisponibles(int idEstabl)
        {
            int countAvailable = 0;

            //Traer todas habitaciones
            IQueryable<Room> rooms = await _repositorioRoom.Consultar(v => v.IdEstablishment == idEstabl && v.IdRoomStatus != 6); //6=Fuera de servicio

            //Traer reservas realizadas y que no estén disponibles hoy
            IQueryable<DetailBook> noDisponible = await _repositorioDetailBook.Consultar(v => v.IdBookNavigation.IdEstablishment == idEstabl &&
                                                                                        (v.IdBookNavigation.IdBookStatus.Value == 1 || v.IdBookNavigation.IdBookStatus.Value == 2 ||
                                                                                        v.IdBookNavigation.IdBookStatus.Value == 3 || v.IdBookNavigation.IdBookStatus.Value == 4) &&
                                                                                        DateTime.Today >= v.IdBookNavigation.CheckIn.Date && 
                                                                                        DateTime.Today <= v.IdBookNavigation.CheckOut.Date);
            //Validar habitaciones disponibles
            foreach (var item in rooms.ToList())            
                if (!noDisponible.Any(x => x.IdRoom == item.IdRoom)) countAvailable++;

            return countAvailable;
        }
        public async Task<int> TotalHabitacionesOcupadas(int idEstabl)
        {
            IQueryable<Book> query = await _repositorioBook.Consultar(v => v.IdBookStatus == 4 && v.IdEstablishment == idEstabl); //4=Ingreso
            return query.Count();
        }
        public async Task<int> TotalHabitacionesReservadas(int idEstabl)
        {
            IQueryable<Book> query = await _repositorioBook.Consultar(v => (v.IdBookStatus == 1 || v.IdBookStatus == 2 || v.IdBookStatus == 3) && v.IdEstablishment == idEstabl); //1=Reserva Sin Confirmar, 2=Reserva Confirmada,3=Reserva Parcial
            return query.Count();
        }
        public async Task<int> TotalHabitacionesFueraDeServicio(int idEstabl) 
        {
            IQueryable<Room> query = await _repositorioRoom.Consultar(v => v.IdRoomStatus == 6 && v.IdEstablishment == idEstabl); //6=Fuera de servicio
            return query.Count();
        }
        public async Task<int> TotalVentasCajaxFecha(int idEstabl, DateTime? fechaInicio)
        {
            try
            {
                IQueryable<DetalleCaja> query = await _repositorioDetailCaja.Consultar(v => v.IdCajaNavigation.IdEstablishment == idEstabl && v.IdMovimiento != null && v.IdMovimientoNavigation.FechaRegistro.Value.Date >= (fechaInicio != null ? fechaInicio : FechaInicio.Date));
                int resultado = Convert.ToInt32(query.Select(v => v.Valor).Sum(v => v));
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> TotalIngresoxFecha(int idEstabl, DateTime? fechaInicio)
        {
            try
            {
                IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(v => v.IdUsuarioNavigation.IdEstablishment == idEstabl && v.FechaRegistro.Value.Date >= (fechaInicio != null ? fechaInicio : FechaInicio.Date));
                int resultado = Convert.ToInt32(query.Select(v => v.Total).Sum(v => v.Value));
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> TotalProductos(int idEstabl)
        {
            try
            {
                IQueryable<Producto> query = await _repositorioProducto.Consultar(x=>x.IdEstablishment == idEstabl);
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> TotalHabitaciones(int idEstabl)
        {
            try
            {
                IQueryable<Room> query = await _repositorioRoom.Consultar(x => x.IdEstablishment == idEstabl);
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> TotalCategoriasProductos(int idEstabl)
        {
            try
            {
                IQueryable<CategoriaProducto> query = await _repositorioCategoriaProducto.Consultar(x => x.IdEstablishment == idEstabl);
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Dictionary<string, int>> MovimientosUltimaSemana(int idEstabl)
        {
            try
            {
                IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(v => v.IdUsuarioNavigation.IdEstablishment == idEstabl && v.FechaRegistro.Value.Date >= FechaInicio.Date);

                Dictionary<string, int> respuesta = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(d => new { fecha = d.Key.ToString("dd/MM/yyyy"), total = d.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r =>r.total);


                Dictionary<string, int> respuesta2 = (from v in query group v by v.FechaRegistro.Value.Date 
                                                      into r select new
                                                     {
                                                         fecha = r.Key.ToString("dd/MM/yyyy"),
                                                         total = r.Count()
                                                     }).ToDictionary(keySelector: r => r.fecha,elementSelector: r => r.total);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Dictionary<string, int>> HabitacionesTopUltimaSemana(int idEstabl)
        {
            try
            {
                IQueryable<DetailBook> query = await _repositorioDetailBook.Consultar(v=> v.IdBookNavigation.IdEstablishment == idEstabl && v.IdBookNavigation.CreationDate.Date >= FechaInicio.Date);
                Dictionary<string, int> respuesta = query
                    .Include(p => p.IdRoomNavigation)
                    .Include(p => p.IdBookNavigation)
                    .GroupBy(dv => dv.IdRoomNavigation.Number + "-" + dv.IdRoomNavigation.CategoryName).OrderBy(g => g.Count())
                    .Select(d => new { room = d.Key, total = d.Count() }).Take(5)
                    .ToDictionary(keySelector: r => r.room, elementSelector: r => r.total);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

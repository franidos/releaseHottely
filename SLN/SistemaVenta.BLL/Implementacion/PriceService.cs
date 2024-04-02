using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{

    public class PriceService : IPriceService
    {
        private readonly IGenericRepository<RoomPrice> _roomPriceRepository;
        private readonly IGenericRepository<Holiday> _holidayRepository;
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<Room> _roomRepository;
        // private readonly IGenericRepository<Categoria> _categoriaRepository;

        public PriceService(
            IGenericRepository<RoomPrice> roomPriceRepository,
            IGenericRepository<Holiday> holidayRepository,
            IGenericRepository<Season> seasonRepository,
            IGenericRepository<Room> roomRepository)
        {
            _roomPriceRepository = roomPriceRepository;
            _holidayRepository = holidayRepository;
            _seasonRepository = seasonRepository;
            _roomRepository = roomRepository;   
        }
        public async Task<List<RoomPrice>> Lista(int idEstabl)
        {

            IQueryable<RoomPrice> query = await _roomPriceRepository.Consultar(x=>x.IdEstablishment == idEstabl);
            return query.Include(tDoc => tDoc.IdCategoriaNavigation).ToList();

        }
        public async Task<List<Season>> ListSeasons(int idEstabl)
        {
            IQueryable<Season> query = await _seasonRepository.Consultar(x => x.IdEstablishment == idEstabl);
            return query.ToList();
        }
        public async Task<List<Holiday>> ListHolidays(int idEstabl)
        {
            IQueryable<Holiday> query = await _holidayRepository.Consultar(x => x.IdEstablishment == idEstabl);
            return query.ToList();
        }

        //public async Task<decimal> CalculateTotalPrice(Book book, List<DetailBook> details, ResponseBookingDTO data)
        //{
        //    decimal totalPrice = 0;

        //    foreach (var detail in details)
        //    {
        //        Room selectedRoom = await _roomRepository.Obtener(c => c.IdRoom == detail.RoomId);

        //        List<DateTime> datesInRange = GetDatesInRange(book.CheckIn, book.CheckOut);

        //        foreach (DateTime date in datesInRange)
        //        {
        //            decimal priceForDate = await GetPriceForDate(date, selectedRoom.IdCategoria);

        //            // Obtener las edades de los huéspedes asociados a este detalle
        //            var guestAges = data.Guests.Where(g => g.IdGuest == detail.IdGuest).Select(g => g.Age).ToList();

        //            // Calcular el precio con el descuento aplicado para cada huésped y sumarlos al total
        //            foreach (var age in guestAges)
        //            {
        //                decimal discountedPrice = ApplyAgeDiscount(age, priceForDate);
        //                totalPrice += discountedPrice;
        //            }
        //        }
        //    }

        //    return totalPrice;
        //}

        public async Task<decimal> CalculateTotalPrice(List<Room> tempRooms, DateTime CheckIn, DateTime CheckOut, string Adults, string AgeChildren)
        {
            decimal totalPrice = 0;

            foreach (var tempRoom in tempRooms)
            {
                foreach (var date in GetDatesInRange(CheckIn,CheckOut))
                {

                    Room selectedRoom = await _roomRepository.Obtener(c => c.IdRoom == tempRoom.IdRoom);

                    var priceForDate = await GetPriceForDate(date, selectedRoom.IdCategoria, selectedRoom.IdEstablishment);
                    totalPrice += priceForDate;
                }
            }

            return totalPrice;
        }

        private List<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate)
        {
            List<DateTime> datesInRange = new List<DateTime>();
            datesInRange.Add(startDate);
            DateTime currentDate = startDate.AddDays(1);
            while (currentDate < endDate)
            {
                datesInRange.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }
            datesInRange.Add(endDate);

            return datesInRange;
        }


        public decimal ApplyAgeDiscount(int age, decimal originalPrice)
        {
            // Definir las edades de referencia para aplicar el descuento
            int childAgeLimit = 12;
            int infantAgeLimit = 3;

            decimal discountPercentage = 0.3m; // 30% de descuento para niños mayores de 3 años y menores de 12 años

            if (age >= infantAgeLimit && age < childAgeLimit)
            {
                // Aplicar el descuento para niños
                return originalPrice * (1 - discountPercentage);
            }

            // No aplicar descuento para adultos o niños mayores de 12 años
            return originalPrice;
        }


        // Método para obtener todas las fechas dentro de un rango
        //private List<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate)
        //{
        //    List<DateTime> datesInRange = new List<DateTime>();

        //    // Agregar la fecha de inicio al listado
        //    datesInRange.Add(startDate);

        //    // Calcular las fechas intermedias dentro del rango
        //    DateTime currentDate = startDate.AddDays(1);
        //    while (currentDate < endDate)
        //    {
        //        datesInRange.Add(currentDate);
        //        currentDate = currentDate.AddDays(1);
        //    }

        //    // Agregar la fecha de fin al listado
        //    datesInRange.Add(endDate);

        //    return datesInRange;
        //}

        public async Task<decimal> GetPriceForDate(DateTime date, int IdCategory, int idEstabl)
        {
            RoomPrice roomPrice = await _roomPriceRepository.Obtener(c => c.IdEstablishment == idEstabl && c.IdCategoria == IdCategory);

            string dayOfWeek = date.ToString("dddd", CultureInfo.InvariantCulture).ToLower();

            decimal price = 0;
            switch (dayOfWeek)
            {
                case "monday":
                    price = roomPrice.Monday;
                    break;
                case "tuesday":
                    price = roomPrice.Tuesday;
                    break;
                case "wednesday":
                    price = roomPrice.Wednesday;
                    break;
                case "thursday":
                    price = roomPrice.Thursday;
                    break;
                case "friday":
                    price = roomPrice.Friday;
                    break;
                case "saturday":
                    price = roomPrice.Saturday;
                    break;
                case "sunday":
                    price = roomPrice.Sunday;
                    break;
                default:
                    // Puedes lanzar una excepción o establecer un valor predeterminado si no se encuentra el día de la semana.
                    // Aquí he establecido el precio en 0, pero debes ajustarlo según tus necesidades.
                    price = 0;
                    break;
            }

            Holiday holiday = await _holidayRepository.Obtener(c => c.Date == date);
            Season season = await _seasonRepository.Obtener(c => c.Date == date);

            if (holiday != null)
            {
                return price * (1 + holiday.Increment);
            }
            else if (season != null)
            {
                return price * (1 + season.Increment);
            }
            else
            {
                return price;
            }
        }


        //public async Task<decimal> GetPriceForDate(DateTime date) 
        //{
        //    RoomPrice roomPrice = await _roomPriceRepository.Obtener(c => c.DayOfTheWeek == date.DayOfWeek);
        //    Holiday holiday = await _holidayRepository.Obtener(c => c.Date == date);
        //    Season season = await _seasonRepository.Obtener(c => c.Date == date);

        //    if (holiday != null)
        //    {
        //        return roomPrice.Price * (1 + holiday.Increment);
        //    }
        //    else if (season != null)
        //    {
        //        return roomPrice.Price * (1 + season.Increment);
        //    }
        //    else
        //    {
        //        return roomPrice.Price;
        //    }
        //}
        public async Task<RoomPrice> Crear(RoomPrice entidad)
        {
            try
            {
                RoomPrice price_creada = await _roomPriceRepository.Crear(entidad);
                if (price_creada.IdRoomPrice == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el precio");
                }
                return price_creada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<RoomPrice> Editar(RoomPrice entidad)
        {
            try
            {
                RoomPrice price_encontrada = await _roomPriceRepository.Obtener(c => c.IdRoomPrice == entidad.IdRoomPrice);
                price_encontrada.Monday = entidad.Monday;
                price_encontrada.Tuesday = entidad.Tuesday;
                price_encontrada.Wednesday = entidad.Wednesday;
                price_encontrada.Thursday = entidad.Thursday;
                price_encontrada.Friday = entidad.Friday;
                price_encontrada.Saturday = entidad.Saturday;
                price_encontrada.Sunday = entidad.Sunday;
                price_encontrada.IsActive = entidad.IsActive;

                bool respuesta = await _roomPriceRepository.Editar(price_encontrada);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el precio");
                }

                return price_encontrada;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //public async Task<RoomPrice> Editar(RoomPrice entidad)
        //{
        //    try
        //    {
        //        RoomPrice price_encontrada = await _roomPriceRepository.Obtener(c => c.IdRoomPrice == entidad.IdRoomPrice);
        //        price_encontrada.Name = entidad.Name;
        //        price_encontrada.IsActive = entidad.IsActive;

        //        bool respuesta = await _roomPriceRepository.Editar(price_encontrada);

        //        if (!respuesta)
        //        {
        //            throw new TaskCanceledException("No se pudo editar el precio");
        //        }

        //        return price_encontrada;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<bool> Eliminar(int idPrice)
        {
            try
            {
                RoomPrice price_encontrada = await _roomPriceRepository.Obtener(c => c.IdRoomPrice == idPrice);

                if (price_encontrada == null)
                {
                    throw new TaskCanceledException("El Precio no existe");
                }
                bool respuesta = await _roomPriceRepository.Eliminar(price_encontrada);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Season> CreateSeason(Season entidad)
        {
            try
            {
                Season season_created = await _seasonRepository.Crear(entidad);
                if (season_created.IdSeason == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el precio");
                }
                return season_created;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Season> EditSeason(Season entidad)
        {
            try
            {
                Season season_found = await _seasonRepository.Obtener(c => c.IdSeason == entidad.IdSeason);
                season_found.Name = entidad.Name;
                season_found.DayOfTheWeek = entidad.DayOfTheWeek;
                season_found.Date = entidad.Date;
                season_found.Increment = entidad.Increment;
                season_found.User = entidad.User;
                season_found.IsActive = entidad.IsActive;

                bool respuesta = await _seasonRepository.Editar(season_found);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el precio");
                }

                return season_found;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSeason(int idSeason)
        {
            try
            {
                Season price_encontrada = await _seasonRepository.Obtener(c => c.IdSeason == idSeason);

                if (price_encontrada == null)
                {
                    throw new TaskCanceledException("El dia de temporada no existe");
                }
                bool respuesta = await _seasonRepository.Eliminar(price_encontrada);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Holiday> CreateHoliday(Holiday entidad)
        {
            try
            {
                Holiday holiday_created = await _holidayRepository.Crear(entidad);
                if (holiday_created.IdHoliday == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el precio");
                }
                return holiday_created;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Holiday> EditHoliday(Holiday entidad)
        {
            try
            {
                Holiday holiday_found = await _holidayRepository.Obtener(c => c.IdHoliday == entidad.IdHoliday);
                holiday_found.Name = entidad.Name;
                holiday_found.DayOfTheWeek = entidad.DayOfTheWeek;
                holiday_found.Date = entidad.Date;
                holiday_found.Increment = entidad.Increment;
                holiday_found.User = entidad.User;
                holiday_found.IsActive = entidad.IsActive;

                bool respuesta = await _holidayRepository.Editar(holiday_found);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el precio de festivo");
                }

                return holiday_found;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteHoliday(int idSeason)
        {
            try
            {
                Holiday holiday_found = await _holidayRepository.Obtener(c => c.IdHoliday == idSeason);

                if (holiday_found == null)
                {
                    throw new TaskCanceledException("El dia de temporada no existe");
                }
                bool respuesta = await _holidayRepository.Eliminar(holiday_found);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

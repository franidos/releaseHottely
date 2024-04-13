using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IPriceService
    {
        //Task<decimal>  CalculateTotalPrice(List<Guest> guests, List<DetailBook> details, Book book);
        Task<decimal> CalculateTotalPrice(List<Room> tempRooms, DateTime CheckIn, DateTime CheckOut, string Adults, string AgeChildren);
        Task<decimal> GetPriceForDate(DateTime date, int IdCategory, int idEstabl);
        Task<List<RoomPrice>> Lista(int idEstabl);
        Task<RoomPrice> Crear(RoomPrice entidad);
        Task<RoomPrice> Editar(RoomPrice entidad);
        Task<bool> Eliminar(int idCategoria);
        Task<Season> CreateSeason(Season entidad);
        Task<Season> EditSeason(Season entidad);
        Task<bool> DeleteSeason(int idCategoria);
        Task<Holiday> CreateHoliday(Holiday entidad);
        Task<Holiday> EditHoliday(Holiday entidad);
        Task<bool> DeleteHoliday(int idCategoria);
        Task<List<Season>> ListSeasons(int idEstabl);
        Task<List<Holiday>> ListHolidays(int idEstabl);
    }
}

using SistemaVenta.Entity;
using Microsoft.AspNetCore.Http;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IImageService
    {
        Task HandleImages(List<IFormFile> newImages, List<string> oldImagesUrl, List<string> imageIds, Establishment establishment);
        Task HandleRoomImages(List<IFormFile> newImages, List<string> oldImagesUrl, List<string> imageIds, Room establishment);
        Task<ImagesEstablishment> CreateImages(ImagesEstablishment entidad, Stream imagen = null);
        Task<List<ImagesEstablishment>> ListImagesByEstablishment(int idEstablishment);
        Task<ImagesEstablishment> GetSingleImage(int idEstablishment, int idOrder);
        Task<ImagesEstablishment> EditarImagenes(ImagesEstablishment entidad);
        Task<ImagesRoom> CreateImages(ImagesRoom entidad, Stream imagen = null);
        Task<List<ImagesRoom>> ListImagesByRoom(int idRoom);

    }
}

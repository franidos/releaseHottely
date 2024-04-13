using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.BLL.Implementacion
{
    public class ImageService : IImageService
    {
        private readonly IEstablishmentService _establishmentService;
        //private readonly IRoomService _roomService;

        private readonly IGenericRepository<ImagesEstablishment> _repositorioImagesEstablishment;
        private readonly IGenericRepository<ImagesRoom> _repositorioImagesRoom;
        private readonly IFireBaseService _fireBaseService;

        private readonly IFireBaseService _firebaseSerice;
        //  private readonly IMapper _mapper;

        public ImageService(IEstablishmentService establishmentService,
                            IGenericRepository<ImagesEstablishment> repositorioImagesEstablishment,
                            IGenericRepository<ImagesRoom> repositorioImagesRoom,
                            IFireBaseService firebaseSerice,
                            IFireBaseService fireBaseService)
        {
            _establishmentService = establishmentService;
            _repositorioImagesEstablishment = repositorioImagesEstablishment;
            _repositorioImagesRoom = repositorioImagesRoom;
            _firebaseSerice = firebaseSerice;
            _fireBaseService = fireBaseService;
            // _mapper = mapper;
        }

        public async Task HandleRoomImages(
            List<IFormFile> newImages,
            List<string> allImagesUrl,
            List<string> imageIds,
            Room room)
        {
            List<ImagesRoom> img_room_found = await ListImagesByRoom(room.IdRoom);
            List<ImagesRoom> originals_images = img_room_found.ToList();

            var counter = 0;

            for (int i = 0; i < imageIds.Count; i++)
            {
                ImagesRoom imagesRoom = new ImagesRoom();
                ImagesOrderViewModel listOrder = JsonConvert.DeserializeObject<ImagesOrderViewModel>(imageIds[i]);

                var current = listOrder.currentOrd;
                var ini = listOrder.iniOrd;

                if (!int.TryParse(ini, out int result)) // insert / delete image (olds)
                {
                    if (ini == "to-del")
                    {
                        ImagesRoom image_found = originals_images[i];
                        if (image_found != null)
                        {
                            var respuesta = await EliminarImagen(image_found);

                            if (respuesta)
                            {
                                await _fireBaseService.EliminarStorage("carpeta_room", image_found.NameImage);
                            }

                        }
                    }
                    else
                    {
                        var image = newImages[counter];

                        if (image != null)
                        {
                            imagesRoom.ImageNumber = i + 1;
                            imagesRoom.IdRoom = room.IdRoom;
                            string nombre_en_codigo = Guid.NewGuid().ToString("N");
                            string extension = Path.GetExtension(image.FileName);
                            imagesRoom.NameImage = string.Concat(nombre_en_codigo, extension);
                            Stream streamImagen = image.OpenReadStream();

                            ImagesRoom image_created = await CreateImages(imagesRoom, streamImagen);
                        }
                        counter++;
                    }
                }
                else if (current != Int32.Parse(ini)) // edit reg image (old)
                {
                    ImagesRoom image_found = originals_images.Find(c => c.ImageNumber == Int32.Parse(ini));
                    image_found.ImageNumber = current;

                    ImagesRoom imagen_editada = await EditarRoomImagenes(image_found);
                }

            }
        }

        public async Task HandleImages(
            List<IFormFile> newImages,
            List<string> allImagesUrl,
            List<string> imageIds,
            Establishment establishment)
        {
            List<ImagesEstablishment> img_establishment_found = await ListImagesByEstablishment(establishment.IdEstablishment);
            List<ImagesEstablishment> originals_images = img_establishment_found.ToList();

            var counter = 0;

            for (int i = 0; i < imageIds.Count; i++)
            {
                ImagesEstablishment imagesEstablishment = new ImagesEstablishment();
                ImagesOrderViewModel listOrder = JsonConvert.DeserializeObject<ImagesOrderViewModel>(imageIds[i]);

                var current = listOrder.currentOrd;
                var ini = listOrder.iniOrd;

                if (!int.TryParse(ini, out int result)) // insert / delete image (olds)
                {
                    if (ini == "to-del")
                    {
                        ImagesEstablishment image_found = originals_images[i];
                        //var keep_image = allImagesUrl.Find(c => c.Equals(image_found.UrlImage));
                        if (image_found != null)
                        {
                            var respuesta = await EliminarImagen(image_found);

                            if (respuesta)
                            {
                                await _fireBaseService.EliminarStorage("carpeta_establishment", image_found.NameImage);
                            }

                        }
                    }
                    else
                    {
                        var image = newImages[counter];

                        if (image != null)
                        {
                            imagesEstablishment.ImageNumber = i + 1;
                            imagesEstablishment.IdEstablishment = establishment.IdEstablishment;
                            string nombre_en_codigo = Guid.NewGuid().ToString("N");
                            string extension = Path.GetExtension(image.FileName);
                            imagesEstablishment.NameImage = string.Concat(nombre_en_codigo, extension);
                            Stream streamImagen = image.OpenReadStream();

                            ImagesEstablishment image_created = await CreateImages(imagesEstablishment, streamImagen);
                        }
                        counter++;
                    }
                }
                else if (current != Int32.Parse(ini)) // edit reg image (old)
                {
                    ImagesEstablishment image_found = originals_images.Find(c => c.ImageNumber == Int32.Parse(ini));
                    image_found.ImageNumber = current;

                    ImagesEstablishment imagen_editada = await EditarImagenes(image_found);
                }

            }
        }
        public async Task<List<ImagesEstablishment>> ListImagesByEstablishment(int idEstablishment)
        {
            IQueryable<ImagesEstablishment> query = await _repositorioImagesEstablishment.ConsultarLista(c => c.IdEstablishment == idEstablishment);
            return query.OrderBy(x => x.ImageNumber).ToList();
        }

        public async Task<ImagesEstablishment> GetSingleImage(int idEstablishment, int idOrder)
        {
            ImagesEstablishment image_found = await _repositorioImagesEstablishment.Obtener(c => c.IdEstablishment == idEstablishment && c.ImageNumber == idOrder);
            return image_found;
        }
        public async Task<ImagesEstablishment> EditarImagenes(ImagesEstablishment entidad)
        {

            try
            {
                IQueryable<ImagesEstablishment> queryEstablishment = await _repositorioImagesEstablishment.Consultar(p => p.IdEstablishment == entidad.IdEstablishment && p.ImageNumber == entidad.ImageNumber);

                entidad.ModificationDate = DateTime.UtcNow;

                bool respuesta = await _repositorioImagesEstablishment.Editar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la Imagen del establecimiento");
                }

                ImagesEstablishment establishmentImage_editado = queryEstablishment.First();
                return establishmentImage_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> EliminarImagen(ImagesEstablishment entidad)
        {

            try
            {
                bool respuesta = await _repositorioImagesEstablishment.Eliminar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar la Imagen del establecimiento");
                }
                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> EliminarImagen(ImagesRoom entidad)
        {

            try
            {
                bool respuesta = await _repositorioImagesRoom.Eliminar(entidad);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar la Imagen de la habitaicion");
                }

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<ImagesEstablishment> CreateImages(ImagesEstablishment entidad, Stream imagen = null)
        {
            try
            {

                if (imagen != null)
                {
                    string urlIMagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_establishment", entidad.NameImage);
                    entidad.UrlImage = urlIMagen;
                }

                ImagesEstablishment image_establishment_creado = await _repositorioImagesEstablishment.Crear(entidad);

                if (image_establishment_creado.IdImagesEstablishment == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la imagen");
                }

                IQueryable<ImagesEstablishment> query = await _repositorioImagesEstablishment.Consultar(p => p.IdEstablishment == image_establishment_creado.IdEstablishment);
                image_establishment_creado = query.Include(c => c.IdEstablishmentNavigation).First();
                return image_establishment_creado;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task<List<ImagesRoom>> ListImagesByRoom(int idRoom)
        {
            IQueryable<ImagesRoom> query = await _repositorioImagesRoom.ConsultarLista(c => c.IdRoom == idRoom);
            return query.OrderBy(x => x.ImageNumber).ToList();
        }
        public async Task<ImagesRoom> CreateImages(ImagesRoom entidad, Stream imagen = null)
        {
            try
            {

                if (imagen != null)
                {
                    string urlIMagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_room", entidad.NameImage);
                    entidad.UrlImage = urlIMagen;
                }

                ImagesRoom image_room_creado = await _repositorioImagesRoom.Crear(entidad);

                if (image_room_creado.IdImagesRoom == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la imagen");
                }

                IQueryable<ImagesRoom> query = await _repositorioImagesRoom.Consultar(p => p.IdRoom == image_room_creado.IdRoom);
                image_room_creado = query.Include(c => c.IdRoomNavigation).First();
                return image_room_creado;
            }
            catch (Exception e)
            {

                throw;
            }


        }
        public async Task<ImagesRoom> EditarRoomImagenes(ImagesRoom entidad)
        {

            try
            {
                IQueryable<ImagesRoom> queryRoom = await _repositorioImagesRoom.Consultar(p => p.IdRoom == entidad.IdRoom && p.ImageNumber == entidad.ImageNumber);

                entidad.ModificationDate = DateTime.UtcNow;

                bool respuesta = await _repositorioImagesRoom.Editar(entidad);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Imagen");
                }

                ImagesRoom establishmentImage_editado = queryRoom.First();
                return establishmentImage_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private class ImagesOrderViewModel
        {
            public int currentOrd { get; set; }
            public string iniOrd { get; set; }
        }
    }

}

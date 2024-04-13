using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.Entity;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void DependencyInyection(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<DbventaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"))
            );

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMovimientoRepository,MovimientoRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();//todo volar
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IProveedorService, ProveedorService>();
            services.AddScoped<ITipoDocumentoMovimientoService, TipoDocumentoMovimientoService>();
            services.AddScoped<IMovimientoService, MovimientoService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IMenuService, MenuService>();

            services.AddScoped<IEstablishmentService, EstablishmentService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IParamPlanService, ParamPlanService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<ICajaService, CajaService>();
            services.AddScoped<IMedioPagoService, MedioPagoService>();
            services.AddScoped<IDetalleCajaService, DetalleCajaService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAreaFisicaService, AreaFisicaService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IDetailRoomService, DetailRoomService>();
        }
    }
}

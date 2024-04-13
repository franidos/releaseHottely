using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, RolDTO>().ReverseMap();
            #endregion

            #region Usuario
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino => destino.nombreRol,
                    opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            CreateMap<UsuarioDTO, Usuario>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdRolNavigation,
                    opt => opt.Ignore());
            #endregion

            #region Categoria
            CreateMap<Categoria, CategoriaDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)); 

            CreateMap<CategoriaDTO, Categoria>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false));
            #endregion

            #region room_map

            CreateMap<RoomMapOrigin, RoomMapOriginDTO>()
                .ForMember(destino => destino.RoomName,
                    opt => opt.MapFrom(origen => origen.IdRoomNavigation.Number))
                .ForMember(destino => destino.OriginName,
                    opt => opt.MapFrom(origen => origen.IdOriginNavigation.Name))
                 .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0));

            CreateMap<RoomMapOriginDTO, RoomMapOrigin>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));
            #endregion

            #region BookStatus
            CreateMap<BookStatus, BookStatusDTO>().ReverseMap();
            CreateMap<BookStatusDTO, BookStatus>().ReverseMap();
            #endregion
            #region Service
            CreateMap<Service, ServiceDTO>()
                .ForMember(destino => destino.ServiceIsActive,
                    opt => opt.MapFrom(origen => origen.ServiceIsActive == true ? 1 : 0))
                .ForMember(destino => destino.IsAdditionalValue,
                    opt => opt.MapFrom(origen => origen.IsAdditionalValue == true ? 1 : 0));

            CreateMap<ServiceDTO, Service>()
                .ForMember(destino => destino.ServiceIsActive,
                    opt => opt.MapFrom(origen => origen.ServiceIsActive == 1 ? true : false))
                .ForMember(destino => destino.IsAdditionalValue,
                    opt => opt.MapFrom(origen => origen.IsAdditionalValue == 1 ? true : false));
            #endregion

            #region Season
            CreateMap<Season, SeasonDTO>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0)); // convertir boolean en entero

            CreateMap<SeasonDTO, Season>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));// convertir entero en boolean
            #endregion

            #region Holiday
            CreateMap<Holiday, HolidayDTO>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0)); // convertir boolean en entero

            CreateMap<HolidayDTO, Holiday>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));// convertir entero en boolean
            #endregion

            #region RoomPrice
            CreateMap<RoomPrice, RoomPriceDTO>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0))
                .ForMember(destino => destino.NombreCategoria,
                    opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.NombreCategoria));


            CreateMap<RoomPriceDTO, RoomPrice>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false))
                 .ForMember(destino => destino.IdCategoriaNavigation,
                                    opt => opt.Ignore());
            #endregion

            #region CategoriaProducto
            CreateMap<CategoriaProducto, CategoriaProductoDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)); 

            CreateMap<CategoriaProductoDTO, CategoriaProducto>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false));
            #endregion
            #region Level

            CreateMap<Level, LevelDTO>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0)); // convertir boolean en entero

            CreateMap<LevelDTO, Level>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));// convertir entero en boolean
            #endregion

            #region Producto
            CreateMap<Producto, ProductoDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino => destino.NombreCategoria,
                    opt => opt.MapFrom(origen => origen.IdCategoriaProductoNavigation.Descripcion))
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO"))));

            CreateMap<ProductoDTO, Producto>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdCategoriaProductoNavigation,
                    opt => opt.Ignore())
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-CO"))));
            #endregion

            #region Room
            CreateMap<Room, RoomDTO>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0))
                .ForMember(destino => destino.StatusName,
                    opt => opt.MapFrom(origen => origen.IdRoomStatusNavigation.Title))
                .ForMember(destino => destino.StatusBackgroud,
                    opt => opt.MapFrom(origen => origen.IdRoomStatusNavigation.Background));
            //.ForMember(destino => destino.Price,
            //    opt => opt.MapFrom(origen => origen.Price.Value));

            CreateMap<RoomDTO, Room>()
                .ForMember(destino => destino.IsActive,
                    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));
                //.ForMember(destino => destino.IdCategoriaNavigation,
                //    opt => opt.Ignore());
            //.ForMember(destino => destino.Price,
            //    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Price, new CultureInfo("es-CO"))));
            #endregion
            #region ImagesRoom
            CreateMap<ImagesRoom, ImagesRoomDTO>().ReverseMap();

            CreateMap<ImagesRoomDTO, ImagesRoom>().ReverseMap();
            #endregion
            #region ImagesEstablishment
            CreateMap<ImagesEstablishment, ImagesEstablishmentDTO>().ReverseMap();

            CreateMap<ImagesEstablishmentDTO, ImagesEstablishment>().ReverseMap();
            #endregion
            //#region TempRoom
            //CreateMap<TempRoom, TempRoomDTO>().ReverseMap();

            //CreateMap<TempRoomDTO, TempRoom>().ReverseMap();
            //#endregion

            #region Proveedor
            CreateMap<Proveedor, ProveedorDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0));

            CreateMap<ProveedorDTO, Proveedor>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false));

            #endregion

            #region Establishment
            CreateMap<Establishment, EstablishmentDTO>().ReverseMap();
            //.ForMember(destino => destino.IsActive,
            //    opt => opt.MapFrom(origen => origen.IsActive == true ? 1 : 0));


            CreateMap<EstablishmentDTO, Establishment>().ReverseMap();
            //.ForMember(destino => destino.IsActive,
            //    opt => opt.MapFrom(origen => origen.IsActive == 1 ? true : false));

            #endregion

            #region Guest
            CreateMap<Guest, GuestDTO>()
                .ForMember(destino => destino.IsMain,
                    opt => opt.MapFrom(origen => origen.IsMain == true ? 1 : 0));
                    //            .ForMember(destino => destino.CreationDate,
                    //opt => opt.MapFrom(origen => origen.CreationDate.Value.ToString("dd/MM/yyyy")));


            CreateMap<GuestDTO, Guest>()
                .ForMember(destino => destino.IsMain,
                    opt => opt.MapFrom(origen => origen.IsMain == 1 ? true : false));
                    //                            .ForMember(destino => destino.CreationDate,
                    //opt => opt.MapFrom(origen => origen.CreationDate.ToString("dd/MM/yyyy")));

            #endregion

            #region TipoDocumentoMovimiento
            CreateMap<TipoDocumentoMovimiento, TipoDocumentoMovimientoDTO>().ReverseMap();
            #endregion

            #region Movimiento
            CreateMap<Movimiento, MovimientoDTO>()
                .ForMember(destino => destino.TipoDocumentoMovimiento,
                    opt => opt.MapFrom(origen => origen.IdTipoDocumentoMovimientoNavigation.Descripcion))
                .ForMember(destino => destino.Usuario,
                    opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
                .ForMember(destino => destino.SubTotal,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal != null? origen.SubTotal.Value : 0, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.ImpuestoTotal,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal != null? origen.ImpuestoTotal.Value : 0, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Total != null? origen.Total.Value : 0, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.FechaRegistro,
                    opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));

            CreateMap<MovimientoDTO, Movimiento>()
                .ForMember(destino => destino.SubTotal,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.ImpuestoTotal,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-CO"))));
            #endregion

            #region Book
            CreateMap<Book, BookDTO>()
                    .ForMember(destino => destino.CheckIn,
                    opt => opt.MapFrom(origen => origen.CheckIn.ToString("dd/MM/yyyy")))
                    .ForMember(destino => destino.CheckOut,
                    opt => opt.MapFrom(origen => origen.CheckOut.ToString("dd/MM/yyyy")))
                    .ForMember(destino => destino.StatusName,
                    opt => opt.MapFrom(origen => origen.IdBookStatusNavigation.StatusName))
                    .ForMember(destino => destino.statusBackground,
                    opt => opt.MapFrom(origen => origen.IdBookStatusNavigation.Background));

            CreateMap<BookDTO, Book>()
                    .ForMember(destino => destino.IdBookStatusNavigation,
                    opt => opt.Ignore())
                    .ForMember(destino => destino.IdBookStatusNavigation,
                    opt => opt.Ignore());

            #endregion

            #region DetailBook
            CreateMap<DetailBook, DetailBookDTO>().ReverseMap();

            CreateMap<DetailBookDTO, DetailBook>().ReverseMap();

            #endregion
            #region Subscription
            CreateMap<Subscription, SubscriptionDTO>().ReverseMap();

            CreateMap<SubscriptionDTO, Subscription>().ReverseMap();

            #endregion
            #region DetalleMovimiento
            CreateMap<DetalleMovimiento, DetalleMovimientoDTO>()
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Precio != null? origen.Precio.Value : 0, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Total != null? origen.Total.Value: 0, new CultureInfo("es-CO"))));

            CreateMap<DetalleMovimientoDTO, DetalleMovimiento>()
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-CO"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-CO"))));

            CreateMap<DetalleMovimiento, ReporteMovimientoDTO>()
               .ForMember(destino => destino.FechaRegistro,
                   opt => opt.MapFrom(origen => origen.IdMovimientoNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
               .ForMember(destino => destino.NumeroMovimiento,
                   opt => opt.MapFrom(origen => origen.IdMovimientoNavigation.NumeroMovimiento))
               .ForMember(destino => destino.TipoDocumento,
                   opt => opt.MapFrom(origen => origen.IdMovimientoNavigation.IdTipoDocumentoMovimientoNavigation.Descripcion))
               .ForMember(destino => destino.TipoDocumento,
                   opt => opt.MapFrom(origen => origen.IdMovimientoNavigation.DocumentoCliente))
               .ForMember(destino => destino.NombreCliente,
                   opt => opt.MapFrom(origen => origen.IdMovimientoNavigation.NombreCliente))
               .ForMember(destino => destino.SubTotalMovimiento,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdMovimientoNavigation.SubTotal != null? origen.IdMovimientoNavigation.SubTotal.Value : 0, new CultureInfo("es-CO"))))
               .ForMember(destino => destino.ImpuestoTotalMovimiento,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdMovimientoNavigation.ImpuestoTotal != null? origen.IdMovimientoNavigation.ImpuestoTotal.Value : 0, new CultureInfo("es-CO"))))
               .ForMember(destino => destino.TotalMovimiento,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdMovimientoNavigation.Total != null? origen.IdMovimientoNavigation.Total.Value : 0, new CultureInfo("es-CO"))))
               .ForMember(destino => destino.Producto,
                   opt => opt.MapFrom(origen => origen.DescripcionProducto))
               .ForMember(destino => destino.Precio,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.Precio != null? origen.Precio.Value: 0, new CultureInfo("es-CO"))))
               .ForMember(destino => destino.Total,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.Total != null? origen.Total.Value : 0, new CultureInfo("es-CO"))));
            #endregion

            #region Pedido
            //CreateMap<Pedido, PedidoDTO>()
            //    //.ForMember(destino => destino.TipoDocumentoPedido,
            //    //    opt => opt.MapFrom(origen => origen.IdTipoDocumentoPedidoNavigation.Descripcion))
            //    .ForMember(destino => destino.Usuario,
            //        opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
            //    .ForMember(destino => destino.SubTotal,
            //        opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.ImpuestoTotal,
            //        opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.Total,
            //        opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.FechaRegistro,
            //        opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));

            //CreateMap<PedidoDTO, Pedido>()
            //    .ForMember(destino => destino.SubTotal,
            //        opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.ImpuestoTotal,
            //        opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.Total,
            //        opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-CO"))));
            #endregion

            #region MedioPago
            CreateMap<MedioPago, MedioPagoDTO>().ReverseMap();
            CreateMap<MedioPagoDTO, MedioPago>().ReverseMap();
            #endregion

            #region DetallePedido
            //CreateMap<DetallePedido, DetallePedidoDTO>()
            //    .ForMember(destino => destino.Precio,
            //        opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.Total,
            //        opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO"))));

            //CreateMap<DetallePedidoDTO, DetallePedido>()
            //    .ForMember(destino => destino.Precio,
            //        opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-CO"))))
            //    .ForMember(destino => destino.Total,
            //        opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-CO"))));

            //CreateMap<DetallePedido, ReportePedidoDTO>()
            //   .ForMember(destino => destino.FechaRegistro,
            //       opt => opt.MapFrom(origen => origen.IdPedidoNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
            //   .ForMember(destino => destino.NumeroPedido,
            //       opt => opt.MapFrom(origen => origen.IdPedidoNavigation.NumeroPedido))
            //   .ForMember(destino => destino.TipoDocumento,
            //       opt => opt.MapFrom(origen => origen.IdPedidoNavigation.IdTipoDocumentoPedidoNavigation.Descripcion))
            //   .ForMember(destino => destino.TipoDocumento,
            //       opt => opt.MapFrom(origen => origen.IdPedidoNavigation.DocumentoCliente))
            //   .ForMember(destino => destino.NombreCliente,
            //       opt => opt.MapFrom(origen => origen.IdPedidoNavigation.NombreCliente))
            //   .ForMember(destino => destino.SubTotalPedido,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.IdPedidoNavigation.SubTotal.Value, new CultureInfo("es-CO"))))
            //   .ForMember(destino => destino.ImpuestoTotalPedido,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.IdPedidoNavigation.ImpuestoTotal.Value, new CultureInfo("es-CO"))))
            //   .ForMember(destino => destino.TotalPedido,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.IdPedidoNavigation.Total.Value, new CultureInfo("es-CO"))))
            //   .ForMember(destino => destino.Producto,
            //       opt => opt.MapFrom(origen => origen.DescripcionProducto))
            //   .ForMember(destino => destino.Precio,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO"))))
            //   .ForMember(destino => destino.Total,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO"))));
            #endregion

            #region Menu
            CreateMap<Menu, MenuDTO>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion
        }
    }
}

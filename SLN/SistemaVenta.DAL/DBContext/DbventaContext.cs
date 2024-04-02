using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.DBContext;

public partial class DbventaContext : DbContext
{
    public DbventaContext()
    {
    }

    public DbventaContext(DbContextOptions<DbventaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }
    public virtual DbSet<CategoriaProducto> CategoriaProducto { get; set; }

    public virtual DbSet<Configuracion> Configuracions { get; set; }

    public virtual DbSet<DetalleMovimiento> DetalleMovimiento { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

   // public virtual DbSet<Negocio> Negocios { get; set; }

    public virtual DbSet<NumeroCorrelativo> NumeroCorrelativos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    public virtual DbSet<TipoDocumentoMovimiento> TipoDocumentoMovimiento { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Movimiento> Movimiento { get; set; }
    public virtual DbSet<Caja> Caja { get; set; }
    public virtual DbSet<DetalleCaja> DetalleCaja { get; set; }
    public virtual DbSet<MedioPago> MedioPago { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Establishment> Establishments { get; set; }
   // public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
   // public DbSet<Company> Companies { get; set; }
    public DbSet<GeneralParams> GeneralParams { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<ParamPlan> ParamPlans { get; set; }
    public DbSet<Origin> Origins { get; set; }
    public DbSet<ImagesEstablishment> ImagesEstablishments { get; set; }
    public DbSet<ImagesRoom> ImagesRoom { get; set; }
    public DbSet<Service> Service { get; set; }
    public DbSet<RoomPrice> RoomPrices { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<BookStatus> BookStatus { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    public DbSet<RoomMapOrigin> RoomMapOrigin { get; set; }
    public DbSet<ServiceInfoEstablishment> ServiceInfoEstablishment { get; set; }
    public DbSet<ServiceInfo> ServiceInfo { get; set; }
    public DbSet<AreaFisica> AreaFisica { get; set; }
    public DbSet<DetailRoom> DetailRoom { get; set; }
    public IEnumerable<BookingDetailResult> GetBookingsByDateRange(DateTime fechaIni, DateTime fechaFin, int idCompany)
    {
        return this.Set<BookingDetailResult>().FromSqlRaw("EXEC sp_GetBookingsByDateRange @fechaIni={0}, @fechaFin={1}, @idCompany={2}", fechaIni, fechaFin, idCompany);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingDetailResult>(entity => { entity.HasNoKey(); });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__8A3D240C512DD09F");

            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreCategoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaProducto).HasName("PK__CateProd__8A3D240C512DD09F");

            entity.Property(e => e.IdCategoriaProducto).HasColumnName("idCategoriaProducto");
            entity.Property(e => e.NombreCategoriaProducto)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreCategoriaProducto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<RoomPrice>(entity =>
        {
            entity.HasKey(e => e.IdRoomPrice).HasName("PK__RoomPri__8A3D240C512DD09F");
            entity.ToTable("RoomPrice");
            entity.Property(e => e.IdRoomPrice).HasColumnName("idRoomPrice");
            entity.Property(e => e.Monday)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monday");
            entity.Property(e => e.Tuesday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("tuesday");
            entity.Property(e => e.Wednesday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("wednesday");
            entity.Property(e => e.Thursday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("thursday");
            entity.Property(e => e.Friday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("friday");
            entity.Property(e => e.Saturday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("saturday");
            entity.Property(e => e.Sunday)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("sunday");
            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("1")
                .HasColumnName("isActive");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.RoomPrices)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_RoomPrice_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.IdSeason).HasName("PK__SeasonPri__8A3D240C512DD09F");
            entity.ToTable("Season");

            entity.Property(e => e.IdSeason).HasColumnName("idSeason");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Date).HasColumnType("date").HasColumnName("date");
            entity.Property(e => e.Increment)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("increment");
            entity.Property(e => e.IsActive).HasDefaultValueSql("1").HasColumnName("isActive");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");

            entity.Property(e => e.DayOfTheWeek)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("dayOfTheWeek")
                .HasConversion(
                    v => v.ToString(),
                    v => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), v)
                );
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Seasons)
                  .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Season_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.IdHoliday).HasName("PK__Holi__B84A03C11B1726B5");
            entity.ToTable("Holiday");

            entity.Property(e => e.IdHoliday).HasColumnName("idHoliday");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Date).HasColumnType("date").HasColumnName("date");
            entity.Property(e => e.Increment)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("increment");
            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("creationDate");

            entity.Property(e => e.DayOfTheWeek)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("dayOfTheWeek")
                .HasConversion(
                    v => v.ToString(),
                    v => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), v)
                );
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Holidays)
                  .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Holiday_idEstablishment__403A8C7D");
        });



        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.IdLevel).HasName("PK__Level__8A3D240C512DD09F");

            entity.Property(e => e.LevelNumber).HasColumnName("levelNumber");
            entity.Property(e => e.LevelName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("levelName");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Levels)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_Level_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Configuracion");

            entity.Property(e => e.Propiedad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("propiedad");
            entity.Property(e => e.Recurso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("recurso");
            entity.Property(e => e.Valor)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<DetalleMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdDetalleMovimiento).HasName("PK__DetalleV__BFE2843FB3D3EFB5");

            entity.Property(e => e.IdDetalleMovimiento).HasColumnName("idDetalleMovimiento");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.DescripcionProducto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcionProducto");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.IdMovimiento).HasColumnName("idMovimiento");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleMovimiento)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetalleMovimiento_Producto");

            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.DetalleMovimiento)
                .HasForeignKey(d => d.IdMovimiento)
                .HasConstraintName("FK__DetalleVe__idVen__440B1D61");
        });

        modelBuilder.Entity<BookStatus>(entity =>
        {
            entity.HasKey(e => e.IdBookStatus).HasName("PK__BookStat__A9D59AEE8BEB6B2E");

            entity.Property(e => e.IdBookStatus).HasColumnName("idBookStatus").ValueGeneratedNever();
            entity.Property(e => e.StatusName)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("StatusName");
            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("modificationDate");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });

        modelBuilder.Entity<RoomStatus>(entity =>
        {
            entity.HasKey(e => e.IdRoomStatus).HasName("PK__RoomStat__A9D59AEE8BEB6B2E");
            entity.Property(e => e.IdRoomStatus).HasColumnName("idRoomStatus").ValueGeneratedNever();
            
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.TitleEn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("titleEn");
            entity.Property(e => e.TitlePor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("titlePor");
            entity.Property(e => e.Background)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("background");

        });

        modelBuilder.Entity<DetailBook>(entity =>
        {
            entity.HasKey(e => e.IdDetailBook).HasName("PK__DetalleB__BFE2843FB3D3EFB5");
            entity.Property(e => e.IdDetailBook).HasColumnName("idDetailBook");
            entity.Property(e => e.IdRoom).HasColumnName("idRoom");
            entity.Property(e => e.IdBook).HasColumnName("idBook");
            //entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.IdGuest).HasColumnName("idGuest");

            entity.HasOne(d => d.IdRoomNavigation).WithMany(p => p.DetailBook)
                .HasForeignKey(d => d.IdRoom)
                .HasConstraintName("FK_DetailBook_Room");  //.OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdGuestNavigation).WithMany(p => p.DetailBook)
                .HasForeignKey(d => d.IdGuest)
                .HasConstraintName("FK_DetailBook_Guest");

            //entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.DetailBook)
            //    .HasForeignKey(d => d.IdCategoria)
            //    .HasConstraintName("FK_DetailBook_Categ");

            entity.HasOne(d => d.IdBookNavigation).WithMany(p => p.DetailBook)
                .HasForeignKey(d => d.IdBook)
                .HasConstraintName("FK__DetailBo__idBoo");

        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__Menu__C26AF483A6068BB7");

            entity.ToTable("Menu");

            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.Controlador)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("controlador");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Icono)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("icono");
            entity.Property(e => e.IdMenuPadre).HasColumnName("idMenuPadre");
            entity.Property(e => e.PaginaAccion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("paginaAccion");

            entity.HasOne(d => d.IdMenuPadreNavigation).WithMany(p => p.InverseIdMenuPadreNavigation)
                .HasForeignKey(d => d.IdMenuPadre)
                .HasConstraintName("FK__Menu__idMenuPadr__24927208");
        });

        modelBuilder.Entity<Establishment>(entity =>
        {
            entity.HasKey(e => e.IdEstablishment).HasName("PK__Establi__8A3D240C512DD09F");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment");

            entity.ToTable("Establishment");

            entity.Property(e => e.NIT)
               .HasMaxLength(50)
               .IsUnicode(false)
               .HasColumnName("nIT");
            entity.Property(e => e.EstablishmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("establishmentName");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EstablishmentType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("establishmentType");
            entity.Property(e => e.Contact)
              .HasMaxLength(50)
              .IsUnicode(false)
              .HasColumnName("contact");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("province");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.Descripcion)
              .HasMaxLength(int.MaxValue)
              .IsUnicode(false)
              .HasColumnName("descripcion");
            entity.Property(e => e.Geolocation)
              .HasMaxLength(700)
              .IsUnicode(false)
              .HasColumnName("geolocation");
            entity.Property(e => e.Token)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("token");          
           entity.Property(e => e.Rnt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rnt");
        //    entity.Property(e => e.IsActive).HasDefaultValueSql("1").HasColumnName("isActive");
            entity.Property(e => e.NameImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nameImage");
            entity.Property(e => e.UrlImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImage");
            entity.Property(e => e.CheckInTime)
                .HasColumnType("datetime")
                .HasColumnName("checkInTime");
           entity.Property(e => e.CheckOutTime)
                .HasColumnType("datetime")
                .HasColumnName("checkOutTime");
           entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");

            //entity.HasOne(d => d.IdCompanyNavigation).WithMany(p => p.Establishments)
            //    .HasForeignKey(d => d.IdCompany)
            //    .HasConstraintName("FK__Establishment__idComp__36B12243");
        });

        modelBuilder.Entity<NumeroCorrelativo>(entity =>
        {
            entity.HasKey(e => e.IdNumeroCorrelativo).HasName("PK__NumeroCo__25FB547E65A40777");

            entity.ToTable("NumeroCorrelativo");

            entity.Property(e => e.IdNumeroCorrelativo).HasColumnName("idNumeroCorrelativo");
            entity.Property(e => e.CantidadDigitos).HasColumnName("cantidadDigitos");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Gestion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("gestion");
            entity.Property(e => e.UltimoNumero).HasColumnName("ultimoNumero");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.NumeroCorrelativos)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_NumeroCorrelativo_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__07F4A132797CBE6F");

            entity.ToTable("Producto");

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.CodigoBarra)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigoBarra");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdCategoriaProducto).HasColumnName("idCategoria");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("marca");
            entity.Property(e => e.NombreImagen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreImagen");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImagen");

            entity.HasOne(d => d.IdCategoriaProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoriaProducto)
                .HasConstraintName("FK__Producto__idCate__36B12243");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Productos)
               .HasForeignKey(d => d.IdProveedor)
               .HasConstraintName("FK__Producto__idProv__36B12244");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__3C872F7648303C61");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol").ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<RolMenu>(entity =>
        {
            entity.HasKey(e => e.IdRolMenu).HasName("PK__RolMenu__CD2045D88E5957FC");

            entity.ToTable("RolMenu");

            entity.Property(e => e.IdRolMenu).HasColumnName("idRolMenu");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.IdMenu)
                .HasConstraintName("FK__RolMenu__idMenu__2C3393D0");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__RolMenu__idRol__2B3F6F97");
        });

        modelBuilder.Entity<TipoDocumentoMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumentoMovimiento).HasName("PK__TipoDocu__A9D59AEE8BEB6B2E");

            entity.Property(e => e.IdTipoDocumentoMovimiento).HasColumnName("idTipoDocumentoMovimiento").ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A6ACC301A0");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__idRol__300424B4");
        });
        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedor__645723A6ACC301A0");

            entity.ToTable("Proveedor");

            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });
        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimiento__077D56148B22AC5G");

            entity.Property(e => e.IdMovimiento).HasColumnName("idMovimiento");
            entity.Property(e => e.DocumentoCliente)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("documentoCliente");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdTipoDocumentoMovimiento).HasColumnName("idTipoDocumentoMovimiento");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.ImpuestoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("impuestoTotal");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombreCliente");
            entity.Property(e => e.NumeroMovimiento)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("numeroMovimiento");
            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subTotal");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdMovimientoRel).HasColumnName("idMovimientoRel");

            entity.HasOne(d => d.IdTipoDocumentoMovimientoNavigation).WithMany(p => p.Movimiento)
                .HasForeignKey(d => d.IdTipoDocumentoMovimiento)
                .HasConstraintName("FK__Movimiento__idTipoDoc__3F466844");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Movimiento)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Movimiento__idUsuario__403A8C7D");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Movimiento)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("FK__Movimiento__idProveedor__403A8C7D");

            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Movimientos)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_Movimiento_idEstablishment__403A8C7D");

        });

        modelBuilder.Entity<Caja>(entity =>
        {
            entity.HasKey(e => e.IdCaja).HasName("PK__Caja__077D56148B22AC5G");

            entity.Property(e => e.IdCaja).HasColumnName("idCaja");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.FechaCierre)
                .HasColumnType("datetime")
                .HasColumnName("fechaCierre");
            entity.Property(e => e.SaldoInicial)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("saldoInicial");
            entity.Property(e => e.SaldoFinal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("saldoFinal");
            entity.Property(e => e.SaldoReal)
              .HasColumnType("decimal(10, 2)")
              .HasColumnName("saldoReal");
            entity.Property(e => e.Observacion)
              .HasMaxLength(200)
              .IsUnicode(false)
              .HasColumnName("observacion");
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Caja)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Caja_Usuario__403A8C7D");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Cajas)
                .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Caja_idEstablishment__403A8C7D");
            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.CajaNavigation)
              .HasForeignKey(d => d.IdAreaFisica).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_AreaFisica_Caja__403A8C7D");
        });

        modelBuilder.Entity<DetalleCaja>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCaja).HasName("PK__DetalleCaja__077D56148B22AC5G");
            entity.Property(e => e.Valor)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("valor");
            entity.Property(e => e.Observacion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("observacion");
            entity.HasOne(d => d.IdCajaNavigation).WithMany(p => p.DetalleCaja)
                .HasForeignKey(d => d.IdCaja)
                .HasConstraintName("FK_DetalleCaja_Caja__403A8C7D");
            entity.HasOne(d => d.IdMedioPagoNavigation).WithMany(p => p.DetalleCaja)
                .HasForeignKey(d => d.IdMedioPago)
                .HasConstraintName("FK_DetalleCaja_medioPago__403A8C7D");
            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.DetalleCaja)
                .HasForeignKey(d => d.IdMovimiento)
                .HasConstraintName("FK_DetalleCaja_Movimiento__403A8C7D");
        });

        modelBuilder.Entity<DetailRoom>(entity =>
        {
            entity.HasKey(e => e.IdDetailRoom).HasName("PK__DetailRoom");
            entity.Property(e => e.Observation)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("observation");
            entity.HasOne(d => d.IdRoomNavigation).WithMany(p => p.DetailRoom)
                .HasForeignKey(d => d.IdRoom)
                .HasConstraintName("FK_DetailRoom_Room");
            entity.HasOne(d => d.IdRoomStatusNavigation).WithMany(p => p.DetailRoom)
                .HasForeignKey(d => d.IdRoomStatus)
                .HasConstraintName("FK_DetailRoom_RoomStatus");
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DetailRoom)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_DetailRoom_user");
        });

        modelBuilder.Entity<MedioPago>(entity =>
        {
            entity.HasKey(e => e.IdMedioPago).HasName("PK__MedioPago__077D56148B22AC5G");

            entity.Property(e => e.IdMedioPago).HasColumnName("idMedioPago").ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasColumnName("descripcion");
            entity.Property(e => e.Naturaleza)
                  .HasMaxLength(2)
                  .IsUnicode(false)
                  .HasColumnName("naturaleza");
            entity.Property(e => e.UrlImagen)
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasColumnName("urlImagen");

        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.IdGuest).HasName("PK_Guest");

            entity.ToTable("Guest");

            entity.Property(e => e.IdGuest).HasColumnName("idGuest");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.CreationDate)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime")
               .HasColumnName("creationDate");
            entity.Property(e => e.DocumentType).HasColumnName("documentType");
            entity.Property(e => e.OriginCity).HasColumnName("originCity");
            entity.Property(e => e.RecidenceCity).HasColumnName("recidenceCity");
            entity.Property(e => e.NumberCompanions).HasColumnName("numberCompanions");
            entity.Property(e => e.Nationality).HasColumnName("nationality");
            entity.Property(e => e.OriginCountry).HasColumnName("originCountry");
            entity.Property(e => e.Treatment).HasColumnName("treatment");
            entity.Property(e => e.IdMainGuest).HasColumnName("idMainGuest");
            entity.Property(e => e.IsMain).HasColumnName("isMain");
            entity.Property(e => e.IsChild).HasColumnName("isChild");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.Document)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("document");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.Guests)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_Guest_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.IdBook).HasName("PK__Book__077D56148B22AC5G");
            entity.Property(e => e.IdBook).HasColumnName("idBook");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
            entity.Property(e => e.CheckIn)
                .HasColumnType("datetime")
                .HasColumnName("checkIn");
            entity.Property(e => e.CheckOut)
                .HasColumnType("datetime")
                .HasColumnName("checkOut");
            entity.Property(e => e.IdMovimiento).HasColumnName("idMovimiento");
            entity.Property(e => e.IdBookStatus).HasColumnName("idBookStatus");

            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment");

            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.Book)
                .HasForeignKey(d => d.IdMovimiento)
                .HasConstraintName("FK__Movimiento__idMovimi__3F466844");

            entity.HasOne(d => d.IdBookStatusNavigation).WithMany(p => p.Book)
                .HasForeignKey(d => d.IdBookStatus)
                .HasConstraintName("FK_BookStatus_Book");
        });

        modelBuilder.Entity<Origin>(entity =>
        {
            entity.HasKey(e => e.IdOrigin).HasName("PK__Origin__A9D59AEE8BEB6B2E");

            entity.Property(e => e.IdOrigin).HasColumnName("IdOrigin").ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.EventTitle)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("eventTitle");
            entity.Property(e => e.BackgroundColor)
             .HasMaxLength(15)
             .IsUnicode(false)
             .HasColumnName("backgroundColor");
            entity.Property(e => e.IsActive).HasDefaultValueSql("1").HasColumnName("isActive");
            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("modificationDate");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });

        modelBuilder.Entity<ImagesEstablishment>(entity =>
        {
            entity.HasKey(e => e.IdImagesEstablishment).HasName("PK__ImagesEstablishments__A9D59AEE8BEB6B2E");

            entity.Property(e => e.IdImagesEstablishment).HasColumnName("IdImagesEstablishment");
            entity.Property(e => e.UrlImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImage");
            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("modificationDate");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });

        modelBuilder.Entity<ImagesRoom>(entity =>
        {
            entity.HasKey(e => e.IdImagesRoom).HasName("PK__ImagesRoom__A9D59AEE8BEB6B2E");

            entity.Property(e => e.IdImagesRoom).HasColumnName("IdImagesRoom");
            entity.Property(e => e.UrlImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImage");
            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("modificationDate");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });
        modelBuilder.Entity<AreaFisica>(entity =>
        {
            entity.HasKey(e => e.IdAreaFisica).HasName("PK__AreaFisica__077D56148B22AC5G");
            entity.Property(e => e.Nombre)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("nombre");
            entity.Property(e => e.Descripcion)
                  .HasMaxLength(200)
                  .IsUnicode(false)
                  .HasColumnName("descripcion");
            entity.Property(e => e.NombreImpresora)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("nombreImpresora");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.AreaFisica)
              .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_AreaFisica_idEstablishment__403A8C7D");
        });
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Establishment>()
            .HasKey(e => e.IdEstablishment);

        modelBuilder.Entity<Establishment>()
            .HasMany(e => e.Rooms)
            .WithOne(r => r.IdEstablishmentNavigation)
            .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Establishment>()
          .HasMany(e => e.Cajas)
          .WithOne(r => r.IdEstablishmentNavigation)
          .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Establishment>()
          .HasMany(e => e.CategoriaProductos)
          .WithOne(r => r.IdEstablishmentNavigation)
          .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Establishment>()
          .HasMany(e => e.Productos)
          .WithOne(r => r.IdEstablishmentNavigation)
          .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Establishment>()
          .HasMany(e => e.Proveedores)
          .WithOne(r => r.IdEstablishmentNavigation)
          .HasForeignKey(r => r.IdEstablishment);

        // Establishment tiene muchos ImagesEstablishment 
        modelBuilder.Entity<Establishment>()
            .HasMany(e => e.ImagesEstablishment)
            .WithOne(r => r.IdEstablishmentNavigation)
            .HasForeignKey(r => r.IdEstablishment);

        // Establishment tiene muchos Service 
        modelBuilder.Entity<Establishment>()
            .HasMany(e => e.Service)
            .WithOne(r => r.IdEstablishmentNavigation)
            .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.IdEstablishmentNavigation)
            .WithMany(c => c.Usuarios)
            .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<Guest>()
           .HasOne(u => u.IdEstablishmentNavigation)
           .WithMany(c => c.Guests)
           .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<Level>()
           .HasOne(u => u.IdEstablishmentNavigation)
           .WithMany(c => c.Levels)
           .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<RoomPrice>()
           .HasOne(u => u.IdEstablishmentNavigation)
           .WithMany(c => c.RoomPrices)
           .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<NumeroCorrelativo>()
          .HasOne(u => u.IdEstablishmentNavigation)
          .WithMany(c => c.NumeroCorrelativos)
          .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<Movimiento>()
         .HasOne(u => u.IdEstablishmentNavigation)
         .WithMany(c => c.Movimientos)
         .HasForeignKey(u => u.IdEstablishment);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.IdRoom).HasName("PK__Room__07F4A132797CBE6F");
            entity.ToTable("Room");
            entity.Property(e => e.IdRoom).HasColumnName("idRoom");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("number");
            entity.Property(e => e.RoomTitle)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("roomTitle");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasDefaultValueSql("1").HasColumnName("isActive");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.IdLevel).HasColumnName("idLevel");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.SizeRoom).HasColumnName("sizeRoom");
            entity.Property(e => e.IdRoomStatus).HasColumnName("idRoomStatus");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Room__idCate__36B12243");

            entity.HasOne(d => d.IdLevelNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.IdLevel)
                .HasConstraintName("FK__Room__idLev__36B12243");

            entity.HasOne(d => d.IdRoomStatusNavigation).WithMany(p => p.Rooms)
              .HasForeignKey(d => d.IdRoomStatus)
              .HasConstraintName("FK__Room__idRoomStatus__36B12243");

        });

        // Establishment  tiene muchos ServicesEstablishment 
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.IdService).HasName("PK__Service__A9D59AEE8BEB6B2E");
            entity.Property(e => e.IdService).HasColumnName("IdService");

            entity.Property(e => e.ServiceName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("serviceName");

            entity.Property(e => e.ServiceInfo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("serviceInfo");

            entity.Property(e => e.ServiceInfoQuantity)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("serviceInfoQuantity");

            entity.Property(e => e.ServiceMaximumAmount)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serviceMaximumAmount");

            entity.Property(e => e.ServiceConditions)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("serviceConditions");

            entity.Property(e => e.ServicePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("servicePrice");

            entity.Property(e => e.ServiceUrlImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("serviceUrlImage");

            entity.Property(e => e.ServiceIsActive)
                .HasDefaultValueSql("1")
                .HasColumnName("serviceIsActive");
            entity.Property(e => e.IsAdditionalValue)
                .HasDefaultValueSql("1")
                .HasColumnName("isAdditionalValue");

            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("modificationDate");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });

        modelBuilder.Entity<RoomMapOrigin>(entity =>
        {
            entity.HasKey(e => e.IdRoomMap).HasName("PK__IdRoomMap__A9D59AEE8BEB6B2E");
            entity.Property(e => e.IdRoomMap).HasColumnName("idRoomMap");
            entity.Property(e => e.IdRoom).HasColumnName("idRoom");

            entity.Property(e => e.IdEstablishmentOrigin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("idEstablishmentOrigin");

            entity.Property(e => e.IdRoomOrigin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("idRoomOrigin");

            entity.Property(e => e.UrlCalendar)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("urlCalendar");

            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user");

            entity.Property(e => e.ChannelName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("channelName");

            entity.Property(e => e.IsActive)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("isActive");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");

            entity.Property(e => e.IdEstablishment).HasColumnName("idEstablishment").HasDefaultValueSql("1");
            entity.HasOne(d => d.IdEstablishmentNavigation).WithMany(p => p.RoomMapOrigins)
                  .HasForeignKey(d => d.IdEstablishment).OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_room_map_idEstablishment__403A8C7D");
        });

        modelBuilder.Entity<ServiceInfo>(entity =>
        {
            entity.HasKey(e => e.IdServiceInfo).HasName("PK__ServiceInfo__A9D59AEE8BEB6B2E");
            entity.Property(e => e.IdServiceInfo).HasColumnName("IdServiceInfo");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("icon");

            entity.Property(e => e.IsActive)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("isActive");       

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });


        modelBuilder.Entity<ServiceInfoEstablishment>(entity =>
        {
            entity.HasKey(e => e.IdServiceInfoEstab).HasName("PK__ServiceEstablishment__A9D59AEE8BEB6B2E");
            entity.Property(e => e.IdServiceInfoEstab).HasColumnName("IdServiceEst");

            entity.Property(e => e.DescripcionOpc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcionOpc");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
        });




        // Room tiene muchos ImagesRoom
        modelBuilder.Entity<Room>()
            .HasMany(e => e.ImagesRoom)
            .WithOne(r => r.IdRoomNavigation)
            .HasForeignKey(r => r.IdRoom);

        // Room tiene muchos Service 
        modelBuilder.Entity<Room>()
            .HasMany(e => e.Service)
            .WithOne(r => r.IdRoomNavigation)
            .HasForeignKey(r => r.IdRoom);

        // Room tiene muchos RoomMapperOrigin 
        modelBuilder.Entity<Room>()
            .HasMany(e => e.RoomMapOrigin)
            .WithOne(r => r.IdRoomNavigation)
            .HasForeignKey(r => r.IdRoom);

        // Origin tiene muchos RoomMapperOrigin 
        modelBuilder.Entity<Origin>()
            .HasMany(e => e.RoomMapOrigin)
            .WithOne(r => r.IdOriginNavigation)
            .HasForeignKey(r => r.IdOrigin);

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Subscription>()
            .HasKey(s => s.IdSubscription);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Establishment)
            .WithMany(c => c.Subscriptions)
            .HasForeignKey(s => s.IdEstablishment);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.IdPlan);

        OnModelCreatingPartial(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Plan>()
            .HasKey(c => c.IdPlan);

        modelBuilder.Entity<Plan>()
            .HasMany(c => c.Subscriptions)
            .WithOne(e => e.Plan)
            .HasForeignKey(e => e.IdPlan);

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<ParamPlan>()
            .HasKey(c => c.IdParamPlan);

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<ParamPlan>()
            .HasOne(gr => gr.IdPlanNavigation)
            .WithMany(r => r.ParamPlans)
            .HasForeignKey(gr => gr.IdPlan);

        // Relacion ImagesEstablishment tiene un Establishment
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<ImagesEstablishment>()
            .HasKey(r => r.IdImagesEstablishment);

        modelBuilder.Entity<ImagesEstablishment>()
            .HasOne(r => r.IdEstablishmentNavigation)
            .WithMany(e => e.ImagesEstablishment)
            .HasForeignKey(r => r.IdEstablishment);

        // Relacion ImagesRoom tienen un Room
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<ImagesRoom>()
            .HasKey(r => r.IdImagesRoom);

        modelBuilder.Entity<ImagesRoom>()
            .HasOne(r => r.IdRoomNavigation)
            .WithMany(e => e.ImagesRoom)
            .HasForeignKey(r => r.IdRoom);

        // Relacion Service tiene un Establishment  
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Service>()
            .HasKey(r => r.IdService);

        modelBuilder.Entity<Service>()
            .HasOne(r => r.IdEstablishmentNavigation)
            .WithMany(e => e.Service)
            .HasForeignKey(r => r.IdEstablishment);

        //modelBuilder.Entity<Caja>()
        //    .HasOne(r => r.IdEstablishmentNavigation)
        //    .WithMany(e => e.Cajas)
        //    .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<CategoriaProducto>()
           .HasOne(r => r.IdEstablishmentNavigation)
           .WithMany(e => e.CategoriaProductos)
           .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Producto>()
           .HasOne(r => r.IdEstablishmentNavigation)
           .WithMany(e => e.Productos)
           .HasForeignKey(r => r.IdEstablishment);

        modelBuilder.Entity<Proveedor>()
           .HasOne(r => r.IdEstablishmentNavigation)
           .WithMany(e => e.Proveedores)
           .HasForeignKey(r => r.IdEstablishment);

        // Relacion Service tiene un Room 
        /*
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Service>()
            .HasKey(r => r.IdService);
        */

        modelBuilder.Entity<Service>()
            .HasOne(r => r.IdRoomNavigation)
            .WithMany(e => e.Service)
            .HasForeignKey(r => r.IdRoom);

        // *
        // Una categoria tiene muchas SeasonPrice 
        modelBuilder.Entity<Categoria>()
            .HasMany(e => e.RoomPrice)
            .WithOne(r => r.IdCategoriaNavigation)
            .HasForeignKey(r => r.IdCategoria);

        // Relacion RoomPrice tiene una Categoria
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<RoomPrice>()
            .HasKey(r => r.IdRoomPrice);

        modelBuilder.Entity<RoomPrice>()
            .HasOne(r => r.IdCategoriaNavigation)
            .WithMany(e => e.RoomPrice)
            .HasForeignKey(r => r.IdCategoria);

        // Relacion ServiceInfoEstableishment tiene una ServiceInfo
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<ServiceInfo>()
            .HasKey(r => r.IdServiceInfo);

        // ServiceInfo tiene muchos ServiceInfoEstableishment
        modelBuilder.Entity<ServiceInfo>()
            .HasMany(e => e.ServiceInfoEstablNavigation)
            .WithOne(r => r.IdServiceInfoNavigation)
            .HasForeignKey(r => r.IdServiceInfo);

        modelBuilder.Entity<Establishment>()
            .HasMany(e => e.ServiceEstabNavigation)
            .WithOne(r => r.IdEstablishmentNavigation)
            .HasForeignKey(r => r.IdEstablishment);


    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

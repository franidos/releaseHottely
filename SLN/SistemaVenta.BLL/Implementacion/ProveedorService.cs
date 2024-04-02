using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
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
    public class ProveedorService : IProveedorService
    {
        private readonly IGenericRepository<Proveedor> _repositorio;
        private readonly IFireBaseService _firebaseSerice;
        private readonly IUtilidadesService _utilidadesService;

        public ProveedorService(IGenericRepository<Proveedor> repositorio, IFireBaseService firebaseSerice, IUtilidadesService utilidadesService)
        {
            _repositorio = repositorio;
            _firebaseSerice = firebaseSerice;
            _utilidadesService = utilidadesService;
        }

        public async Task<List<Proveedor>> Listar(int idEstablishment)
        {
            IQueryable<Proveedor> query = await _repositorio.Consultar(x => x.IdEstablishment == idEstablishment);
            return query.ToList();
        }
        public async Task<Proveedor> Crear(Proveedor entidad)
        {
            Proveedor proveedor_existe = await _repositorio.Obtener(p => p.NIT == entidad.NIT && p.IdEstablishment == entidad.IdEstablishment);
            if (proveedor_existe != null)
            {
                throw new TaskCanceledException("El Nit de proveedor ya existe");
            }

            try
            {

                Proveedor proveedor_creado = await _repositorio.Crear(entidad);

                if (proveedor_creado.IdProveedor == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el proveedor");
                }

                IQueryable<Proveedor> query = await _repositorio.Consultar(p => p.IdProveedor == proveedor_creado.IdProveedor);
                proveedor_creado = query.First();
                return proveedor_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Proveedor> Editar(Proveedor entidad)
        {
            Proveedor proveedor_existe = await _repositorio.Obtener(p => p.NIT == entidad.NIT && p.IdEstablishment == entidad.IdEstablishment && p.IdProveedor != entidad.IdProveedor);
            if (proveedor_existe != null)
            {
                throw new TaskCanceledException("El Proveedor ya existe");
            }
            try
            {
                IQueryable<Proveedor> queryProveedor = await _repositorio.Consultar(p => p.IdProveedor == entidad.IdProveedor);
                Proveedor proveedor_para_editar = queryProveedor.First();
                proveedor_para_editar.Nombre = entidad.Nombre;
                proveedor_para_editar.IdProveedor = entidad.IdProveedor;
                proveedor_para_editar.Telefono = entidad.Telefono;
                proveedor_para_editar.Contacto = entidad.Contacto;
                proveedor_para_editar.Correo = entidad.Correo;
                proveedor_para_editar.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(proveedor_para_editar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el proveedor");
                }

                Proveedor proveedor_editado = queryProveedor.First();
                return proveedor_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idProveedor)
        {
            try
            {
                Proveedor proveedor_encontrado = await _repositorio.Obtener(p => p.IdProveedor == idProveedor);
                if (proveedor_encontrado == null)
                {
                    throw new TaskCanceledException("El proveedor no existe");
                }

                bool respuesta = await _repositorio.Eliminar(proveedor_encontrado);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }

}

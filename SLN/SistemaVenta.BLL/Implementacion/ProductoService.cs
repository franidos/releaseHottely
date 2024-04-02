﻿using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IFireBaseService _firebaseSerice;
        private readonly IUtilidadesService _utilidadesService;

        public ProductoService(IGenericRepository<Producto> repositorio, IFireBaseService firebaseSerice, IUtilidadesService utilidadesService)
        {
            _repositorio = repositorio;
            _firebaseSerice = firebaseSerice;
            _utilidadesService = utilidadesService;
        }

        public async Task<List<Producto>> Listar(int idEstablishment)
        {
            IQueryable<Producto> query = await _repositorio.Consultar(x=>x.IdEstablishment == idEstablishment);
            return query.Include(p => p.IdCategoriaProductoNavigation).ToList();
        }

        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto producto_existe = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdEstablishment == entidad.IdEstablishment);
            if (producto_existe != null)
            {
                throw new TaskCanceledException("El Codigo de barra ya existe");
            }

            try
            {
                entidad.NombreImagen = nombreImagen;
                if (imagen != null)
                {
                    string urlIMagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlIMagen;
                }

                Producto producto_creado = await _repositorio.Crear(entidad);

                if (producto_creado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto");
                }

                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == producto_creado.IdProducto);
                producto_creado = query.Include(c => c.IdCategoriaProductoNavigation).First();
                return producto_creado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null)
        {
            Producto producto_existe = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdEstablishment == entidad.IdEstablishment && p.IdProducto != entidad.IdProducto);
            if (producto_existe != null)
            {
                throw new TaskCanceledException("El Codigo de barra ya existe");
            }
            try
            {
                IQueryable<Producto> queryProducto = await _repositorio.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto producto_para_editar = queryProducto.First();
                producto_para_editar.CodigoBarra = entidad.CodigoBarra;
                producto_para_editar.Marca = entidad.Marca;
                producto_para_editar.Descripcion = entidad.Descripcion;
                producto_para_editar.IdCategoriaProducto = entidad.IdCategoriaProducto;
                producto_para_editar.IdProveedor = entidad.IdProveedor;
                producto_para_editar.Stock = entidad.Stock;
                producto_para_editar.Precio = entidad.Precio;
                producto_para_editar.EsActivo = entidad.EsActivo;

                if(imagen != null)
                {
                    string urlImagen = await _firebaseSerice.SubirStorage(imagen, "carpeta_producto", producto_para_editar.NombreImagen);
                    producto_para_editar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repositorio.Editar(producto_para_editar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el producto");
                }

                Producto producto_editado = queryProducto.Include(c => c.IdCategoriaProductoNavigation).First();
                return producto_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto producto_encontrado = await _repositorio.Obtener(p => p.IdProducto == idProducto);
                if (producto_encontrado == null)
                {
                    throw new TaskCanceledException("El producto no existe");
                }

                string nombreImagen = producto_encontrado.NombreImagen;
                bool respuesta = await _repositorio.Eliminar(producto_encontrado);
                if (respuesta)
                {
                    await _firebaseSerice.EliminarStorage("carpeta_producto", nombreImagen);
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

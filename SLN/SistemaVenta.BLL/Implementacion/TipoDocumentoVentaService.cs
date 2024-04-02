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
    public class TipoDocumentoMovimientoService :ITipoDocumentoMovimientoService
    {
        private readonly IGenericRepository<TipoDocumentoMovimiento> _repository;

        public TipoDocumentoMovimientoService(IGenericRepository<TipoDocumentoMovimiento> repository)
        {
            _repository = repository;
        }

        public async Task<List<TipoDocumentoMovimiento>> Lista()
        {
           IQueryable<TipoDocumentoMovimiento> query = await _repository.Consultar();
            return query.ToList();
        }
    }
}

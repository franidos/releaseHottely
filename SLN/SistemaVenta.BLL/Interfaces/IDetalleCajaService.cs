﻿using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IDetalleCajaService
    {
        Task<List<DetalleCaja>> ObtenerDetallesCaja(int idCaja);
        Task<DetalleCaja> Crear(DetalleCaja entidad);
    }
}

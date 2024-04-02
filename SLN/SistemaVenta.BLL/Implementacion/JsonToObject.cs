using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class JsonToObject
    {
        public long? totalPorServicio { get; set; }
        public long? total { get; set; }
        public long? totalValue { get; set; }
        public List<JsonService> servicios { get; set; }

    }
}
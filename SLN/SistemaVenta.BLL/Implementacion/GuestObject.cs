using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class GuestObject
    {
        public string DocumentType { get; set; }
        public string Document { get; set; }
        public string OriginCity { get; set; }
        public string RecidenceCity { get; set; }
        public int NumberCompanions { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool IsMain { get; set; }
        public string Room { get; set; }
        public float Price { get; set; }
    }
}
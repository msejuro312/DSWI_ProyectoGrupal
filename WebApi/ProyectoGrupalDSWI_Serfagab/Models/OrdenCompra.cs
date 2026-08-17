namespace ProyectoGrupalDSWI_Serfagab.Models
{
    public class OrdenCompra
    {
        public int IdOrdenCompra { get; set; }
        public int IdProveedor { get; set; }
        public string? Proveedor { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public List<DetalleOrdenCompra> Detalles { get; set; } = new List<DetalleOrdenCompra>();
    }
}
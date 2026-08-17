namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    public class OrdenCompra
    {
        public int IdOrdenCompra { get; set; }
        public int IdProveedor { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public List<DetalleOrdenCompra> Detalles { get; set; } = new List<DetalleOrdenCompra>();
    }
}
namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    public class OrdenCompraVM
    {
        public int IdProveedor { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "PENDIENTE";
        public string Observaciones { get; set; }
        public List<DetalleOrdenCompra> Detalles { get; set; } = new List<DetalleOrdenCompra>();
    }
}
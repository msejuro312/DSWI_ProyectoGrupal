namespace ProyectoGrupalDSWI_Serfagab.Models
{
    public class DetalleOrdenCompra
    {
        public int IdDetalle { get; set; }
        public int IdOrdenCompra { get; set; }
        public int IdMaterial { get; set; }
        public string Material { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
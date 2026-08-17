namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    public class Material
    {
        public int IdMaterial { get; set; }
        public int IdTipoMaterial { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal PrecioReferencial { get; set; }
        public string Descripcion { get; set; }
        public string TipoMaterial { get; set; } = string.Empty;
    }
}

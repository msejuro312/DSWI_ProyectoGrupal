namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Material
    {
        public int IdMaterial { get; set; }
        public int IdTipoMaterial { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria")]
        public string UnidadMedida { get; set; }

        [Range(0, 999999999, ErrorMessage = "El stock debe ser un número positivo")]
        public decimal StockActual { get; set; }

        [Range(0, 999999999, ErrorMessage = "El precio referencial debe ser un número positivo")]
        public decimal PrecioReferencial { get; set; }

        public string Descripcion { get; set; } = string.Empty;
        public string TipoMaterial { get; set; } = string.Empty;
    }
}

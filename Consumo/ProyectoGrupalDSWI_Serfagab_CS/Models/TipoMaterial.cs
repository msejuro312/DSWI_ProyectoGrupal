namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TipoMaterial
    {
        public int IdTipoMaterial { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; } = string.Empty;
    }
}

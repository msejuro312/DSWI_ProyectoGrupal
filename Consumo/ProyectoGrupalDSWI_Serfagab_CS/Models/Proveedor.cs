namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Proveedor
    {
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "El RUC debe tener 11 dígitos")]
        public string Ruc { get; set; }

        [RegularExpression("^[0-9]{9}$", ErrorMessage = "El celular debe tener 9 dígitos")]
        public string Celular { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email no válido")]
        public string Email { get; set; }

        public string Descripcion { get; set; } = string.Empty;
    }
}
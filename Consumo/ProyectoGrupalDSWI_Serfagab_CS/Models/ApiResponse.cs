namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models
{
    public class ApiResponse<T>
    {
        public string message { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
    }
}

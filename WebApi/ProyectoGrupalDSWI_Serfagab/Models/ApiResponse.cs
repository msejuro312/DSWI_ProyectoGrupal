namespace ProyectoGrupalDSWI_Serfagab.Models
{
    public class ApiResponse<T>
    {
        public string message { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
    }
}

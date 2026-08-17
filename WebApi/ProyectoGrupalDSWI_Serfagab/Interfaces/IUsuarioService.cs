using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Interfaces
{
    public interface IUsuarioService
    {
        Usuario login(string usuario, string clave);
    }
}
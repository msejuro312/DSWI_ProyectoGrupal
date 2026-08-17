using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Interfaces
{
    public interface IMaterialService
    {
        List<Material> list();
        Material getById(int IdMaterial);
        bool insert(Material material);
        bool update(Material material);
        bool delete(int IdMaterial);
    }
}

using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Interfaces
{
    public interface ITipoMaterialService
    {
        List<TipoMaterial> list();
        TipoMaterial getById(int IdTipoMaterial);
        bool insert(TipoMaterial tipoMaterial);
        bool update(TipoMaterial tipoMaterial);
        int delete(int IdTipoMaterial);
    }
}
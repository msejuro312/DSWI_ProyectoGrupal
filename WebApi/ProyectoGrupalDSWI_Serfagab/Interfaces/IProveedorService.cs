using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Interfaces
{
    public interface IProveedorService
    {
        List<Proveedor> list();
        Proveedor getById(int IdProveedor);
        bool insert(Proveedor proveedor);
        bool update(Proveedor proveedor);
        bool delete(int IdProveedor);
    }
}
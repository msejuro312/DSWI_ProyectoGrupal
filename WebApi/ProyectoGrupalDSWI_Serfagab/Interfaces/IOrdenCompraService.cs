using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Interfaces
{
    public interface IOrdenCompraService
    {
        List<OrdenCompra> list();
        OrdenCompra getById(int IdOrdenCompra);
        int insert(OrdenCompra orden);
        bool delete(int IdOrdenCompra);
        int recepcionar(int IdOrdenCompra);
    }
}
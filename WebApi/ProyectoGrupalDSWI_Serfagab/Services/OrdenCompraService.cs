using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly string? conexion;
        private readonly ILogger<OrdenCompraService> _logger;

        public OrdenCompraService(IConfiguration configuration, ILogger<OrdenCompraService> logger)
        {
            conexion = configuration.GetConnectionString("conexion");
            _logger = logger;
        }

        public List<OrdenCompra> list()
        {
            List<OrdenCompra> temporal = new List<OrdenCompra>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_ordenes", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrdenCompra orden = new OrdenCompra
                            {
                                IdOrdenCompra = reader.GetInt32(0),
                                IdProveedor = reader.GetInt32(1),
                                Proveedor = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Estado = reader.GetString(4),
                                Total = reader.GetDecimal(5),
                                Observaciones = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            };
                            temporal.Add(orden);
                        }
                    }
                }
            }
            return temporal;
        }

        public OrdenCompra getById(int IdOrdenCompra)
        {
            OrdenCompra orden = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_orden_by_id", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idOrdenCompra", IdOrdenCompra);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orden = new OrdenCompra
                            {
                                IdOrdenCompra = reader.GetInt32(0),
                                IdProveedor = reader.GetInt32(1),
                                Proveedor = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                Estado = reader.GetString(4),
                                Total = reader.GetDecimal(5),
                                Observaciones = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            };
                        }
                    }
                }
            }

            if (orden != null)
            {
                orden.Detalles = listDetalles(IdOrdenCompra);
            }
            return orden;
        }

        public List<DetalleOrdenCompra> listDetalles(int IdOrdenCompra)
        {
            List<DetalleOrdenCompra> temporal = new List<DetalleOrdenCompra>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_detalle_orden", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idOrdenCompra", IdOrdenCompra);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetalleOrdenCompra detalle = new DetalleOrdenCompra
                            {
                                IdDetalle = reader.GetInt32(0),
                                IdOrdenCompra = reader.GetInt32(1),
                                IdMaterial = reader.GetInt32(2),
                                Material = reader.GetString(3),
                                Cantidad = reader.GetDecimal(4),
                                PrecioUnitario = reader.GetDecimal(5),
                                Subtotal = reader.GetDecimal(6),
                            };
                            temporal.Add(detalle);
                        }
                    }
                }
            }
            return temporal;
        }

        public int insert(OrdenCompra orden)
        {
            int idOrden = 0;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    decimal total = 0;
                    if (orden.Detalles != null)
                    {
                        foreach (var d in orden.Detalles)
                        {
                            total += d.Cantidad * d.PrecioUnitario;
                        }
                    }

                    using (SqlCommand command = new SqlCommand("sp_insert_orden_compra", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idProveedor", orden.IdProveedor);
                        command.Parameters.AddWithValue("@fecha", orden.Fecha == default(DateTime) ? (object)DBNull.Value : orden.Fecha);
                        command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(orden.Estado) ? "PENDIENTE" : orden.Estado);
                        command.Parameters.AddWithValue("@total", total);
                        command.Parameters.AddWithValue("@observaciones", (object?)orden.Observaciones ?? DBNull.Value);
                        object? result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            idOrden = Convert.ToInt32(result);
                        }
                    }

                    if (orden.Detalles != null && idOrden > 0)
                    {
                        foreach (var d in orden.Detalles)
                        {
                            using (SqlCommand command = new SqlCommand("sp_insert_detalle_orden", con))
                            {
                                command.Transaction = tran;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@idOrdenCompra", idOrden);
                                command.Parameters.AddWithValue("@idMaterial", d.IdMaterial);
                                command.Parameters.AddWithValue("@cantidad", d.Cantidad);
                                command.Parameters.AddWithValue("@precioUnitario", d.PrecioUnitario);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar la orden de compra del proveedor {IdProveedor}", orden.IdProveedor);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en insert de orden de compra"); }
                    idOrden = 0;
                }
            }
            return idOrden;
        }

        public bool delete(int IdOrdenCompra)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_orden_compra", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idOrdenCompra", IdOrdenCompra);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            return resp;
        }
    }
}
using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly string? conexion;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(IConfiguration configuration, ILogger<ProveedorService> logger)
        {
            conexion = configuration.GetConnectionString("conexion");
            _logger = logger;
        }

        public List<Proveedor> list()
        {
            List<Proveedor> temporal = new List<Proveedor>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_proveedores", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Proveedor proveedor = new Proveedor
                            {
                                IdProveedor = reader.GetInt32(0),
                                RazonSocial = reader.GetString(1),
                                Ruc = reader.GetString(2),
                                Celular = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Descripcion = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            };
                            temporal.Add(proveedor);
                        }
                    }
                }
            }
            return temporal;
        }

        public Proveedor getById(int IdProveedor)
        {
            Proveedor proveedor = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_proveedor_by_id", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idProveedor", IdProveedor);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            proveedor = new Proveedor
                            {
                                IdProveedor = reader.GetInt32(0),
                                RazonSocial = reader.GetString(1),
                                Ruc = reader.GetString(2),
                                Celular = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Descripcion = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            };
                        }
                    }
                }
            }
            return proveedor;
        }

        public bool insert(Proveedor proveedor)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_proveedor", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@razonSocial", proveedor.RazonSocial);
                        command.Parameters.AddWithValue("@ruc", proveedor.Ruc);
                        command.Parameters.AddWithValue("@celular", (object?)proveedor.Celular ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object?)proveedor.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@descripcion", (object?)proveedor.Descripcion ?? DBNull.Value);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al insertar proveedor {RazonSocial}", proveedor.RazonSocial);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en insert de proveedor"); }
                }
            }
            return resp;
        }

        public bool update(Proveedor proveedor)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_proveedor", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idProveedor", proveedor.IdProveedor);
                        command.Parameters.AddWithValue("@razonSocial", proveedor.RazonSocial);
                        command.Parameters.AddWithValue("@ruc", proveedor.Ruc);
                        command.Parameters.AddWithValue("@celular", (object?)proveedor.Celular ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object?)proveedor.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@descripcion", (object?)proveedor.Descripcion ?? DBNull.Value);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar proveedor {IdProveedor}", proveedor.IdProveedor);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en update de proveedor"); }
                }
            }
            return resp;
        }

        public bool delete(int IdProveedor)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_proveedor", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idProveedor", IdProveedor);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar proveedor {IdProveedor}", IdProveedor);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en delete de proveedor"); }
                }
            }
            return resp;
        }
    }
}
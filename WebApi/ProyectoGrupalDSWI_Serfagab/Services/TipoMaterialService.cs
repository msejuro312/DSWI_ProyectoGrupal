using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class TipoMaterialService : ITipoMaterialService
    {
        private readonly string? conexion;
        private readonly ILogger<TipoMaterialService> _logger;

        public TipoMaterialService(IConfiguration configuration, ILogger<TipoMaterialService> logger)
        {
            conexion = configuration.GetConnectionString("conexion");
            _logger = logger;
        }

        public List<TipoMaterial> list()
        {
            List<TipoMaterial> temporal = new List<TipoMaterial>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_tipo_material", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipoMaterial tipo = new TipoMaterial
                            {
                                IdTipoMaterial = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            };
                            temporal.Add(tipo);
                        }
                    }
                }
            }
            return temporal;
        }

        public TipoMaterial getById(int IdTipoMaterial)
        {
            TipoMaterial tipo = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_tipo_material_by_id", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idTipoMaterial", IdTipoMaterial);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipo = new TipoMaterial
                            {
                                IdTipoMaterial = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            };
                        }
                    }
                }
            }
            return tipo;
        }

        public bool insert(TipoMaterial tipoMaterial)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_tipo_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@nombre", tipoMaterial.Nombre);
                        command.Parameters.AddWithValue("@descripcion", (object?)tipoMaterial.Descripcion ?? DBNull.Value);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al insertar tipo de material {Nombre}", tipoMaterial.Nombre);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en insert de tipo de material"); }
                }
            }
            return resp;
        }

        public bool update(TipoMaterial tipoMaterial)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_tipo_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idTipoMaterial", tipoMaterial.IdTipoMaterial);
                        command.Parameters.AddWithValue("@nombre", tipoMaterial.Nombre);
                        command.Parameters.AddWithValue("@descripcion", (object?)tipoMaterial.Descripcion ?? DBNull.Value);
                        resp = command.ExecuteNonQuery() > 0;
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar tipo de material {IdTipoMaterial}", tipoMaterial.IdTipoMaterial);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en update de tipo de material"); }
                }
            }
            return resp;
        }

        public int delete(int IdTipoMaterial)
        {
            int resultado = 0;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_tipo_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idTipoMaterial", IdTipoMaterial);
                        SqlParameter resultadoParam = command.Parameters.Add("@resultado", System.Data.SqlDbType.Int);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        if (resultadoParam.Value != null && resultadoParam.Value != DBNull.Value)
                        {
                            resultado = Convert.ToInt32(resultadoParam.Value);
                        }
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar tipo de material {IdTipoMaterial}", IdTipoMaterial);
                    try { tran.Rollback(); }
                    catch (Exception rbEx) { _logger.LogError(rbEx, "Fallo adicional al hacer rollback en delete de tipo de material"); }
                }
            }
            return resultado;
        }
    }
}
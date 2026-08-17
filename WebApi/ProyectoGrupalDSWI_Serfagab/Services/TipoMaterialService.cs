using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class TipoMaterialService : ITipoMaterialService
    {
        private readonly string? conexion;

        public TipoMaterialService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion");
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
                catch (Exception)
                {
                    tran.Rollback();
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
                catch (Exception)
                {
                    tran.Rollback();
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
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            return resultado;
        }
    }
}
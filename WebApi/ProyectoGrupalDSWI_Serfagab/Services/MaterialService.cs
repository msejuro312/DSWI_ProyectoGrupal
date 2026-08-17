using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly string? conexion;

        public MaterialService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion");
        }

        public List<Material> list()
        {
            List<Material> temporal = new List<Material>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_materiales", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Material material = new Material
                            {
                                IdMaterial = reader.GetInt32(0),
                                IdTipoMaterial = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                UnidadMedida = reader.GetString(3),
                                StockActual = reader.GetDecimal(4),
                                PrecioReferencial = reader.GetDecimal(5),
                                Descripcion = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                TipoMaterial = reader.GetString(7),
                            };
                            temporal.Add(material);
                        }
                    }
                }
            }
            return temporal;
        }

        public Material getById(int IdMaterial)
        {
            Material material = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_material_by_id", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idMaterial", IdMaterial);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            material = new Material
                            {
                                IdMaterial = reader.GetInt32(0),
                                IdTipoMaterial = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                UnidadMedida = reader.GetString(3),
                                StockActual = reader.GetDecimal(4),
                                PrecioReferencial = reader.GetDecimal(5),
                                Descripcion = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                TipoMaterial = reader.GetString(7),
                            };
                        }
                    }
                }
            }
            return material;
        }

        public bool insert(Material material)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idTipoMaterial", material.IdTipoMaterial);
                        command.Parameters.AddWithValue("@nombre", material.Nombre);
                        command.Parameters.AddWithValue("@unidadMedida", material.UnidadMedida);
                        command.Parameters.AddWithValue("@stockActual", material.StockActual);
                        command.Parameters.AddWithValue("@precioReferencial", material.PrecioReferencial);
                        command.Parameters.AddWithValue("@descripcion", (object?)material.Descripcion ?? DBNull.Value);
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

        public bool update(Material material)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMaterial", material.IdMaterial);
                        command.Parameters.AddWithValue("@idTipoMaterial", material.IdTipoMaterial);
                        command.Parameters.AddWithValue("@nombre", material.Nombre);
                        command.Parameters.AddWithValue("@unidadMedida", material.UnidadMedida);
                        command.Parameters.AddWithValue("@stockActual", material.StockActual);
                        command.Parameters.AddWithValue("@precioReferencial", material.PrecioReferencial);
                        command.Parameters.AddWithValue("@descripcion", (object?)material.Descripcion ?? DBNull.Value);
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

        public bool delete(int IdMaterial)
        {
            bool resp = false;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_material", con))
                    {
                        command.Transaction = tran;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMaterial", IdMaterial);
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

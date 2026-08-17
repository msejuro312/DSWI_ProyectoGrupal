using Microsoft.Data.SqlClient;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string? conexion;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IConfiguration configuration, ILogger<UsuarioService> logger)
        {
            conexion = configuration.GetConnectionString("conexion");
            _logger = logger;
        }

        public Usuario login(string usuario, string clave)
        {
            Usuario user = null;

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    using (SqlCommand command = new SqlCommand("sp_login_usuario", con))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@clave", clave);
                        SqlParameter resultadoParam = command.Parameters.Add("@resultado", System.Data.SqlDbType.Int);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        con.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user = new Usuario
                                {
                                    IdUsuario = reader.GetInt32(0),
                                    NombreUsuario = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Rol = reader.GetString(4),
                                    Activo = reader.GetBoolean(5),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar login del usuario {Usuario}", usuario);
            }

            return user;
        }
    }
}
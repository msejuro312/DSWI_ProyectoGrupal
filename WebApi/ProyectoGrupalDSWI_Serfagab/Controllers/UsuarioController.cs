using Microsoft.AspNetCore.Mvc;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginVM vm)
        {
            if (string.IsNullOrEmpty(vm.Usuario) || string.IsNullOrEmpty(vm.Clave))
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "Debe ingresar usuario y clave",
                    success = false,
                    data = ""
                });
            }

            var user = _service.login(vm.Usuario, vm.Clave);
            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    message = "Usuario o clave incorrectos",
                    success = false,
                    data = ""
                });
            }

            return Ok(new ApiResponse<Usuario>
            {
                message = "Bienvenido " + user.NombreCompleto + "!",
                success = true,
                data = user
            });
        }
    }
}
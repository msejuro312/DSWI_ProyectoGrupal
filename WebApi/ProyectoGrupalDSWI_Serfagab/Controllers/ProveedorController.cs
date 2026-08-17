using Microsoft.AspNetCore.Mvc;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _service;

        public ProveedorController(IProveedorService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var proveedores = _service.list();
            return Ok(proveedores);
        }

        [HttpGet("{IdProveedor}")]
        public IActionResult GetById(int IdProveedor)
        {
            var proveedor = _service.getById(IdProveedor);
            if (proveedor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el proveedor con el id " + IdProveedor,
                    success = false,
                    data = ""
                });
            }
            else
            {
                return Ok(new ApiResponse<Proveedor>
                {
                    message = "Proveedor encontrado!",
                    success = true,
                    data = proveedor
                });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Proveedor proveedor)
        {
            var resp = _service.insert(proveedor);
            if (resp)
            {
                return Ok(new ApiResponse<Proveedor>
                {
                    message = "Proveedor insertado correctamente!",
                    success = true,
                    data = proveedor
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo insertar el proveedor",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] Proveedor proveedor)
        {
            var exists = _service.getById(proveedor.IdProveedor);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el proveedor con el id " + proveedor.IdProveedor,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.update(proveedor);
            if (resp)
            {
                return Ok(new ApiResponse<Proveedor>
                {
                    message = "Proveedor actualizado correctamente!",
                    success = true,
                    data = proveedor
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo actualizar el proveedor",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpDelete("{IdProveedor}")]
        public IActionResult Delete(int IdProveedor)
        {
            var exists = _service.getById(IdProveedor);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el proveedor con el id " + IdProveedor,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.delete(IdProveedor);
            if (resp)
            {
                return Ok(new ApiResponse<object>
                {
                    message = "Proveedor eliminado correctamente!",
                    success = true,
                    data = ""
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo eliminar el proveedor",
                    success = false,
                    data = ""
                });
            }
        }
    }
}
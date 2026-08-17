using Microsoft.AspNetCore.Mvc;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoMaterialController : ControllerBase
    {
        private readonly ITipoMaterialService _service;

        public TipoMaterialController(ITipoMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var tipos = _service.list();
            return Ok(tipos);
        }

        [HttpGet("{IdTipoMaterial}")]
        public IActionResult GetById(int IdTipoMaterial)
        {
            var tipo = _service.getById(IdTipoMaterial);
            if (tipo == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el tipo de material con el id " + IdTipoMaterial,
                    success = false,
                    data = ""
                });
            }
            else
            {
                return Ok(new ApiResponse<TipoMaterial>
                {
                    message = "Tipo de material encontrado!",
                    success = true,
                    data = tipo
                });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] TipoMaterial tipoMaterial)
        {
            var resp = _service.insert(tipoMaterial);
            if (resp)
            {
                return Ok(new ApiResponse<TipoMaterial>
                {
                    message = "Tipo de material insertado correctamente!",
                    success = true,
                    data = tipoMaterial
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo insertar el tipo de material",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] TipoMaterial tipoMaterial)
        {
            var exists = _service.getById(tipoMaterial.IdTipoMaterial);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el tipo de material con el id " + tipoMaterial.IdTipoMaterial,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.update(tipoMaterial);
            if (resp)
            {
                return Ok(new ApiResponse<TipoMaterial>
                {
                    message = "Tipo de material actualizado correctamente!",
                    success = true,
                    data = tipoMaterial
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo actualizar el tipo de material",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpDelete("{IdTipoMaterial}")]
        public IActionResult Delete(int IdTipoMaterial)
        {
            var exists = _service.getById(IdTipoMaterial);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el tipo de material con el id " + IdTipoMaterial,
                    success = false,
                    data = ""
                });
            }

            var resultado = _service.delete(IdTipoMaterial);
            if (resultado == 1)
            {
                return Ok(new ApiResponse<object>
                {
                    message = "Tipo de material eliminado correctamente!",
                    success = true,
                    data = ""
                });
            }
            else if (resultado == -1)
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se puede eliminar el tipo de material porque tiene materiales asociados",
                    success = false,
                    data = ""
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo eliminar el tipo de material",
                    success = false,
                    data = ""
                });
            }
        }
    }
}
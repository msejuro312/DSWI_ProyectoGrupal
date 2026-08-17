using Microsoft.AspNetCore.Mvc;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _service;

        public MaterialController(IMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var materiales = _service.list();
            return Ok(materiales);
        }

        [HttpGet("{IdMaterial}")]
        public IActionResult GetById(int IdMaterial)
        {
            var material = _service.getById(IdMaterial);
            if (material == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el material con el id " + IdMaterial,
                    success = false,
                    data = ""
                });
            }
            else
            {
                return Ok(new ApiResponse<Material>
                {
                    message = "Material encontrado!",
                    success = true,
                    data = material
                });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Material material)
        {
            var resp = _service.insert(material);
            if (resp)
            {
                return Ok(new ApiResponse<Material>
                {
                    message = "Material insertado correctamente!",
                    success = true,
                    data = material
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo insertar el material",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] Material material)
        {
            var exists = _service.getById(material.IdMaterial);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el material con el id " + material.IdMaterial,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.update(material);
            if (resp)
            {
                return Ok(new ApiResponse<Material>
                {
                    message = "Material actualizado correctamente!",
                    success = true,
                    data = material
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo actualizar el material",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpDelete("{IdMaterial}")]
        public IActionResult Delete(int IdMaterial)
        {
            var exists = _service.getById(IdMaterial);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró el material con el id " + IdMaterial,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.delete(IdMaterial);
            if (resp)
            {
                return Ok(new ApiResponse<object>
                {
                    message = "Material eliminado correctamente!",
                    success = true,
                    data = ""
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo eliminar el material",
                    success = false,
                    data = ""
                });
            }
        }
    }
}
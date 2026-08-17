using Microsoft.AspNetCore.Mvc;
using ProyectoGrupalDSWI_Serfagab.Interfaces;
using ProyectoGrupalDSWI_Serfagab.Models;

namespace ProyectoGrupalDSWI_Serfagab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenCompraController : ControllerBase
    {
        private readonly IOrdenCompraService _service;

        public OrdenCompraController(IOrdenCompraService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var ordenes = _service.list();
            return Ok(ordenes);
        }

        [HttpGet("{IdOrdenCompra}")]
        public IActionResult GetById(int IdOrdenCompra)
        {
            var orden = _service.getById(IdOrdenCompra);
            if (orden == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró la orden de compra con el id " + IdOrdenCompra,
                    success = false,
                    data = ""
                });
            }
            else
            {
                return Ok(new ApiResponse<OrdenCompra>
                {
                    message = "Orden de compra encontrada!",
                    success = true,
                    data = orden
                });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrdenCompra orden)
        {
            var id = _service.insert(orden);
            if (id > 0)
            {
                return Ok(new ApiResponse<object>
                {
                    message = "Orden de compra registrada correctamente!",
                    success = true,
                    data = id
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo registrar la orden de compra",
                    success = false,
                    data = ""
                });
            }
        }

        [HttpDelete("{IdOrdenCompra}")]
        public IActionResult Delete(int IdOrdenCompra)
        {
            var exists = _service.getById(IdOrdenCompra);
            if (exists == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = "No se encontró la orden de compra con el id " + IdOrdenCompra,
                    success = false,
                    data = ""
                });
            }

            var resp = _service.delete(IdOrdenCompra);
            if (resp)
            {
                return Ok(new ApiResponse<object>
                {
                    message = "Orden de compra eliminada correctamente!",
                    success = true,
                    data = ""
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    message = "No se pudo eliminar la orden de compra",
                    success = false,
                    data = ""
                });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    public class OrdenCompraController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";

        public OrdenCompraController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<List<OrdenCompra>> listOrdenes()
        {
            var response = await _httpClient.GetAsync("api/OrdenCompra");
            var content = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<OrdenCompra>>(content) ?? new List<OrdenCompra>();
            return lista;
        }

        async Task<OrdenCompra> getOrden(int IdOrdenCompra)
        {
            var response = await _httpClient.GetAsync("api/OrdenCompra/" + IdOrdenCompra);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<OrdenCompra>>(content) ?? new ApiResponse<OrdenCompra>();
            return apiResponse.data;
        }

        async Task<List<Proveedor>> listProveedores()
        {
            var response = await _httpClient.GetAsync("api/Proveedor");
            var content = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Proveedor>>(content) ?? new List<Proveedor>();
            return lista;
        }

        async Task<List<Material>> listMateriales()
        {
            var response = await _httpClient.GetAsync("api/Material");
            var content = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Material>>(content) ?? new List<Material>();
            return lista;
        }

        async Task<string> insertar(OrdenCompraVM vm)
        {
            var json = JsonConvert.SerializeObject(vm);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/OrdenCompra", body);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            return apiResponse.message;
        }

        async Task<string> eliminar(int IdOrdenCompra)
        {
            var request = await _httpClient.DeleteAsync("api/OrdenCompra/" + IdOrdenCompra);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            return apiResponse.message;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await listOrdenes();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var proveedores = await listProveedores();
            var materiales = await listMateriales();
            ViewBag.proveedores = new SelectList(proveedores, "IdProveedor", "RazonSocial");
            ViewBag.materiales = materiales;
            return View(new OrdenCompraVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrdenCompraVM vm)
        {
            var message = await insertar(vm);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int IdOrdenCompra)
        {
            var orden = await getOrden(IdOrdenCompra);
            if (orden == null)
            {
                TempData["message"] = "La orden de compra no existe!";
                return RedirectToAction("Index");
            }
            return View(orden);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int IdOrdenCompra)
        {
            var orden = await getOrden(IdOrdenCompra);
            if (orden == null)
            {
                TempData["message"] = "La orden de compra a eliminar no existe!";
                return RedirectToAction("Index");
            }
            return View(orden);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int IdOrdenCompra)
        {
            var message = await eliminar(IdOrdenCompra);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }
    }
}
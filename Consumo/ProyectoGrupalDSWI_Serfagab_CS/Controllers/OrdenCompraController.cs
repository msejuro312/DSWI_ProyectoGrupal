using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    [Authorize]
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
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<OrdenCompra>>>(content) ?? new ApiResponse<List<OrdenCompra>>();
            return apiResponse.data ?? new List<OrdenCompra>();
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
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Proveedor>>>(content) ?? new ApiResponse<List<Proveedor>>();
            return apiResponse.data ?? new List<Proveedor>();
        }

        async Task<List<Material>> listMateriales()
        {
            var response = await _httpClient.GetAsync("api/Material");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Material>>>(content) ?? new ApiResponse<List<Material>>();
            return apiResponse.data ?? new List<Material>();
        }

        async Task<ApiResponse<object>> insertar(OrdenCompraVM vm)
        {
            var json = JsonConvert.SerializeObject(vm);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/OrdenCompra", body);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
        }

        async Task<ApiResponse<object>> eliminar(int IdOrdenCompra)
        {
            var request = await _httpClient.DeleteAsync("api/OrdenCompra/" + IdOrdenCompra);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
        }

        async Task<ApiResponse<object>> recepcionar(int IdOrdenCompra)
        {
            var request = await _httpClient.PostAsync("api/OrdenCompra/" + IdOrdenCompra + "/recepcionar", null);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
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
            if (vm.IdProveedor <= 0)
            {
                TempData["message"] = "Debe seleccionar un proveedor!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Create");
            }
            if (vm.Detalles == null || vm.Detalles.Count == 0)
            {
                TempData["message"] = "Debe agregar al menos un material al detalle!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Create");
            }
            var apiResponse = await insertar(vm);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int IdOrdenCompra)
        {
            var orden = await getOrden(IdOrdenCompra);
            if (orden == null)
            {
                TempData["message"] = "La orden de compra no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            return View(orden);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int IdOrdenCompra)
        {
            var orden = await getOrden(IdOrdenCompra);
            if (orden == null)
            {
                TempData["message"] = "La orden de compra a eliminar no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            return View(orden);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int IdOrdenCompra)
        {
            var apiResponse = await eliminar(IdOrdenCompra);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Recepcionar(int IdOrdenCompra)
        {
            var apiResponse = await recepcionar(IdOrdenCompra);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }
    }
}
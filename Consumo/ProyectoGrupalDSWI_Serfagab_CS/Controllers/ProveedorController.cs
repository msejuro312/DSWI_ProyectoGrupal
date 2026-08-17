using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";

        public ProveedorController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<List<Proveedor>> listProveedores()
        {
            var response = await _httpClient.GetAsync("api/Proveedor");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Proveedor>>>(content) ?? new ApiResponse<List<Proveedor>>();
            return apiResponse.data ?? new List<Proveedor>();
        }

        async Task<Proveedor> getProveedor(int IdProveedor)
        {
            var response = await _httpClient.GetAsync("api/Proveedor/" + IdProveedor);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Proveedor>>(content) ?? new ApiResponse<Proveedor>();
            return apiResponse.data;
        }

        async Task<string> insertar(Proveedor proveedor)
        {
            var json = JsonConvert.SerializeObject(proveedor);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/Proveedor", body);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            return apiResponse.message;
        }

        async Task<string> actualizar(Proveedor proveedor)
        {
            var json = JsonConvert.SerializeObject(proveedor);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PutAsync("api/Proveedor", body);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            return apiResponse.message;
        }

        async Task<string> eliminar(int IdProveedor)
        {
            var request = await _httpClient.DeleteAsync("api/Proveedor/" + IdProveedor);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            return apiResponse.message;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await listProveedores();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new Proveedor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Proveedor proveedor)
        {
            var message = await insertar(proveedor);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int IdProveedor)
        {
            var proveedor = await getProveedor(IdProveedor);
            if (proveedor == null)
            {
                TempData["message"] = "El proveedor a editar no existe!";
                return RedirectToAction("Index");
            }
            return View(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Proveedor proveedor)
        {
            var message = await actualizar(proveedor);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int IdProveedor)
        {
            var proveedor = await getProveedor(IdProveedor);
            if (proveedor == null)
            {
                TempData["message"] = "El proveedor a eliminar no existe!";
                return RedirectToAction("Index");
            }
            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int IdProveedor)
        {
            var message = await eliminar(IdProveedor);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }
    }
}
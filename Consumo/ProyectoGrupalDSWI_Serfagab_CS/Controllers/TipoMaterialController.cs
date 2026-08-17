using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    [Authorize]
    public class TipoMaterialController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";

        public TipoMaterialController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<List<TipoMaterial>> listTipos()
        {
            var resp = await _httpClient.GetAsync("api/TipoMaterial");
            var contenido = await resp.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<TipoMaterial>>>(contenido) ?? new ApiResponse<List<TipoMaterial>>();
            return apiResponse.data ?? new List<TipoMaterial>();
        }

        async Task<TipoMaterial> getTipo(int IdTipoMaterial)
        {
            var resp = await _httpClient.GetAsync("api/TipoMaterial/" + IdTipoMaterial);
            var contenido = await resp.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TipoMaterial>>(contenido) ?? new ApiResponse<TipoMaterial>();
            return apiResponse.data;
        }

        async Task<ApiResponse<object>> insertar(TipoMaterial tipoMaterial)
        {
            var json = JsonConvert.SerializeObject(tipoMaterial);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/TipoMaterial", body);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
        }

        async Task<ApiResponse<object>> actualizar(TipoMaterial tipoMaterial)
        {
            var json = JsonConvert.SerializeObject(tipoMaterial);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PutAsync("api/TipoMaterial", body);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
        }

        async Task<ApiResponse<object>> eliminar(int IdTipoMaterial)
        {
            var request = await _httpClient.DeleteAsync("api/TipoMaterial/" + IdTipoMaterial);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
        }

        public async Task<IActionResult> Index()
        {
            var lista = await listTipos();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new TipoMaterial());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipoMaterial tipoMaterial)
        {
            var apiResponse = await insertar(tipoMaterial);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(int IdTipoMaterial)
        {
            var tipo = await getTipo(IdTipoMaterial);
            if (tipo == null)
            {
                TempData["message"] = "El tipo de material a editar no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            return View(tipo);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(TipoMaterial tipoMaterial)
        {
            var apiResponse = await actualizar(tipoMaterial);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int IdTipoMaterial)
        {
            var tipo = await getTipo(IdTipoMaterial);
            if (tipo == null)
            {
                TempData["message"] = "El tipo de material a eliminar no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int IdTipoMaterial)
        {
            var apiResponse = await eliminar(IdTipoMaterial);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }
    }
}
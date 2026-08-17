using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";
        public MaterialController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<List<Material>> listMateriales()
        {
            var response = await _httpClient.GetAsync("api/Material");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Material>>>(content) ?? new ApiResponse<List<Material>>();
            return await Task.Run(() => apiResponse.data ?? new List<Material>());
        }

        async Task<Material> getMaterial(int IdMaterial)
        {
            var response = await _httpClient.GetAsync("api/Material/" + IdMaterial);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Material>>(content) ?? new ApiResponse<Material>();
            var material = apiResponse.data;
            return await Task.Run(() => material);
        }

        async Task<List<TipoMaterial>> listTipos()
        {
            var response = await _httpClient.GetAsync("api/TipoMaterial");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<TipoMaterial>>>(content) ?? new ApiResponse<List<TipoMaterial>>();
            return await Task.Run(() => apiResponse.data ?? new List<TipoMaterial>());
        }

        async Task<ApiResponse<object>> insertar(Material material)
        {
            var json = JsonConvert.SerializeObject(material);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/Material", body);
            var response = await request.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>());
        }

        async Task<ApiResponse<object>> actualizar(Material material)
        {
            var json = JsonConvert.SerializeObject(material);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PutAsync("api/Material", body);
            var response = await request.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>());
        }

        async Task<ApiResponse<object>> eliminar(int IdMaterial)
        {
            var request = await _httpClient.DeleteAsync("api/Material/" + IdMaterial);
            var response = await request.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>());
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var lista = await listMateriales();
            return View(lista);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            var tipos = await listTipos();
            ViewBag.tipos = new SelectList(tipos, "IdTipoMaterial", "Nombre");
            return View(new Material());
        }

        [HttpPost]

        public async Task<IActionResult> Create(Material material)
        {
            var apiResponse = await insertar(material);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(int IdMaterial)
        {
            var material = await getMaterial(IdMaterial);
            if (material == null)
            {
                TempData["message"] = "El material a editar no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            else
            {
                var tipos = await listTipos();
                ViewBag.tipos = new SelectList(tipos, "IdTipoMaterial", "Nombre", material.IdTipoMaterial);
                return View(material);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(Material material)
        {
            var apiResponse = await actualizar(material);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int IdMaterial)
        {
            var material = await getMaterial(IdMaterial);
            if (material == null)
            {
                TempData["message"] = "El material a eliminar no existe!";
                TempData["tipo"] = "warning";
                return RedirectToAction("Index");
            }
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int IdMaterial)
        {
            var apiResponse = await eliminar(IdMaterial);
            TempData["message"] = apiResponse.message;
            TempData["tipo"] = apiResponse.success ? "success" : "danger";
            return RedirectToAction("Index");
        }
    }
}

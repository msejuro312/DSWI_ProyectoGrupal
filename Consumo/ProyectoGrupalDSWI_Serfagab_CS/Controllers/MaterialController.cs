using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
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
            var lista = JsonConvert.DeserializeObject<List<Material>>(content) ?? new List<Material>();
            return await Task.Run(() => lista);
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
            var lista = JsonConvert.DeserializeObject<List<TipoMaterial>>(content) ?? new List<TipoMaterial>();
            return await Task.Run(() => lista);
        }

        async Task<string> insertar(Material material)
        {
            var json = JsonConvert.SerializeObject(material);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/Material", body);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            var message = apiResponse.message;
            return await Task.Run(() => message);
        }

        async Task<string> actualizar(Material material)
        {
            var json = JsonConvert.SerializeObject(material);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PutAsync("api/Material", body);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            var message = apiResponse.message;
            return await Task.Run(() => message);
        }

        async Task<string> eliminar(int IdMaterial)
        {
            var request = await _httpClient.DeleteAsync("api/Material/" + IdMaterial);
            var response = await request.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response) ?? new ApiResponse<object>();
            var message = apiResponse.message;
            return await Task.Run(() => message);
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
            var message = await insertar(material);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Edit(int IdMaterial)
        {
            var material = await getMaterial(IdMaterial);
            if (material == null)
            {
                TempData["message"] = "El material a editar no existe!";
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

        public async Task<IActionResult> Edit(Material material)
        {
            var message = await actualizar(material);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int IdMaterial)
        {
            var material = await getMaterial(IdMaterial);
            if (material == null)
            {
                TempData["message"] = "El material a eliminar no existe!";
                return RedirectToAction("Index");
            }
            return View(material);
        }

        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int IdMaterial)
        {
            var message = await eliminar(IdMaterial);
            TempData["message"] = message;
            return RedirectToAction("Index");
        }
    }
}

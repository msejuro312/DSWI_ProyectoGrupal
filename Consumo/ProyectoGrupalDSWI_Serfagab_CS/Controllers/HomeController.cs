using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;
using System.Diagnostics;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<List<TipoMaterial>> listTipos()
        {
            var response = await _httpClient.GetAsync("api/TipoMaterial");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<TipoMaterial>>>(content) ?? new ApiResponse<List<TipoMaterial>>();
            return apiResponse.data ?? new List<TipoMaterial>();
        }

        async Task<List<Material>> listMateriales()
        {
            var response = await _httpClient.GetAsync("api/Material");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Material>>>(content) ?? new ApiResponse<List<Material>>();
            return apiResponse.data ?? new List<Material>();
        }

        async Task<List<Proveedor>> listProveedores()
        {
            var response = await _httpClient.GetAsync("api/Proveedor");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Proveedor>>>(content) ?? new ApiResponse<List<Proveedor>>();
            return apiResponse.data ?? new List<Proveedor>();
        }

        async Task<List<OrdenCompra>> listOrdenes()
        {
            var response = await _httpClient.GetAsync("api/OrdenCompra");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<OrdenCompra>>>(content) ?? new ApiResponse<List<OrdenCompra>>();
            return apiResponse.data ?? new List<OrdenCompra>();
        }

        public async Task<IActionResult> Index()
        {
            List<TipoMaterial> tipos = new List<TipoMaterial>();
            List<Material> materiales = new List<Material>();
            List<Proveedor> proveedores = new List<Proveedor>();
            List<OrdenCompra> ordenes = new List<OrdenCompra>();

            try { tipos = await listTipos(); } catch (Exception ex) { _logger.LogError(ex, "Dashboard: no se pudo obtener tipos de material"); }
            try { materiales = await listMateriales(); } catch (Exception ex) { _logger.LogError(ex, "Dashboard: no se pudo obtener materiales"); }
            try { proveedores = await listProveedores(); } catch (Exception ex) { _logger.LogError(ex, "Dashboard: no se pudo obtener proveedores"); }
            try { ordenes = await listOrdenes(); } catch (Exception ex) { _logger.LogError(ex, "Dashboard: no se pudo obtener órdenes de compra"); }

            ViewBag.Tipos = tipos.Count;
            ViewBag.Materiales = materiales.Count;
            ViewBag.Proveedores = proveedores.Count;
            ViewBag.Ordenes = ordenes.Count;
            ViewBag.StockTotal = materiales.Sum(m => m.StockActual);
            ViewBag.Pendientes = ordenes.Count(o => o.Estado == "PENDIENTE");
            ViewBag.UltimasOrdenes = ordenes
                .OrderByDescending(o => o.Fecha)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
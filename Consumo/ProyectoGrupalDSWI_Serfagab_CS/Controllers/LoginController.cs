using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Models;
using System.Security.Claims;
using System.Text;

namespace ProyectoGrupalDSWI_Serfagab_ConsumoServicios.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBase = "https://localhost:7229";

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiBase);
        }

        async Task<ApiResponse<Usuario>> login(string usuario, string clave)
        {
            var json = JsonConvert.SerializeObject(new { usuario = usuario, clave = clave });
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var request = await _httpClient.PostAsync("api/Usuario/login", body);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<Usuario>>(response) ?? new ApiResponse<Usuario>();
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string clave)
        {
            var apiResponse = await login(usuario, clave);

            if (!apiResponse.success || apiResponse.data == null)
            {
                TempData["message"] = apiResponse.message ?? "Usuario o clave incorrectos";
                TempData["tipo"] = "danger";
                return View();
            }

            var user = apiResponse.data;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.GivenName, user.NombreCompleto),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
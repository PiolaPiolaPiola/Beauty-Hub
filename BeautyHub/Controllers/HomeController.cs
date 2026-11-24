using BeautyHub.Data;
using BeautyHub.Helpers;
using BeautyHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BeautyHub.Controllers
{
    public class HomeController : Controller
    {
        private const string UsuarioAdmin = "admin";
        private const string ContrasenaAdmin = "12345";

        private readonly BeautyHubContext _db;
        private readonly IPasswordHasher<Usuario> _hasher;

        public HomeController(BeautyHubContext db, IPasswordHasher<Usuario> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            var model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var claims = await ObtenerClaimsAsync(model);
            if (claims is null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(new ClaimsPrincipal(identidad));

            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
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

        private async Task<List<Claim>?> ObtenerClaimsAsync(LoginViewModel model)
        {
            var usuario = await _db.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.Usuario && u.Activo);

            if (usuario is not null
                && _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, model.Contrasena) != PasswordVerificationResult.Failed)
            {
                return Identidad.Crear(usuario.NombreUsuario, usuario.Rol, usuario.ClienteId, usuario.ProfesionalId);
            }

            if (model.Usuario == UsuarioAdmin && model.Contrasena == ContrasenaAdmin)
            {
                return Identidad.Crear(UsuarioAdmin, Roles.Admin, null, null);
            }

            return null;
        }
    }
}

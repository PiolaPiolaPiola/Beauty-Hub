using BeautyHub.Data;
using BeautyHub.Helpers;
using BeautyHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeautyHub.Controllers
{
    public class CuentaController : Controller
    {
        private const string UsuarioReservado = "admin";

        private readonly BeautyHubContext _db;
        private readonly IPasswordHasher<Usuario> _hasher;

        public CuentaController(BeautyHubContext db, IPasswordHasher<Usuario> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        [HttpGet]
        public IActionResult Registro(string? tipo)
        {
            var model = new RegistroViewModel
            {
                Tipo = tipo == Roles.Profesional ? Roles.Profesional : Roles.Cliente
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            var esProfesional = model.Tipo == Roles.Profesional;
            if (!esProfesional && model.Tipo != Roles.Cliente)
            {
                ModelState.AddModelError(nameof(model.Tipo), "Tipo de cuenta no válido.");
            }

            if (esProfesional && string.IsNullOrWhiteSpace(model.Especialidad))
            {
                ModelState.AddModelError(nameof(model.Especialidad), "Indica tu especialidad.");
            }

            if (string.Equals(model.Usuario, UsuarioReservado, StringComparison.OrdinalIgnoreCase)
                || await _db.Usuarios.AnyAsync(u => u.NombreUsuario == model.Usuario))
            {
                ModelState.AddModelError(nameof(model.Usuario), "Ese nombre de usuario no está disponible.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new Usuario
            {
                NombreUsuario = model.Usuario.Trim(),
                Rol = esProfesional ? Roles.Profesional : Roles.Cliente,
                Activo = true
            };

            if (esProfesional)
            {
                usuario.Profesional = new Profesional
                {
                    Nombres = model.Nombres.Trim(),
                    Apellidos = model.Apellidos.Trim(),
                    Telefono = model.Telefono.Trim(),
                    Correo = NullIfEmpty(model.Correo),
                    Especialidad = model.Especialidad!.Trim(),
                    Activo = true
                };
            }
            else
            {
                usuario.Cliente = new Cliente
                {
                    Nombres = model.Nombres.Trim(),
                    Apellidos = model.Apellidos.Trim(),
                    Telefono = model.Telefono.Trim(),
                    Correo = NullIfEmpty(model.Correo)
                };
            }

            usuario.ContrasenaHash = _hasher.HashPassword(usuario, model.Contrasena);

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            var claims = Identidad.Crear(usuario.NombreUsuario, usuario.Rol, usuario.ClienteId, usuario.ProfesionalId);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return esProfesional
                ? RedirectToAction("Perfil", "Profesional")
                : RedirectToAction("Index", "Home");
        }

        private static string? NullIfEmpty(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}

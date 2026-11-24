using BeautyHub.Data;
using BeautyHub.Helpers;
using BeautyHub.Models;
using BeautyHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyHub.Controllers
{
    [Authorize(Roles = Roles.Profesional)]
    public class ProfesionalController : Controller
    {
        private readonly BeautyHubContext _db;

        public ProfesionalController(BeautyHubContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Perfil(DateOnly? fecha)
        {
            var idProfesional = Identidad.IdProfesional(User);
            if (idProfesional is null)
            {
                return Forbid();
            }

            var profesional = await _db.Profesionales.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idProfesional.Value);
            if (profesional is null)
            {
                return NotFound();
            }

            var dia = fecha ?? DateOnly.FromDateTime(DateTime.Now);

            var citas = await _db.Citas.AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Where(c => c.ProfesionalId == idProfesional.Value && c.Fecha == dia)
                .OrderBy(c => c.Hora)
                .ToListAsync();

            ViewData["Dia"] = dia;
            ViewData["Profesional"] = profesional;
            return View(citas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoCita nuevoEstado, DateOnly? fecha)
        {
            var idProfesional = Identidad.IdProfesional(User);
            if (idProfesional is null)
            {
                return Forbid();
            }

            var cita = await _db.Citas
                .FirstOrDefaultAsync(c => c.Id == id && c.ProfesionalId == idProfesional.Value);
            if (cita is null)
            {
                return NotFound();
            }

            if (!CitaService.TransicionValida(cita.Estado, nuevoEstado))
            {
                TempData["Error"] = $"No se puede pasar la cita de {cita.Estado} a {nuevoEstado}.";
            }
            else
            {
                cita.Estado = nuevoEstado;
                await _db.SaveChangesAsync();
                TempData["Mensaje"] = $"Cita actualizada a {nuevoEstado}.";
            }

            return RedirectToAction(nameof(Perfil), new { fecha = fecha ?? cita.Fecha });
        }
    }
}

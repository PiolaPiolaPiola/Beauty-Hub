using BeautyHub.Data;
using BeautyHub.Helpers;
using BeautyHub.Models;
using BeautyHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyHub.Controllers
{
    [Authorize(Roles = Roles.Cliente)]
    public class CitasController : Controller
    {
        private readonly BeautyHubContext _db;
        private readonly CitaService _citas;

        public CitasController(BeautyHubContext db, CitaService citas)
        {
            _db = db;
            _citas = citas;
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            var model = new RegistrarCitaViewModel();
            await LlenarListasAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarCitaViewModel model)
        {
            var idCliente = Identidad.IdCliente(User);
            if (idCliente is null)
            {
                return Forbid();
            }

            var error = await ValidarFormularioAsync(model, citaIdExcluir: null);
            if (error is not null)
            {
                await LlenarListasAsync(model);
                return View(model);
            }

            var cita = new Cita
            {
                ClienteId = idCliente.Value,
                ProfesionalId = model.ProfesionalId,
                ServicioId = model.ServicioId!.Value,
                Fecha = model.Fecha!.Value,
                Hora = model.Hora!.Value,
                Observaciones = NullIfEmpty(model.Observaciones),
                Estado = EstadoCita.Solicitada
            };

            _db.Citas.Add(cita);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmacion), new { id = cita.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmacion(int id)
        {
            var cita = await CargarCitaDelClienteAsync(id);
            return cita is null ? NotFound() : View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Mis()
        {
            var idCliente = Identidad.IdCliente(User);
            if (idCliente is null)
            {
                return Forbid();
            }

            var citas = await _db.Citas.AsNoTracking()
                .Include(c => c.Servicio)
                .Include(c => c.Profesional)
                .Where(c => c.ClienteId == idCliente.Value)
                .OrderByDescending(c => c.Fecha).ThenByDescending(c => c.Hora)
                .ToListAsync();

            ViewData["Ahora"] = DateTime.Now;
            return View(citas);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var cita = await CargarCitaDelClienteAsync(id);
            if (cita is null)
            {
                return NotFound();
            }

            if (!CitaService.PuedeModificarse(cita, DateTime.Now))
            {
                TempData["Error"] = "Esta cita ya no se puede modificar.";
                return RedirectToAction(nameof(Mis));
            }

            var model = new RegistrarCitaViewModel
            {
                Id = cita.Id,
                ProfesionalId = cita.ProfesionalId,
                ServicioId = cita.ServicioId,
                Fecha = cita.Fecha,
                Hora = cita.Hora,
                Observaciones = cita.Observaciones
            };

            await LlenarListasAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, RegistrarCitaViewModel model)
        {
            var cita = await CargarCitaDelClienteAsync(id);
            if (cita is null)
            {
                return NotFound();
            }

            if (!CitaService.PuedeModificarse(cita, DateTime.Now))
            {
                TempData["Error"] = "Esta cita ya no se puede modificar.";
                return RedirectToAction(nameof(Mis));
            }

            model.Id = id;
            var error = await ValidarFormularioAsync(model, citaIdExcluir: id);
            if (error is not null)
            {
                await LlenarListasAsync(model);
                return View(model);
            }

            cita.ProfesionalId = model.ProfesionalId;
            cita.ServicioId = model.ServicioId!.Value;
            cita.Fecha = model.Fecha!.Value;
            cita.Hora = model.Hora!.Value;
            cita.Observaciones = NullIfEmpty(model.Observaciones);

            await _db.SaveChangesAsync();

            TempData["Mensaje"] = "Tu cita fue modificada.";
            return RedirectToAction(nameof(Confirmacion), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var cita = await CargarCitaDelClienteAsync(id);
            if (cita is null)
            {
                return NotFound();
            }

            if (!CitaService.PuedeModificarse(cita, DateTime.Now))
            {
                TempData["Error"] = "Esta cita ya no se puede cancelar.";
                return RedirectToAction(nameof(Mis));
            }

            cita.Estado = EstadoCita.Cancelada;
            await _db.SaveChangesAsync();

            TempData["Mensaje"] = "Tu cita fue cancelada.";
            return RedirectToAction(nameof(Mis));
        }

        [HttpGet]
        public async Task<IActionResult> HorasLibres(int profesionalId, int servicioId, DateOnly fecha, int? citaId)
        {
            var horas = await _citas.HorasLibresAsync(profesionalId, servicioId, fecha, citaId);
            return Json(horas.Select(h => h.ToString("HH:mm")));
        }

        private async Task<string?> ValidarFormularioAsync(RegistrarCitaViewModel model, int? citaIdExcluir)
        {
            if (!ModelState.IsValid)
            {
                return "Revisa los datos del formulario.";
            }

            var error = await _citas.ValidarAsync(
                model.ProfesionalId!.Value,
                model.ServicioId!.Value,
                model.Fecha!.Value,
                model.Hora!.Value,
                citaIdExcluir);

            if (error is not null)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return error;
        }

        private async Task LlenarListasAsync(RegistrarCitaViewModel model)
        {
            model.Servicios = await _citas.ServiciosActivosAsync();
            model.Profesionales = await _citas.ProfesionalesActivosAsync();
            model.Horas = CitaService.HorasBase.ToList();
        }

        private async Task<Cita?> CargarCitaDelClienteAsync(int id)
        {
            var idCliente = Identidad.IdCliente(User);
            if (idCliente is null)
            {
                return null;
            }

            var consulta = _db.Citas
                .Include(c => c.Servicio)
                .Include(c => c.Profesional)
                .Include(c => c.Cliente)
                .Where(c => c.Id == id && c.ClienteId == idCliente.Value);

            return await consulta.FirstOrDefaultAsync();
        }

        private static string? NullIfEmpty(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}

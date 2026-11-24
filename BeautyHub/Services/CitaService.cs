using BeautyHub.Data;
using BeautyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautyHub.Services
{
    public class CitaService
    {
        public static readonly TimeOnly[] HorasBase =
        {
            new(9, 0), new(10, 0), new(11, 0), new(12, 0),
            new(14, 0), new(15, 0), new(16, 0), new(17, 0)
        };

        private readonly BeautyHubContext _db;

        public CitaService(BeautyHubContext db)
        {
            _db = db;
        }

        public static (TimeOnly Apertura, TimeOnly Cierre)? HorarioDe(DayOfWeek dia) => dia switch
        {
            DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday
                or DayOfWeek.Thursday or DayOfWeek.Friday => (new TimeOnly(9, 0), new TimeOnly(19, 0)),
            DayOfWeek.Saturday => (new TimeOnly(9, 0), new TimeOnly(18, 0)),
            _ => null
        };

        public static bool TransicionValida(EstadoCita actual, EstadoCita nuevo) => (actual, nuevo) switch
        {
            (EstadoCita.Solicitada, EstadoCita.Confirmada) => true,
            (EstadoCita.Confirmada, EstadoCita.Atendida) => true,
            (EstadoCita.Solicitada, EstadoCita.Cancelada) => true,
            (EstadoCita.Confirmada, EstadoCita.Cancelada) => true,
            _ => false
        };

        public static bool PuedeModificarse(Cita cita, DateTime ahora) =>
            cita.Estado is EstadoCita.Solicitada or EstadoCita.Confirmada
            && cita.Fecha.ToDateTime(cita.Hora) > ahora;

        public async Task<List<ServicioOpcion>> ServiciosActivosAsync()
        {
            return await _db.Servicios.AsNoTracking()
                .Where(s => s.Activo)
                .OrderBy(s => s.Categoria).ThenBy(s => s.Nombre)
                .Select(s => new ServicioOpcion
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos
                })
                .ToListAsync();
        }

        public async Task<List<ProfesionalOpcion>> ProfesionalesActivosAsync()
        {
            return await _db.Profesionales.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombres).ThenBy(p => p.Apellidos)
                .Select(p => new ProfesionalOpcion
                {
                    Id = p.Id,
                    Nombre = p.Nombres + " " + p.Apellidos,
                    Especialidad = p.Especialidad
                })
                .ToListAsync();
        }

        public async Task<string?> ValidarAsync(int profesionalId, int servicioId, DateOnly fecha, TimeOnly hora,
            int? citaIdExcluir, DateTime? ahora = null)
        {
            var momento = ahora ?? DateTime.Now;
            var hoy = DateOnly.FromDateTime(momento);

            if (fecha < hoy)
            {
                return "La fecha no puede ser anterior a hoy.";
            }

            var horario = HorarioDe(fecha.DayOfWeek);
            if (horario is null)
            {
                return "Los domingos estamos cerrados.";
            }

            if (fecha == hoy && hora <= TimeOnly.FromDateTime(momento))
            {
                return "Esa hora ya pasó. Elige una hora posterior.";
            }

            if (!HorasBase.Contains(hora))
            {
                return "La hora elegida no es válida.";
            }

            var servicio = await _db.Servicios.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == servicioId && s.Activo);
            if (servicio is null)
            {
                return "El servicio elegido no existe o no está disponible.";
            }

            var profesionalExiste = await _db.Profesionales.AnyAsync(p => p.Id == profesionalId && p.Activo);
            if (!profesionalExiste)
            {
                return "El profesional elegido no existe o no está disponible.";
            }

            var fin = hora.AddMinutes(servicio.DuracionMinutos);
            if (hora < horario.Value.Apertura || fin > horario.Value.Cierre)
            {
                return $"Ese horario no alcanza para el servicio. Cerramos a las {horario.Value.Cierre:HH:mm}.";
            }

            var ocupadas = await CitasActivasAsync(profesionalId, fecha, citaIdExcluir);
            if (ocupadas.Any(c => hora < c.Fin && c.Inicio < fin))
            {
                return "El profesional ya tiene una cita en ese horario. Elige otra hora.";
            }

            return null;
        }

        public async Task<List<TimeOnly>> HorasLibresAsync(int profesionalId, int servicioId, DateOnly fecha,
            int? citaIdExcluir, DateTime? ahora = null)
        {
            var libres = new List<TimeOnly>();
            var momento = ahora ?? DateTime.Now;

            var horario = HorarioDe(fecha.DayOfWeek);
            var duracion = await _db.Servicios.AsNoTracking()
                .Where(s => s.Id == servicioId && s.Activo)
                .Select(s => (int?)s.DuracionMinutos)
                .FirstOrDefaultAsync();

            if (horario is null || duracion is null)
            {
                return libres;
            }

            var ocupadas = await CitasActivasAsync(profesionalId, fecha, citaIdExcluir);

            foreach (var inicio in HorasBase)
            {
                var fin = inicio.AddMinutes(duracion.Value);
                if (inicio < horario.Value.Apertura || fin > horario.Value.Cierre)
                {
                    continue;
                }

                if (fecha == DateOnly.FromDateTime(momento) && inicio <= TimeOnly.FromDateTime(momento))
                {
                    continue;
                }

                if (ocupadas.Any(c => inicio < c.Fin && c.Inicio < fin))
                {
                    continue;
                }

                libres.Add(inicio);
            }

            return libres;
        }

        private async Task<List<(TimeOnly Inicio, TimeOnly Fin)>> CitasActivasAsync(int profesionalId, DateOnly fecha,
            int? citaIdExcluir)
        {
            var filas = await _db.Citas.AsNoTracking()
                .Where(c => c.ProfesionalId == profesionalId
                            && c.Fecha == fecha
                            && c.Estado != EstadoCita.Cancelada
                            && (citaIdExcluir == null || c.Id != citaIdExcluir))
                .Select(c => new { c.Hora, Duracion = c.Servicio!.DuracionMinutos })
                .ToListAsync();

            return filas.Select(f => (f.Hora, f.Hora.AddMinutes(f.Duracion))).ToList();
        }
    }
}

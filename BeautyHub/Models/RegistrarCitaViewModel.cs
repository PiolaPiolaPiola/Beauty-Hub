using System.ComponentModel.DataAnnotations;

namespace BeautyHub.Models
{
    public class RegistrarCitaViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Elige un profesional.")]
        public int? ProfesionalId { get; set; }

        [Required(ErrorMessage = "Elige un servicio.")]
        public int? ServicioId { get; set; }

        [Required(ErrorMessage = "Elige una fecha.")]
        public DateOnly? Fecha { get; set; }

        [Required(ErrorMessage = "Elige una hora.")]
        public TimeOnly? Hora { get; set; }

        [MaxLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string? Observaciones { get; set; }

        public List<ServicioOpcion> Servicios { get; set; } = new();
        public List<ProfesionalOpcion> Profesionales { get; set; } = new();
        public List<TimeOnly> Horas { get; set; } = new();

        public bool EsModificacion => Id.HasValue;
    }
}

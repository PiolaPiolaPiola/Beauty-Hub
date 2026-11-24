using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyHub.Models
{
    public enum EstadoCita
    {
        Solicitada,
        Confirmada,
        Cancelada,
        Atendida
    }

    [Table("citas")]
    public class Cita
    {
        [Key, Column("id_cita")]
        public int Id { get; set; }

        [Column("id_cliente")]
        public int ClienteId { get; set; }

        [Column("id_servicio")]
        public int ServicioId { get; set; }

        [Column("id_profesional")]
        public int? ProfesionalId { get; set; }

        [Column("fecha_cita")]
        public DateOnly Fecha { get; set; }

        [Column("hora_cita")]
        public TimeOnly Hora { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("estado")]
        public EstadoCita Estado { get; set; } = EstadoCita.Solicitada;

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(ServicioId))]
        public Servicio? Servicio { get; set; }

        [ForeignKey(nameof(ProfesionalId))]
        public Profesional? Profesional { get; set; }
    }
}

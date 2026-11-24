using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyHub.Models
{
    [Table("profesionales")]
    public class Profesional
    {
        [Key, Column("id_profesional")]
        public int Id { get; set; }

        [Column("nombres"), Required, MaxLength(100)]
        public string Nombres { get; set; } = "";

        [Column("apellidos"), Required, MaxLength(100)]
        public string Apellidos { get; set; } = "";

        [Column("telefono"), Required, MaxLength(20)]
        public string Telefono { get; set; } = "";

        [Column("correo"), MaxLength(100)]
        public string? Correo { get; set; }

        [Column("especialidad"), MaxLength(100)]
        public string? Especialidad { get; set; }

        [Column("estado")]
        public bool Activo { get; set; } = true;

        [Column("fecha_registro"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaRegistro { get; set; }

        [NotMapped]
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}

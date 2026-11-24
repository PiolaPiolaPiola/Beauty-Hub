using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyHub.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key, Column("id_usuario")]
        public int Id { get; set; }

        [Column("usuario"), Required, MaxLength(50)]
        public string NombreUsuario { get; set; } = "";

        [Column("contrasena_hash"), Required, MaxLength(255)]
        public string ContrasenaHash { get; set; } = "";

        [Column("rol"), Required, MaxLength(20)]
        public string Rol { get; set; } = Roles.Cliente;

        [Column("id_cliente")]
        public int? ClienteId { get; set; }

        [Column("id_profesional")]
        public int? ProfesionalId { get; set; }

        [Column("estado")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(ProfesionalId))]
        public Profesional? Profesional { get; set; }
    }
}

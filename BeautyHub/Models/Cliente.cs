using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyHub.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key, Column("id_cliente")]
        public int Id { get; set; }

        [Column("nombres"), Required, MaxLength(100)]
        public string Nombres { get; set; } = "";

        [Column("apellidos"), Required, MaxLength(100)]
        public string Apellidos { get; set; } = "";

        [Column("telefono"), Required, MaxLength(20)]
        public string Telefono { get; set; } = "";

        [Column("correo"), MaxLength(100)]
        public string? Correo { get; set; }

        [Column("fecha_registro"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaRegistro { get; set; }

        public List<Cita> Citas { get; set; } = new();
    }
}

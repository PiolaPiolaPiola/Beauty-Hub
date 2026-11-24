using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyHub.Models
{
    public enum CategoriaServicio
    {
        Barberia,
        Peluqueria,
        Combo
    }

    [Table("servicios")]
    public class Servicio
    {
        [Key, Column("id_servicio")]
        public int Id { get; set; }

        [Column("nombre_servicio"), Required, MaxLength(100)]
        public string Nombre { get; set; } = "";

        [Column("categoria")]
        public CategoriaServicio Categoria { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("duracion_minutos")]
        public int DuracionMinutos { get; set; }

        [Column("estado")]
        public bool Activo { get; set; } = true;

        [Column("popular")]
        public bool Popular { get; set; }

        public List<Cita> Citas { get; set; } = new();
    }
}

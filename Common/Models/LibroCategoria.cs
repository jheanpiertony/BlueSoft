using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    [Table("LibroCategoria")]
    public class LibroCategoria
    {
        public int LibroId { get; set; }
        public int CategoriaId { get; set; }
        public Libro Libro { get; set; }
        public Categoria Categoria { get; set; }
    }
}
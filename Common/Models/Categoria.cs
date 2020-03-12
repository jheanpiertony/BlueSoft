using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        //public Categoria()
        //{
        //    LibroCategorias = new List<LibroCategoria>();
        //}
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nombre de la categoria"), MaxLength(length: 100)]
        public string NombreCategoria { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        public List<LibroCategoria> LibroCategorias { get; set; }
    }
}

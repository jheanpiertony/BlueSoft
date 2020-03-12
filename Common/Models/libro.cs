using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Models
{
    [Table("Libro")]
    public class Libro
    {
        //public Libro()
        //{
        //    LibroCategorias = new List<LibroCategoria>();
        //}
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre del libro"), MaxLength(length: 100)]
        public string NombreLibro { get; set; }

        [Required]
        [Display(Name = "ISBN"), MaxLength(length: 150)]
        public string ISBN { get; set; }

        public List<LibroCategoria> LibroCategorias { get; set; }
        public Autor Autor { get; set; }

        [Display(Name = "Autor")]
        public int AutorId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    [Table("Autor")]
    public class Autor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombres"), MaxLength(length: 100)]
        public string Nombres { get; set; }

        [Required]
        [Display(Name = "Apellidos"), MaxLength(length: 100)]
        public string Apellidos { get; set; }

        [Required]
        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        public List<Libro> Libros { get; set; }
    }
}

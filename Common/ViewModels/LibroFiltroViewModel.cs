using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Common.ViewModels
{
    public class LibroFiltroViewModel
    {
        public List<Libro> Libros { get; set; }
        public SelectList Categoria { get; set; }
        public string LibroCategoria { get; set; }
        public string FiltrarString { get; set; }
    }
}

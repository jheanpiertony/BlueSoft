using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Common.ViewModels;

namespace Web.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public string ListaCategoriasLibro = string.Empty;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;           
        }

        // GET: Libros
        public async Task<IActionResult> Index(string LibroCategoria, string searchString)
        {
            HttpContext.Session.Set<List<Categoria>>(ListaCategoriasLibro, new List<Categoria>());// Variable se seccion lista de categorias del libro.
            var applicationDbContext = _context.Libros.Include(l => l.Autor);

            IQueryable<string> categoriaQuery = _context.Categorias.OrderBy(nc => nc.NombreCategoria).Select(x => x.NombreCategoria);

            var listaLibros = from m in _context.Libros
                              select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                listaLibros = listaLibros.Where(s => s.NombreLibro.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(LibroCategoria))
            {
                var categoria = _context.Categorias.Where(nc => nc.NombreCategoria.Contains(LibroCategoria)).First();
                var _listaLibrosCategoria = _context.LibroCategorias.Where(x => x.CategoriaId == categoria.Id).Include(l => l.Libro).Select(l => l.Libro);
                listaLibros = _listaLibrosCategoria;
            }

            var libroFiltroViewModel = new LibroFiltroViewModel
            {
                Categoria = new SelectList(await categoriaQuery.Distinct().ToListAsync()),
                Libros = await listaLibros.ToListAsync(),
            };

            return View(libroFiltroViewModel);


            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.ListaCategoriasLibro = _context.LibroCategorias.Where(x => x.LibroId == libro.Id).Include(x => x.Categoria).Select(x => x.Categoria).ToList();

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            ViewData["AutorId"] = new SelectList(_context.Autores.OrderBy(n => n.Nombres).ThenBy(a => a.Apellidos), "Id", "Apellidos");
            ViewBag.CategoriaId = new SelectList(_context.Categorias.OrderBy(nc => nc.NombreCategoria), "Id", "NombreCategoria");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreLibro,ISBN,AutorId")] Libro libro)
        {
            var _listaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);

            if (_listaCategoriasLibro == null || _listaCategoriasLibro.Count == 0 || !ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "El libro debe tener al menos una categoría asignada!");
                ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
                ViewData["AutorId"] = new SelectList(_context.Autores.OrderBy(n => n.Nombres).ThenBy(a => a.Apellidos), "Id", "Apellidos");
                ViewBag.CategoriaId = new SelectList(_context.Categorias.OrderBy(nc => nc.NombreCategoria), "Id", "NombreCategoria");
                return View(libro);
            }
            else
            {
                List<LibroCategoria> listaLibroCategorias = new List<LibroCategoria>();
                _context.Libros.Add(libro);
                _context.SaveChanges();

                foreach (Categoria item in _listaCategoriasLibro)
                {
                    listaLibroCategorias.Add(new LibroCategoria() 
                    {
                        LibroId = libro.Id,
                        CategoriaId = item.Id,
                    });
                }

                _context.LibroCategorias.AddRange(listaLibroCategorias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }            
        }
        [HttpPost]
        public IActionResult AsignarCategoriasLibro(int CategoriaId)
        {
            var categoria = _context.Categorias.Find(CategoriaId);
            var _listaCategoriasLibro =  HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            
            if (!_listaCategoriasLibro.Any(x => x.Id == categoria.Id)) 
            { 
                _listaCategoriasLibro.Add(categoria);
            }
            
            HttpContext.Session.Set<List<Categoria>>(ListaCategoriasLibro, _listaCategoriasLibro.OrderBy(nc => nc.NombreCategoria).ToList());
            ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            return PartialView("_ListaCategoriasLibro", ViewBag.ListaCategoriasLibro);
        }
        [HttpPost]
        public IActionResult EliminarAsignarCategoriasLibro(int Id)
        {
            var categoria = _context.Categorias.Find(Id);
            var _listaCategoriasLibro =  HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            
            if (_listaCategoriasLibro.Any(x => x.Id == categoria.Id)) 
            { 
                _listaCategoriasLibro.RemoveAll(x => x.Id == Id);
            }
            
            HttpContext.Session.Set<List<Categoria>>(ListaCategoriasLibro, _listaCategoriasLibro.OrderBy(nc => nc.NombreCategoria).ToList());
            ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            return PartialView("_ListaCategoriasLibro", ViewBag.ListaCategoriasLibro);
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            var listaCategorias = _context.LibroCategorias
                .Where(li => li.LibroId == libro.Id)
                .Include(c => c.Categoria)
                .Select(x => x.Categoria).ToList();
            
            HttpContext.Session.Set<List<Categoria>>(ListaCategoriasLibro, listaCategorias);
            ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
            ViewData["AutorId"] = new SelectList(_context.Autores.OrderBy(n => n.Nombres).ThenBy(a => a.Apellidos), "Id", "Apellidos");
            ViewBag.CategoriaId = new SelectList(_context.Categorias.OrderBy(nc => nc.NombreCategoria), "Id", "NombreCategoria");
            return View(libro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreLibro,ISBN,AutorId")] Libro libro)
        {

            if (id != libro.Id)
            {
                return NotFound();
            }

            var _listaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);

            if (_listaCategoriasLibro == null || _listaCategoriasLibro.Count == 0 || !ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "El libro debe tener al menos una categoría asignada!");
                ViewBag.ListaCategoriasLibro = HttpContext.Session.Get<List<Categoria>>(ListaCategoriasLibro);
                ViewData["AutorId"] = new SelectList(_context.Autores.OrderBy(n => n.Nombres).ThenBy(a => a.Apellidos), "Id", "Apellidos");
                ViewBag.CategoriaId = new SelectList(_context.Categorias.OrderBy(nc => nc.NombreCategoria), "Id", "NombreCategoria");
                return View(libro);
            }
            else
            {
                List<LibroCategoria> listaLibroCategorias = new List<LibroCategoria>();

                _context.Libros.Update(libro);

                var libroCategoriasEliminar = _context.LibroCategorias.Where(li => li.LibroId == libro.Id).ToList();
                _context.LibroCategorias.RemoveRange(libroCategoriasEliminar);

                foreach (Categoria item in _listaCategoriasLibro)
                {
                    listaLibroCategorias.Add(new LibroCategoria()
                    {
                        LibroId = libro.Id,
                        CategoriaId = item.Id,
                    });
                }

                _context.LibroCategorias.AddRange(listaLibroCategorias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.ListaCategoriasLibro = _context.LibroCategorias.Where(x => x.LibroId == libro.Id).Include(x => x.Categoria).Select(x => x.Categoria).ToList();

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Id == id);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}

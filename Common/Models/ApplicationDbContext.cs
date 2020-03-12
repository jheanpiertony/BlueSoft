using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Common.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LibroCategoria>()
                .HasKey(x => new
                {
                    x.LibroId,
                    x.CategoriaId
                });
            builder.Entity<Libro>()
                .HasIndex(np => np.ISBN)
                .IsUnique();
            //Seed
            //builder.Entity<Usuario>().HasData(
            //    new Usuario() 
            //    {
            //    },
            //    new Usuario()
            //    {
            //    });

        }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<LibroCategoria> LibroCategorias { get; set; }
    }
}

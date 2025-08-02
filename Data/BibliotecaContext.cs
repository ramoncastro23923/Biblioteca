using Microsoft.EntityFrameworkCore;
using Biblioteca.Models;

namespace Biblioteca.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {
        }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais do modelo
            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Livro)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Usuario)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Seed inicial
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Admin",
                    Email = "admin@biblioteca.com",
                    Telefone = "11999999999",
                    Senha = "admin123", // Na prática, deve ser hash
                    TipoPerfil = TipoPerfil.Administrador
                }
            );
        }
    }
}
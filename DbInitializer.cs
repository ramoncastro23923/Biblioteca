using Biblioteca.Models;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BibliotecaContext context)
        {
            context.Database.EnsureCreated();

            // Seed de livros
            if (!context.Livros.Any())
            {
                var livros = new Livro[]
                {
                    new Livro { 
                        Titulo = "Dom Casmurro", 
                        Autor = "Machado de Assis", 
                        Editora = "Martin Claret", 
                        AnoPublicacao = 1899, 
                        ISBN = "9788572326979", 
                        QuantidadeDisponivel = 5 
                    },
                    new Livro { 
                        Titulo = "O Senhor dos Anéis", 
                        Autor = "J.R.R. Tolkien", 
                        Editora = "Martins Fontes", 
                        AnoPublicacao = 1954, 
                        ISBN = "9788533613379", 
                        QuantidadeDisponivel = 3 
                    },
                    new Livro { 
                        Titulo = "1984", 
                        Autor = "George Orwell", 
                        Editora = "Companhia das Letras", 
                        AnoPublicacao = 1949, 
                        ISBN = "9788535902779", 
                        QuantidadeDisponivel = 2 
                    }
                };

                context.Livros.AddRange(livros);
            }

            // Seed do usuário admin
            if (!context.Usuarios.Any(u => u.Email == "admin@biblioteca.com"))
            {
                var hasher = new PasswordHasher<Usuario>();
                var admin = new Usuario
                {
                    Nome = "Admin Principal",
                    Email = "admin@biblioteca.com",
                    Telefone = "11999999999",
                    Senha = hasher.HashPassword(null, "Admin123@"),
                    TipoPerfil = TipoPerfil.Administrador
                };

                context.Usuarios.Add(admin);
            }

            context.SaveChanges();
        }
    }
}
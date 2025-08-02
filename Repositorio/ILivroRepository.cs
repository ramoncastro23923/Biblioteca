using System.Collections.Generic;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositorio
{
    public interface ILivroRepository
    {
        IEnumerable<Livro> GetAll();
        Livro GetById(int id);
        IEnumerable<Livro> Search(string termo);
        void Add(Livro livro);
        void Update(Livro livro);
        void Delete(int id);
        bool Exists(int id);
    }
}
using System.Collections.Generic;
using Biblioteca.Models;

namespace Biblioteca.Repositorio
{
    public interface IUsuarioRepository
    {
        IEnumerable<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario GetByEmail(string email);
        void Add(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(int id);
        bool Exists(int id);
        Usuario Authenticate(string email, string senha);
    }
}
using System.Collections.Generic;
using Biblioteca.Models;
using System.Threading.Tasks;

namespace Biblioteca.Repositorio
    {
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByEmailAsync(string email);
        Task<Usuario> AuthenticateAsync(string email, string senha);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<Usuario?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
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
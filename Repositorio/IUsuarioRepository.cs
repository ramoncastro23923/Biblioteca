using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Models;

namespace Biblioteca.Repositorio
{
    public interface IUsuarioRepository
    {
        // Métodos Assíncronos
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task<Usuario> AuthenticateAsync(string email, string senha);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Métodos Síncronos (opcionais)
        IEnumerable<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario GetByEmail(string email);
        void Add(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(int id);
        bool Exists(int id);
        Usuario Authenticate(string email, string senha);
        IQueryable<Usuario> GetAllQueryable();
        
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Models;

namespace Biblioteca.Repositorio
{
    public interface ILocacaoRepository
    {
        Task<ICollection<Locacao>> GetAllWithDetailsAsync();
        Task<ICollection<Locacao>> GetByUsuarioWithDetailsAsync(int usuarioId);
        Task<Locacao> GetByIdWithDetailsAsync(int id);
        Task<bool> LivroDisponivelAsync(int livroId);
        Task<bool> UsuarioPodeLocarAsync(int usuarioId);
        Task AddAsync(Locacao locacao);
        Task DevolverAsync(int locacaoId);
        Task RenovarAsync(int locacaoId);
        Task<IEnumerable<Locacao>> GetPendentesAsync();
        
    }
}
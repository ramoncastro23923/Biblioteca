using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Models;

namespace Biblioteca.Repositorio
{
    public interface ILivroRepository
    {
        Task<IEnumerable<Livro>> GetDisponiveisAsync();
        IEnumerable<Livro> GetAll();
        Livro GetById(int id);
        IEnumerable<Livro> Search(string termo);
        void Add(Livro livro);
        void Update(Livro livro);
        void Delete(int id);
        bool Exists(int id);
        Task<IEnumerable<Livro>> GetAllAsync();
        
        // Adicione este método
        Task<IEnumerable<RelatorioLivrosMaisLocados>> GetLivrosMaisLocadosAsync(DateTime? dataInicio, DateTime? dataFim);
    }
}
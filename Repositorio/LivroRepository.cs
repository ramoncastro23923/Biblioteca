using System.Collections.Generic;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositorio
{
    public class LivroRepository : ILivroRepository
    {
        private readonly BibliotecaContext _context;

        public LivroRepository(BibliotecaContext context)
        {
            _context = context;
        }

public async Task<IEnumerable<Livro>> GetAllAsync()
{
    return await _context.Livros.ToListAsync();
}

        public IEnumerable<Livro> GetAll()
        {
            return _context.Livros.ToList();
        }

        public Livro GetById(int id)
        {
            return _context.Livros.FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<Livro> Search(string termo)
        {
            return _context.Livros.Where(l =>
                l.Titulo.Contains(termo) ||
                l.Autor.Contains(termo) ||
                l.ISBN.Contains(termo)).ToList();
        }

        public void Add(Livro livro)
        {
            _context.Livros.Add(livro);
            _context.SaveChanges();
        }

        public void Update(Livro livro)
        {
            _context.Livros.Update(livro);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var livro = GetById(id);
            if (livro != null)
            {
                _context.Livros.Remove(livro);
                _context.SaveChanges();
            }
        }

       public async Task<IEnumerable<RelatorioLivrosMaisLocados>> GetLivrosMaisLocadosAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Locacoes
                .Include(l => l.Livro)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataRetirada >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataRetirada <= dataFim.Value);

            var totalLocacoes = await query.CountAsync();

            return await query
                .GroupBy(l => new { l.Livro.Id, l.Livro.Titulo })
                .Select(g => new RelatorioLivrosMaisLocados
                {
                    LivroId = g.Key.Id,
                    Titulo = g.Key.Titulo,
                    TotalLocacoes = g.Count(),
                    Percentual = totalLocacoes > 0 ? (decimal)g.Count() / totalLocacoes * 100 : 0
                })
                .OrderByDescending(r => r.TotalLocacoes)
                .ToListAsync();
        }

        public IQueryable<Livro> GetAllQueryable()
        {
            return _context.Livros.AsQueryable();
        }
        public bool Exists(int id)
        {
            return _context.Livros.Any(l => l.Id == id);
        }
        public async Task<IEnumerable<Livro>> GetDisponiveisAsync()
{
    return await _context.Livros
        .Where(l => l.QuantidadeDisponivel > 0)
        .ToListAsync();
}
    }
}
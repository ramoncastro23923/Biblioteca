using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Models;
using Biblioteca.Data;

namespace Biblioteca.Repositorio
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly BibliotecaContext _context;

        public LocacaoRepository(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Locacao>> GetAllWithDetailsAsync()
        {
            return await _context.Locacoes
                .Include(l => l.Livro)
                .Include(l => l.Usuario)
                .Where(l => l.Status == StatusLocacao.Pendente || l.Status == StatusLocacao.Atrasado)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Locacao>> GetByUsuarioWithDetailsAsync(int usuarioId)
        {
            return await _context.Locacoes
                .Include(l => l.Livro)
                .Where(l => l.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Locacao> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Locacoes
                .Include(l => l.Livro)
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> LivroDisponivelAsync(int livroId)
        {
            var livro = await _context.Livros.FindAsync(livroId);
            return livro != null && livro.QuantidadeDisponivel > 0;
        }

        public async Task<bool> UsuarioPodeLocarAsync(int usuarioId)
        {
            var locacoesPendentes = await _context.Locacoes
                .CountAsync(l => l.UsuarioId == usuarioId &&
                               (l.Status == StatusLocacao.Pendente || l.Status == StatusLocacao.Atrasado));
            return locacoesPendentes < 5;
        }

        public async Task AddAsync(Locacao locacao)
        {
            var livro = await _context.Livros.FindAsync(locacao.LivroId);
            if (livro != null && livro.QuantidadeDisponivel > 0)
            {
                livro.QuantidadeDisponivel--;
                _context.Livros.Update(livro);

                locacao.Status = StatusLocacao.Pendente;
                await _context.Locacoes.AddAsync(locacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DevolverAsync(int locacaoId)
        {
            var locacao = await GetByIdWithDetailsAsync(locacaoId);
            if (locacao != null)
            {
                var livro = await _context.Livros.FindAsync(locacao.LivroId);
                if (livro != null)
                {
                    livro.QuantidadeDisponivel++;
                    _context.Livros.Update(livro);
                }

                locacao.DataDevolucaoReal = DateTime.Now;
                locacao.Status = StatusLocacao.Devolvido;

                if (locacao.DataDevolucaoReal > locacao.DataDevolucaoPrevista)
                {
                    var diasAtraso = (locacao.DataDevolucaoReal.Value - locacao.DataDevolucaoPrevista).Days;
                    locacao.Multa = diasAtraso * 2.50m;
                }

                _context.Locacoes.Update(locacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RenovarAsync(int locacaoId)
        {
            var locacao = await GetByIdWithDetailsAsync(locacaoId);
            if (locacao != null && locacao.PodeRenovar)
            {
                locacao.DataDevolucaoPrevista = locacao.DataDevolucaoPrevista.AddDays(14);
                locacao.PodeRenovar = false;
                _context.Locacoes.Update(locacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Locacao>> GetPendentesAsync()
        {
            return await _context.Locacoes
                .Where(l => l.Status == StatusLocacao.Pendente)
                .ToListAsync();
        }

        public async Task<IEnumerable<RelatorioUsuariosMaisAtivos>> GetUsuariosMaisAtivosAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Locacoes
                .Include(l => l.Usuario)
                .AsQueryable();

            // Aplicar filtros de data
            if (dataInicio.HasValue)
                query = query.Where(l => l.DataRetirada >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataRetirada <= dataFim.Value);

            var totalLocacoesPeriodo = await query.CountAsync();

            var resultado = await query
                .GroupBy(l => new { l.Usuario.Id, l.Usuario.Nome, l.Usuario.Email })
                .Select(g => new RelatorioUsuariosMaisAtivos
                {
                    UsuarioId = g.Key.Id,
                    Nome = g.Key.Nome,
                    Email = g.Key.Email,
                    TotalLocacoes = g.Count(),
                    Percentual = totalLocacoesPeriodo > 0 ?
                        (decimal)g.Count() / totalLocacoesPeriodo * 100 : 0
                })
                .OrderByDescending(r => r.TotalLocacoes)
                .Take(20)
                .ToListAsync();

            return resultado;
        }

        public async Task<IEnumerable<RelatorioLivrosMaisLocados>> GetLivrosMaisLocadosAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Locacoes
                .Include(l => l.Livro)
                .AsQueryable();

            // Aplicar filtros de data se fornecidos
            if (dataInicio.HasValue)
                query = query.Where(l => l.DataRetirada >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataRetirada <= dataFim.Value);

            var totalLocacoesPeriodo = await query.CountAsync();

            var resultado = await query
                .GroupBy(l => new { l.Livro.Id, l.Livro.Titulo, l.Livro.Autor })
                .Select(g => new RelatorioLivrosMaisLocados
                {
                    LivroId = g.Key.Id,
                    Titulo = g.Key.Titulo,
                    Autor = g.Key.Autor,
                    TotalLocacoes = g.Count(),
                    Percentual = totalLocacoesPeriodo > 0 ?
                        (decimal)g.Count() / totalLocacoesPeriodo * 100 : 0
                })
                .OrderByDescending(r => r.TotalLocacoes)
                .Take(20)
                .ToListAsync();

            return resultado;
        }
    }
}
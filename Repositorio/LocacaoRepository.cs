using System;
using System.Collections.Generic;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositorio
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly BibliotecaContext _context;

        public LocacaoRepository(BibliotecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Locacao> GetAll()
        {
            return _context.Locacoes
                .Include(l => l.Livro)
                .Include(l => l.Usuario)
                .ToList();
        }

        public Locacao GetById(int id)
        {
            return _context.Locacoes
                .Include(l => l.Livro)
                .Include(l => l.Usuario)
                .FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<Locacao> GetByUsuario(int usuarioId)
        {
            return _context.Locacoes
                .Include(l => l.Livro)
                .Where(l => l.UsuarioId == usuarioId)
                .ToList();
        }

        public IEnumerable<Locacao> GetPendentes()
        {
            return _context.Locacoes
                .Include(l => l.Livro)
                .Include(l => l.Usuario)
                .Where(l => l.Status == StatusLocacao.Pendente || l.Status == StatusLocacao.Atrasado)
                .ToList();
        }

        public void Add(Locacao locacao)
        {
            var livro = _context.Livros.Find(locacao.LivroId);
            if (livro != null && livro.QuantidadeDisponivel > 0)
            {
                livro.QuantidadeDisponivel--;
                _context.Livros.Update(livro);
                
                locacao.Status = StatusLocacao.Pendente;
                _context.Locacoes.Add(locacao);
                _context.SaveChanges();
            }
        }

        public void Update(Locacao locacao)
        {
            _context.Locacoes.Update(locacao);
            _context.SaveChanges();
        }

        public void Devolver(int locacaoId)
        {
            var locacao = GetById(locacaoId);
            if (locacao != null)
            {
                var livro = _context.Livros.Find(locacao.LivroId);
                if (livro != null)
                {
                    livro.QuantidadeDisponivel++;
                    _context.Livros.Update(livro);
                }

                locacao.DataDevolucaoReal = DateTime.Now;
                locacao.Status = StatusLocacao.Devolvido;

                // Calcular multa se houver atraso
                if (locacao.DataDevolucaoReal > locacao.DataDevolucaoPrevista)
                {
                    var diasAtraso = (locacao.DataDevolucaoReal.Value - locacao.DataDevolucaoPrevista).Days;
                    locacao.Multa = diasAtraso * 2.50m; // R$ 2,50 por dia de atraso
                }

                _context.Locacoes.Update(locacao);
                _context.SaveChanges();
            }
        }

        public void Renovar(int locacaoId)
        {
            var locacao = GetById(locacaoId);
            if (locacao != null && locacao.PodeRenovar)
            {
                locacao.DataDevolucaoPrevista = locacao.DataDevolucaoPrevista.AddDays(14);
                locacao.PodeRenovar = false; // Permite apenas uma renovação
                _context.Locacoes.Update(locacao);
                _context.SaveChanges();
            }
        }

        public bool LivroDisponivel(int livroId)
        {
            var livro = _context.Livros.Find(livroId);
            return livro != null && livro.QuantidadeDisponivel > 0;
        }

        public bool UsuarioPodeLocar(int usuarioId)
        {
            // Verifica se o usuário tem menos de 5 locações pendentes
            return _context.Locacoes.Count(l => 
                l.UsuarioId == usuarioId && 
                (l.Status == StatusLocacao.Pendente || l.Status == StatusLocacao.Atrasado)) < 5;
        }
    }
}
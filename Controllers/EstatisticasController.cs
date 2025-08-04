using Biblioteca.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Biblioteca.Repositorio;
using Biblioteca.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class EstatisticasController : Controller
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly BibliotecaContext _context;

        public EstatisticasController(
            ILocacaoRepository locacaoRepository,
            ILivroRepository livroRepository,
            IUsuarioRepository usuarioRepository,
            BibliotecaContext context)
        {
            _locacaoRepository = locacaoRepository;
            _livroRepository = livroRepository;
            _usuarioRepository = usuarioRepository;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var locacoes = await _context.Locacoes.ToListAsync();

            var model = new DashboardViewModel
            {
                StatusLocacoes = new[]
                {
                    new StatusLocacaoChart
                    {
                        Status = "Pendentes",
                        Quantidade = locacoes.Count(l => l.Status == StatusLocacao.Pendente),
                        Cor = "#4e73df" // Azul
                    },
                    new StatusLocacaoChart
                    {
                        Status = "Devolvidos",
                        Quantidade = locacoes.Count(l => l.Status == StatusLocacao.Devolvido),
                        Cor = "#1cc88a" // Verde
                    },
                    new StatusLocacaoChart
                    {
                        Status = "Atrasados",
                        Quantidade = locacoes.Count(l => l.Status == StatusLocacao.Atrasado),
                        Cor = "#e74a3b" // Vermelho
                    }
                },
                LocacoesPorMes = await GetLocacoesUltimosMeses(6),
                TopLivros = await _locacaoRepository.GetLivrosMaisLocadosAsync(DateTime.Now.AddMonths(-6), DateTime.Now),
                TotalLivros = await _livroRepository.GetAllQueryable().CountAsync(),
                LocacoesAtivas = (await _locacaoRepository.GetPendentesAsync()).Count(),
                LocacoesAtrasadas = await _context.Locacoes.CountAsync(l => l.Status == StatusLocacao.Atrasado),
                MultasPendentes = await _context.Locacoes
                            .Where(l => l.Status == StatusLocacao.Atrasado && l.Multa > 0)
                            .SumAsync(l => l.Multa)
            };

            return View(model);
        }

        private async Task<IEnumerable<LocacoesPorMes>> GetLocacoesUltimosMeses(int meses)
        {
            var dataLimite = DateTime.Now.AddMonths(-meses);
            
            return await _context.Locacoes
                .Where(l => l.DataRetirada >= dataLimite)
                .GroupBy(l => new { l.DataRetirada.Year, l.DataRetirada.Month })
                .Select(g => new LocacoesPorMes
                {
                    MesAno = $"{g.Key.Month:00}/{g.Key.Year}",
                    Quantidade = g.Count()
                })
                .OrderBy(x => x.MesAno)
                .ToListAsync();
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EstatisticasGerais()
        {
            var model = new EstatisticasGeraisViewModel
            {
                TotalLivros = await _livroRepository.GetAllQueryable().CountAsync(),
                TotalUsuarios = await _usuarioRepository.GetAllQueryable().CountAsync(),
                LocacoesAtivas = (await _locacaoRepository.GetPendentesAsync()).Count(),
                LocacoesAtrasadas = await _context.Locacoes
                    .CountAsync(l => l.Status == StatusLocacao.Atrasado),
                MultasPendentes = await _context.Locacoes
                    .Where(l => l.Status == StatusLocacao.Atrasado && l.Multa > 0)
                    .SumAsync(l => l.Multa)
            };

            return View(model);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RelatorioMultas()
        {
            var multas = await _context.Locacoes
                .Include(l => l.Usuario)
                .Include(l => l.Livro)
                .Where(l => l.Multa > 0 && !l.DataDevolucaoReal.HasValue)
                .OrderByDescending(l => l.Multa)
                .ToListAsync();

            var totalMultas = multas.Sum(l => l.Multa);

            ViewBag.TotalMultas = totalMultas;
            return View(multas);
        }
    }
}
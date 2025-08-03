using Biblioteca.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Biblioteca.Repositorio;
using Biblioteca.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
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

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EstatisticasGerais()
        {
            var model = new EstatisticasGeraisViewModel
            {
                TotalLivros = await _livroRepository.GetAllQueryable().CountAsync(),
                TotalUsuarios = await _usuarioRepository.GetAllQueryable().CountAsync(),
                LocacoesAtivas = (await _locacaoRepository.GetPendentesAsync()).Count(),
                LocacoesAtrasadas = await _context.Locacoes
                    .CountAsync(l => l.Status == StatusLocacao.Atrasado)
            };

            return View(model);
        }
    }
}
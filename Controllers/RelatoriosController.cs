using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biblioteca.Repositorio;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RelatoriosController : Controller
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public RelatoriosController(
            ILocacaoRepository locacaoRepository,
            ILivroRepository livroRepository,
            IUsuarioRepository usuarioRepository)
        {
            _locacaoRepository = locacaoRepository;
            _livroRepository = livroRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IActionResult> LivrosMaisLocados()
        {
            var livros = await _locacaoRepository.GetLivrosMaisLocadosAsync(null, null);
            return View(livros);
        }

        public async Task<IActionResult> UsuariosMaisAtivos(DateTime? dataInicio, DateTime? dataFim)
        {
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            var usuarios = await _locacaoRepository.GetUsuariosMaisAtivosAsync(dataInicio, dataFim);
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> LivrosMaisLocados(RelatorioFiltroViewModel filtro)
        {
            var livros = await _locacaoRepository.GetLivrosMaisLocadosAsync(
                filtro.DataInicio, 
                filtro.DataFim);
                
            return View(livros);
        }

        public IActionResult EstatisticasGerais()
        {
            var viewModel = new EstatisticasGeraisViewModel {
                TotalLivros = _livroRepository.GetAll().Count(),
                TotalUsuarios = _usuarioRepository.GetAll().Count(),
            };
            
            return View(viewModel);
        }
    }
}
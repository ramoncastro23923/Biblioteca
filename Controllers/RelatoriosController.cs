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

        public RelatoriosController(ILocacaoRepository locacaoRepository)
        {
            _locacaoRepository = locacaoRepository;
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
        public IActionResult UsuariosMaisAtivosPost(DateTime? dataInicio, DateTime? dataFim)
        {
            return RedirectToAction("UsuariosMaisAtivos", new { dataInicio, dataFim });
        }

        [HttpPost]
        public async Task<IActionResult> LivrosMaisLocados(RelatorioFiltroViewModel filtro)
        {
            var livros = await _locacaoRepository.GetLivrosMaisLocadosAsync(
                filtro.DataInicio, 
                filtro.DataFim);
                
            return View(livros);
        }
    }
}
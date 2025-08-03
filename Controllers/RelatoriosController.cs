using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biblioteca.Repositorio;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
   [Authorize(Roles = "Administrador")]
    public class RelatoriosController : Controller
    {        private readonly ILocacaoRepository _locacaoRepository;

        public RelatoriosController(ILocacaoRepository locacaoRepository)
        {
            _locacaoRepository = locacaoRepository;
        }

        public async Task<IActionResult> LivrosMaisLocados()
        {
            var livros = await _locacaoRepository.GetLivrosMaisLocadosAsync(null, null);
            return View(livros);
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
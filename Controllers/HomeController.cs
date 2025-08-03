using Microsoft.AspNetCore.Mvc;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILivroRepository _livroRepository;
        private readonly ILocacaoRepository _locacaoRepository;

        public HomeController(ILivroRepository livroRepository, ILocacaoRepository locacaoRepository)
        {
            _livroRepository = livroRepository;
            _locacaoRepository = locacaoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var livros = await _livroRepository.GetAllAsync();
            var locacoesPendentes = await _locacaoRepository.GetPendentesAsync();
            
            ViewBag.TotalLivros = livros.Count();
            ViewBag.LocacoesPendentes = locacoesPendentes.Count();
            
            return View();
        }
    }
}
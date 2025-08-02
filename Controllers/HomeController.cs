using Microsoft.AspNetCore.Mvc;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authorization;

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

        public IActionResult Index()
        {
            ViewBag.TotalLivros = _livroRepository.GetAll().Count();
            ViewBag.LocacoesPendentes = _locacaoRepository.GetPendentes().Count();
            return View();
        }
    }
}
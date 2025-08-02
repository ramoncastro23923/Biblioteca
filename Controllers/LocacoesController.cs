using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering; // Adicionado para usar SelectList

namespace Biblioteca.Controllers
{
    [Authorize]
    public class LocacoesController : Controller
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public LocacoesController(
            ILocacaoRepository locacaoRepository,
            ILivroRepository livroRepository,
            IUsuarioRepository usuarioRepository)
        {
            _locacaoRepository = locacaoRepository;
            _livroRepository = livroRepository;
            _usuarioRepository = usuarioRepository;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Administrador"))
            {
                return View(_locacaoRepository.GetAll());
            }
            else
            {
                var usuario = _usuarioRepository.GetByEmail(User.Identity.Name);
                return View(_locacaoRepository.GetByUsuario(usuario.Id));
            }
        }

        public IActionResult Create()
        {
            // Obter apenas livros disponíveis
            var livrosDisponiveis = _livroRepository.GetAll()
                                .Where(l => l.QuantidadeDisponivel > 0)
                                .ToList();
            
            // Criar SelectList explicitamente
            ViewBag.Livros = new SelectList(livrosDisponiveis, "Id", "Titulo");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Locacao locacao)
        {
            var usuario = _usuarioRepository.GetByEmail(User.Identity.Name);
            if (usuario == null) return NotFound();

            if (!_locacaoRepository.LivroDisponivel(locacao.LivroId))
            {
                ModelState.AddModelError("", "Livro indisponível para locação");
            }

            if (!_locacaoRepository.UsuarioPodeLocar(usuario.Id))
            {
                ModelState.AddModelError("", "Você atingiu o limite de locações pendentes");
            }

            if (ModelState.IsValid)
            {
                locacao.UsuarioId = usuario.Id;
                _locacaoRepository.Add(locacao);
                return RedirectToAction(nameof(Index));
            }

            // Recarregar a lista de livros disponíveis em caso de erro
            var livrosDisponiveis = _livroRepository.GetAll()
                                .Where(l => l.QuantidadeDisponivel > 0)
                                .ToList();
            ViewBag.Livros = new SelectList(livrosDisponiveis, "Id", "Titulo");
            return View(locacao);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Devolver(int id)
        {
            var locacao = _locacaoRepository.GetById(id);
            if (locacao == null)
            {
                return NotFound();
            }
            return View(locacao);
        }

        [HttpPost, ActionName("Devolver")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public IActionResult DevolverConfirmed(int id)
        {
            _locacaoRepository.Devolver(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Renovar(int id)
        {
            var locacao = _locacaoRepository.GetById(id);
            if (locacao == null)
            {
                return NotFound();
            }
            return View(locacao);
        }

        [HttpPost, ActionName("Renovar")]
        [ValidateAntiForgeryToken]
        public IActionResult RenovarConfirmed(int id)
        {
            _locacaoRepository.Renovar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class LivrosController : Controller
    {
        private readonly ILivroRepository _livroRepository;

        public LivrosController(ILivroRepository livroRepository)
        {
            _livroRepository = livroRepository;
        }

        public IActionResult Index(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return View(_livroRepository.Search(searchTerm));
            }
            return View(_livroRepository.GetAll());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Livro livro)
        {
            if (ModelState.IsValid)
            {
                _livroRepository.Add(livro);
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var livro = _livroRepository.GetById(id);
            if (livro == null)
            {
                return NotFound();
            }
            return View(livro);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Livro livro)
        {
            if (id != livro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _livroRepository.Update(livro);
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var livro = _livroRepository.GetById(id);
            if (livro == null)
            {
                return NotFound();
            }
            return View(livro);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _livroRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
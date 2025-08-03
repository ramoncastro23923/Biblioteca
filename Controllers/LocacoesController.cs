using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteca.Repositorio;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var usuario = await _usuarioRepository.GetByIdAsync(userId);

                if (usuario == null)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["ErrorMessage"] = "Sessão inválida. Por favor, faça login novamente.";
                    return RedirectToAction("Login", "Account");
                }

                ICollection<Locacao> locacoes;
                
                if (User.IsInRole("Administrador"))
                {
                    locacoes = await _locacaoRepository.GetAllWithDetailsAsync();
                }
                else
                {
                    locacoes = await _locacaoRepository.GetByUsuarioWithDetailsAsync(userId);
                }
                
                return View(locacoes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar locações: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var livrosDisponiveis = await _livroRepository.GetDisponiveisAsync();
                ViewBag.Livros = new SelectList(livrosDisponiveis, "Id", "Titulo");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar livros: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Locacao locacao)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var usuario = await _usuarioRepository.GetByIdAsync(userId);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado";
                    return RedirectToAction("Login", "Account");
                }

                if (!await _locacaoRepository.LivroDisponivelAsync(locacao.LivroId))
                {
                    ModelState.AddModelError("", "Livro indisponível para locação");
                }

                if (!await _locacaoRepository.UsuarioPodeLocarAsync(userId))
                {
                    ModelState.AddModelError("", "Você atingiu o limite de locações pendentes");
                }

                if (ModelState.IsValid)
                {
                    locacao.UsuarioId = userId;
                    await _locacaoRepository.AddAsync(locacao);
                    TempData["SuccessMessage"] = "Locação realizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                var livrosDisponiveis = await _livroRepository.GetDisponiveisAsync();
                ViewBag.Livros = new SelectList(livrosDisponiveis, "Id", "Titulo");
                
                return View(locacao);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao criar locação: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Devolver(int id)
        {
            try
            {
                var locacao = await _locacaoRepository.GetByIdWithDetailsAsync(id);
                if (locacao == null)
                {
                    TempData["ErrorMessage"] = "Locação não encontrada";
                    return RedirectToAction(nameof(Index));
                }
                return View(locacao);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar locação: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        [ActionName("Devolver")]
        public async Task<IActionResult> DevolverConfirmed(int id)
        {
            try
            {
                await _locacaoRepository.DevolverAsync(id);
                TempData["SuccessMessage"] = "Livro devolvido com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao devolver livro: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Renovar(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var locacao = await _locacaoRepository.GetByIdWithDetailsAsync(id);

                if (locacao == null)
                {
                    TempData["ErrorMessage"] = "Locação não encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (locacao.UsuarioId != userId && !User.IsInRole("Administrador"))
                {
                    TempData["ErrorMessage"] = "Você não tem permissão para renovar esta locação";
                    return RedirectToAction(nameof(Index));
                }

                if (!locacao.PodeRenovar)
                {
                    TempData["ErrorMessage"] = "Esta locação não pode ser renovada";
                    return RedirectToAction(nameof(Index));
                }

                return View(locacao);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar locação: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> UsuariosMaisAtivos()
{
    var usuarios = await _locacaoRepository.GetUsuariosMaisAtivosAsync(null, null);
    return View(usuarios);
}

[HttpPost]
public async Task<IActionResult> UsuariosMaisAtivos(RelatorioFiltroViewModel filtro)
{
    var usuarios = await _locacaoRepository.GetUsuariosMaisAtivosAsync(
        filtro.DataInicio, 
        filtro.DataFim);
        
    return View(usuarios);
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Renovar")]
        public async Task<IActionResult> RenovarConfirmed(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var locacao = await _locacaoRepository.GetByIdWithDetailsAsync(id);

                if (locacao != null && (locacao.UsuarioId == userId || User.IsInRole("Administrador")))
                {
                    await _locacaoRepository.RenovarAsync(id);
                    TempData["SuccessMessage"] = "Locação renovada com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Você não tem permissão para esta ação";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao renovar locação: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
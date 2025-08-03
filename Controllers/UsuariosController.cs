using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;  

namespace Biblioteca.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IUsuarioRepository usuarioRepository,
            ILogger<UsuariosController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de usuários");
                TempData["ErrorMessage"] = "Erro ao carregar lista de usuários";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _usuarioRepository.AddAsync(usuario);
                    TempData["SuccessMessage"] = "Usuário criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Por favor, corrija os erros no formulário.";
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                TempData["ErrorMessage"] = $"Erro ao criar usuário: {ex.Message}";
                return View(usuario);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado";
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao carregar usuário para edição (ID: {id})");
                TempData["ErrorMessage"] = "Erro ao carregar dados do usuário";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            _logger.LogInformation($"Iniciando edição do usuário ID: {id}");

            if (id != usuario.Id)
            {
                TempData["ErrorMessage"] = "ID do usuário inválido";
                return RedirectToAction(nameof(Index));
            }

            // Validação manual para evitar problemas com a senha
            if (string.IsNullOrEmpty(usuario.Nome))
                ModelState.AddModelError("Nome", "O nome é obrigatório");
            
            if (string.IsNullOrEmpty(usuario.Email))
                ModelState.AddModelError("Email", "O e-mail é obrigatório");
            else if (!new EmailAddressAttribute().IsValid(usuario.Email))
                ModelState.AddModelError("Email", "E-mail inválido");
            
            if (string.IsNullOrEmpty(usuario.Telefone))
                ModelState.AddModelError("Telefone", "O telefone é obrigatório");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                
                _logger.LogWarning($"Erros de validação: {errors}");
                TempData["ErrorMessage"] = "Por favor, corrija os erros no formulário.";
                return View(usuario);
            }

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                
                _logger.LogInformation($"Usuário ID: {id} atualizado com sucesso");
                TempData["SuccessMessage"] = "Usuário atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _usuarioRepository.ExistsAsync(usuario.Id))
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado";
                    return RedirectToAction(nameof(Index));
                }
                
                _logger.LogError(ex, $"Erro de concorrência ao atualizar usuário ID: {id}");
                TempData["ErrorMessage"] = "Ocorreu um erro ao salvar. O registro foi modificado por outro usuário.";
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar usuário ID: {id}");
                TempData["ErrorMessage"] = $"Erro ao atualizar usuário: {ex.Message}";
                return View(usuario);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado";
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao carregar usuário para exclusão (ID: {id})");
                TempData["ErrorMessage"] = "Erro ao carregar dados do usuário";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _usuarioRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao excluir usuário ID: {id}");
                TempData["ErrorMessage"] = $"Erro ao excluir usuário: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biblioteca.Repositorio;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RelatorioDevolucoesController : Controller
    {
        private readonly ILocacaoRepository _locacaoRepo;

        public RelatorioDevolucoesController(ILocacaoRepository locacaoRepo)
        {
            _locacaoRepo = locacaoRepo;
        }

        public async Task<IActionResult> Index(DateTime? inicio, DateTime? fim)
        {
            var devolucoes = await _locacaoRepo.GetDevolucoesAsync(inicio, fim);
            ViewBag.DataInicio = inicio;
            ViewBag.DataFim = fim;
            return View(devolucoes);
        }

        [HttpPost]
        public async Task<IActionResult> CalcularMultas()
        {
            try
            {
                await _locacaoRepo.CalcularMultasAtrasadasAsync();
                TempData["SuccessMessage"] = "Multas recalculadas com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao calcular multas: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
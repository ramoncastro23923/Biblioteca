using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Repositorio;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;

namespace Biblioteca.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AccountController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Se o usuário já estiver autenticado, redireciona para Home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _usuarioRepository.Authenticate(model.Email, model.Senha);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciais inválidas");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.TipoPerfil.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    AllowRefresh = true
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Limpa o cache do navegador antes do logout
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Expires"] = "-1";

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("create-admin")]
        public IActionResult CreateAdmin()
        {
            try
            {
                var hasher = new PasswordHasher<Usuario>();
                var admin = new Usuario
                {
                    Nome = "Admin Principal",
                    Email = "admin@biblioteca.com",
                    Telefone = "11999999999",
                    Senha = "Admin123@", // Senha mais segura
                    TipoPerfil = TipoPerfil.Administrador
                };

                _usuarioRepository.Add(admin);
                return Ok("✅ Administrador criado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }
    }
}
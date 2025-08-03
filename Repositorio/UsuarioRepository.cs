using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Models;
using Biblioteca.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Repositorio
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BibliotecaContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioRepository(BibliotecaContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.AsNoTracking().ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<Usuario> AuthenticateAsync(string email, string senha)
        {
            var usuario = await GetByEmailAsync(email);
            if (usuario == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }

        public async Task AddAsync(Usuario usuario)
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(usuario.Id);
            if (usuarioExistente == null) return;

            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.Telefone = usuario.Telefone;
            usuarioExistente.TipoPerfil = usuario.TipoPerfil;

            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuarioExistente.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            }

            _context.Usuarios.Update(usuarioExistente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }

        // Implementações síncronas (opcionais)
        public IEnumerable<Usuario> GetAll() => _context.Usuarios.AsNoTracking().ToList();
        public Usuario GetById(int id) => _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
        public Usuario GetByEmail(string email) => _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());
        
        public void Add(Usuario usuario)
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Update(Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null) return;

            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.Telefone = usuario.Telefone;
            usuarioExistente.TipoPerfil = usuario.TipoPerfil;

            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuarioExistente.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            }

            _context.Usuarios.Update(usuarioExistente);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Usuarios.Any(u => u.Id == id);

        public Usuario Authenticate(string email, string senha)
        {
            var usuario = GetByEmail(email);
            if (usuario == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }
    }
}
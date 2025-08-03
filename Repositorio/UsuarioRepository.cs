using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Biblioteca.Models;
using Biblioteca.Data;
using System.Collections.Generic;
using System.Linq;

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

        // Métodos assíncronos (já existentes)
        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<Usuario?> GetByIdAsync(int id)
{
             return await _context.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);
}

        public async Task<Usuario> AuthenticateAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());

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
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            }
            _context.Usuarios.Update(usuario);
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

        // Novos métodos síncronos para implementar a interface
        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuarios.AsNoTracking().ToList();
        }

        public Usuario GetById(int id)
        {
            return _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
        }

        public Usuario GetByEmail(string email)
        {
            return _context.Usuarios.AsNoTracking()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public void Add(Usuario usuario)
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Update(Usuario usuario)
        {
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            }
            _context.Usuarios.Update(usuario);
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

        public bool Exists(int id)
        {
            return _context.Usuarios.Any(u => u.Id == id);
        }

        public Usuario Authenticate(string email, string senha)
        {
            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());

            if (usuario == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }
    }
}
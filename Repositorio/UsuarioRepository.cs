using System.Collections.Generic;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;
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

        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuarios.ToList();
        }

        public Usuario GetById(int id)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public Usuario GetByEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email);
        }

public void Add(Usuario usuario)
{
    Console.WriteLine($"Senha antes do hash: {usuario.Senha}");
    usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
    Console.WriteLine($"Senha após hash: {usuario.Senha}");
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
            var usuario = GetById(id);
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
    // Debug inicial
    Console.WriteLine($"Iniciando autenticação para: {email}");
    
    var usuario = _context.Usuarios
        .AsNoTracking()
        .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

    if (usuario == null)
    {
        Console.WriteLine("Usuário não encontrado no banco de dados");
        return null;
    }

    Console.WriteLine($"Usuário encontrado: {usuario.Nome}");
    Console.WriteLine($"Hash armazenado: {usuario.Senha}");

    try
    {
        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
        Console.WriteLine($"Resultado da verificação: {result}");
        
        return result == PasswordVerificationResult.Success ? usuario : null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na verificação: {ex.Message}");
        return null;
    }
}
    }
}
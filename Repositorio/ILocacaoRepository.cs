using System.Collections.Generic;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositorio
{
    public interface ILocacaoRepository
    {
        IEnumerable<Locacao> GetAll();
        Locacao GetById(int id);
        IEnumerable<Locacao> GetByUsuario(int usuarioId);
        IEnumerable<Locacao> GetPendentes();
        void Add(Locacao locacao);
        void Update(Locacao locacao);
        void Devolver(int locacaoId);
        void Renovar(int locacaoId);
        bool LivroDisponivel(int livroId);
        bool UsuarioPodeLocar(int usuarioId);
    }
}
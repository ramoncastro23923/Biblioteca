using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public enum StatusLocacao
    {
        Pendente,
        Devolvido,
        Atrasado
    }

    public class Locacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Livro")]
        public int LivroId { get; set; }
        public Livro Livro { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataRetirada { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataDevolucaoPrevista { get; set; } = DateTime.Now.AddDays(14);

        [DataType(DataType.Date)]
        public DateTime? DataDevolucaoReal { get; set; }

        public StatusLocacao Status { get; set; } = StatusLocacao.Pendente;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Multa { get; set; } = 0;

        public bool PodeRenovar { get; set; } = true;
    }
}
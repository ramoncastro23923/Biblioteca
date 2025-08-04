using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Livro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "O autor é obrigatório")]
        [StringLength(100, ErrorMessage = "O autor deve ter no máximo 100 caracteres")]
        public string? Autor { get; set; }

        [Required(ErrorMessage = "A editora é obrigatória")]
        [StringLength(100, ErrorMessage = "A editora deve ter no máximo 100 caracteres")]
        public string? Editora { get; set; }

        [Required(ErrorMessage = "O ano de publicação é obrigatório")]
        [Range(1000, 9999, ErrorMessage = "Ano de publicação inválido")]
        public int AnoPublicacao { get; set; }

        [Required(ErrorMessage = "O ISBN é obrigatório")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN deve ter 10 ou 13 caracteres")]
        [RegularExpression(@"^(?:\d{9}[\dXx]|\d{13})$", ErrorMessage = "ISBN inválido")]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "A quantidade disponível é obrigatória")]
        [Range(0, 1000, ErrorMessage = "Quantidade deve ser entre 0 e 1000")]
        public int QuantidadeDisponivel { get; set; }
    }
}
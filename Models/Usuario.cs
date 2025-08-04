using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public enum TipoPerfil
    {
        Administrador,
        UsuarioPadrao
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string Telefone { get; set; }

        [DataType(DataType.Password)]
          [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
          public string? Senha { get; set; }

        [Required]
        public TipoPerfil TipoPerfil { get; set; }
    }
}
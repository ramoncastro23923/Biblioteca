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
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "O e-mail é obrigatório")]
            [EmailAddress(ErrorMessage = "E-mail inválido")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "O telefone é obrigatório")]
            [Phone(ErrorMessage = "Telefone inválido")]
            public string Telefone { get; set; } = string.Empty;

            [Required(ErrorMessage = "A senha é obrigatória")]
            [DataType(DataType.Password)]
            public string Senha { get; set; } = string.Empty;

            [Required]
            public TipoPerfil TipoPerfil { get; set; }
        }
}
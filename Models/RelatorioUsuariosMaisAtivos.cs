using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class RelatorioUsuariosMaisAtivos
    {
        public int UsuarioId { get; set; }
        
        [Display(Name = "Nome do Usuário")]
        public string Nome { get; set; }
        
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        
        [Display(Name = "Total de Locações")]
        public int TotalLocacoes { get; set; }
        
        [Display(Name = "Percentual")]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal Percentual { get; set; }
    }
}
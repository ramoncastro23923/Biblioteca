// Em Models/RelatorioLivrosMaisLocados.cs
namespace Biblioteca.Models
{
    public class RelatorioLivrosMaisLocados
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }  // Adicione esta linha
        public int TotalLocacoes { get; set; }
        public decimal Percentual { get; set; }
    }
}
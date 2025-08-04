namespace Biblioteca.Models
{
    public class RelatorioLivrosMaisLocados
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }   
        public int TotalLocacoes { get; set; }
        public decimal Percentual { get; set; }
    }
}
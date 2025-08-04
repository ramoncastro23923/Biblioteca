using System.Collections.Generic;

namespace Biblioteca.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<StatusLocacaoChart> StatusLocacoes { get; set; }
        public IEnumerable<LocacoesPorMes> LocacoesPorMes { get; set; }
        public IEnumerable<RelatorioLivrosMaisLocados> TopLivros { get; set; }
        public IEnumerable<RelatorioUsuariosMaisAtivos> TopUsuarios { get; set; }
    }

    public class StatusLocacaoChart
    {
        public string Status { get; set; }
        public int Quantidade { get; set; }
        public string Cor { get; set; }
    }

    public class LocacoesPorMes
    {
        public string MesAno { get; set; }
        public int Quantidade { get; set; }
    }
}
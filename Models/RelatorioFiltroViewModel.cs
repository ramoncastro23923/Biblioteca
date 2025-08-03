using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class RelatorioFiltroViewModel
    {
        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }
        
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }
    }
}
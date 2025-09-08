using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Lote
    {
        [Key]
        public int codLote { get; set; }
        [Required(ErrorMessage = "Você deve preencher o campo preço!")]
        [Display(Name = "Preço")]
        public double valorLote { get; set; }
        public int numLote { get; set; }
        [Required(ErrorMessage = "Você deve definir a quantidade de ingressos!")]
        [Display(Name = "Quantidade")]
        public int qtdIngLote { get; set; }
        [Required(ErrorMessage = "Você deve preencher a data de venda do lote!")]
        [Display(Name = "Data")]
        public DateTime dataVendaLote { get; set; }
        public int codEvento { get; set; }
        public virtual Evento? Evento { get; set; }
        public ICollection<Ingresso>? Ingressos { get; set; }
    }
    public class LoteDisponivelInfo
    {
        public int LoteAtual { get; set; }
        public int IngressosDisponiveisAtual { get; set; }
        public Lote? ProximoLote { get; set; }
        public DateTime? DataProximoLote { get; set; }
        public int? QuantidadeTotalProximoLote { get; set; } // Nova propriedade
    }
}

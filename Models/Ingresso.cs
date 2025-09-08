using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Ingresso
    {
        [Key]
        public int codIngresso { get; set; }
        [Required]
        public string? ticketIngresso { get; set; }
        [StringLength(150, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public string? NomeIngresso { get; set; }
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string? EmailIngresso { get; set; }
        [StringLength(15)]
        public string? cpfIngresso { get; set; }
        public string? tipoVendaIngresso { get; set; }
        public bool ativoIngresso { get; set; }
        [ForeignKey("Lote")]
        public int codLote { get; set; }
        public virtual Lote? Lote { get; set; }
        public int? codConvite { get; set; }
        public Convite? Convite { get; set; }
        public int? codReciboIngresso { get; set; }
        public ReciboIngresso? ReciboIngresso { get; set; }
    }
}

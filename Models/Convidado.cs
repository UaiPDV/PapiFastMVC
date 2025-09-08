using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Convidado
    {
        [Key]
        public int codConvidado { get; set; }
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string? emailConvidado { get; set; }
        public string? nomeConvidado { get; set; }
        [StringLength(18, ErrorMessage = "O {0} deve ter ser preenchido corretamente.", MinimumLength = 11)]
        [Display(Name = "Telefone")]
        public string? telefoneConvidado { get; set; }
        public bool? vistoConvite { get; set; }
        public string? confirmacaoConvite { get; set; } //valores= "Confirmado", "Recusado", "Aguardando"
        public int codIngresso { get; set; }
        public Ingresso? Ingresso { get; set; }
        public int codConvite { get; set; }
        public Convite? Convite { get; set; }
    }
}

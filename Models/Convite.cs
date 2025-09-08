using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Convite
    {
        [Key]
        public int codConvite {  get; set; }
        [StringLength(30000, ErrorMessage = "Voçê precisa escrever um convite com no minímo 10 caracteres", MinimumLength = 11)]
        [Display(Name = "Mensagem")]
        public string? msgConvite { get; set; }
        public bool ativo { get; set; }
        public string? facebook { get; set; }
        public string? instagram { get; set; }
        public string? imagemConvite { get; set; }
        public int? codEvento { get; set; }
        public DateTime dataCriacao { get; set; }
        public Evento? Evento { get; set; }
        public ICollection<Convidado>? Convidados { get; set; }
        public ICollection<Ingresso>? Ingressos { get; set; }
        public int? codCriador { get; set; }
        public int? codFilial { get; set; }
        public UsuarioFilial? UsuarioFilial { get; set; }
        public int codCliente { get; set; }
        public int notificar { get; set; } // 0 = não notificar, 1 = notificar email, 2 = notificar whatsapp, 3 = notificar ambos
        public Usuario? Usuario { get; set; }
        public ListaPresente? ListaPresente { get; set; }
        public ICollection<ReciboPresente>? recibosPresentes{ get; set; }
    }
}

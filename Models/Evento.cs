using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Evento
    {
        [Key]
        public int codEvento { get; set; }
        public bool eventoGratuito { get; set; }
        [Required(ErrorMessage = "Você deve deve definir uma quantidade de lotes ou definir como gratuito!")]
        public int qtdlotesEvento { get; set; }
        [StringLength(150, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Required(ErrorMessage = "Você deve deve definir um nome para o evento!")]
        [Display(Name = "Nome")]
        public string? nomeEvento { get; set; }
        [Required(ErrorMessage = "Você deve definir uma categoria!")]
        public string? categoriaEvento { get; set; }
        [Required(ErrorMessage = "Você deve definir uma categoria!")]
        public string? tipoEvento { get; set; }
        [StringLength(150, ErrorMessage = "O campo link deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Display(Name = "Link")]
        public string? linkEvento { get; set; }
        [Required(ErrorMessage = "Você deve definir uma data e hora!")]
        public DateTime dataInicioEvento { get; set; }
        [Required(ErrorMessage = "Você deve definir uma data e hora!")]
        public DateTime dataTerminoEvento { get; set; }
        public DateTime dataCriacaoEvento { get; set; }
        public int qtdTotalIngEvento { get; set; }
        [StringLength(5000, ErrorMessage = "O campo descrição deve ter pelo menos 4 caracteres e não mais que 5000.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public string? descricaoEvento { get; set; }
        public bool statusEvento { get; set; }
        public string? bannerEvento { get; set; }
        public string? thumbnailEvento { get; set; }
        public int? codUsuario { get; set; }
        public int? codFilial { get; set; }
        public string? linkCardapio { get; set; }
        public int tipoCardapio { get; set; }
        public bool eventoPrivado { get; set; }
        public int classificacaoEtaria { get; set; }
        public UsuarioFilial? UsuarioFilial { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Lote>? Lotes { get; set; }
        public int codEndereco { get; set; }
        public Endereco? Endereco { get; set; }
        public ICollection<Convite>? Convites { get; set; }
        public ICollection<ProdutoCampanha>? ProdutosEvento { get; set; }
    }
}

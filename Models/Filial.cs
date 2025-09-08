using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Filial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int codFilial { get; set; }
        [StringLength(150, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public required string nome { get; set; }
        [StringLength(18, ErrorMessage = "O {0} deve ser preenchido corretamente.", MinimumLength = 14)]
        [Display(Name = "CNPJ")]
        public required string cnpj { get; set; }
        [StringLength(14, ErrorMessage = "O {0} deve ter ser preenchido corretamente.", MinimumLength = 11)]
        [Display(Name = "Telefone")]
        public string? telefone { get; set; }
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string? email { get; set; }
        [MaxLength(200)]
        public string? logoHome { get; set; }
        public string? logoPDV { get; set; }
        public ICollection<UsuarioFilial>? Usuarios { get; set; }
        public int codEndereco { get; set; }
        public bool ativo { get; set; }
        public Endereco? Endereco { get; set; }
        public WhatsApp? WhatsApp { get; set; }
        public ICollection<Evento>? Eventos { get; set; }
        public ICollection<Preco>? precos { get; set; }
        public ICollection<Dispositivo>? Dispositivos { get; set; }
        }

    public class FilialUai
    {
        public int codigo { get; set; }
        public string? nome { get; set; }
        public string? cnpj { get; set; }
        public string? telefone { get; set; }
        public string? email { get; set; }
        public string? ender { get; set; }
        public required string numeroendereco { get; set; }
        public string? bairro { get; set; }
        public string? estado { get; set; }
        public string? cidade { get; set; }
        public int codigomunicipio { get; set; }
    }

    public class ImagemPDV
    {
        public string? logoHome { get; set; }
        public string? logoPDV { get; set; }
        public string? logoTablet { get; set; }
    }
}
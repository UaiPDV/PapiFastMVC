using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class ReciboPresente
    {
        [Key]
        public int codReciboPresente { get; set; }
        [StringLength(100, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public required string nomeCliente { get; set; }
        [StringLength(15, ErrorMessage = "O campo nome deve ser preenchido corretamente!", MinimumLength = 12)]
        [Display(Name = "CPF")]
        public required string cpfCliente { get; set; }
        public DateTime dataCriacao { get; set; } = DateTime.Now;
        public required string status { get; set; }
        [StringLength(150)]
        [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de e-mail válido.")]
        public required string emailCliente { get; set; }
        public string? telefoneCliente { get; set; }
        public int? codUsuario { get; set; }
        public Usuario? usuario { get; set; }
        public int codConvite { get; set; }
        public Convite? convite{ get; set; }
        public ICollection<Presente>? Presentes { get; set; }
    }
}

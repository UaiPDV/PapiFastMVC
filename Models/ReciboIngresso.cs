using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class ReciboIngresso
    {
        [Key]
        public int codReciboIngresso { get; set; }
        [StringLength(100, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Required(ErrorMessage = "O campo de nome é obrigatório.")]
        [Display(Name = "Nome")]
        public required string nomeCliente { get; set; }
        [Required(ErrorMessage = "O campo de CPF é obrigatório.")]
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
        public ICollection<Ingresso>? ingressos { get; set; }
    }
}

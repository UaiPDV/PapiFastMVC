using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BixWeb.Models
{
    public class Usuario
    {
        [Key]
        public int codUsuario { get; set; }
        [StringLength(150)]
        [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de e-mail válido.")]
        public required string email { get; set; }
        [StringLength(150, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        [Required(ErrorMessage = "O campo de nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string? nome { get; set; }
        [StringLength(18, ErrorMessage = "O {0} deve ter ser preenchido corretamente.", MinimumLength = 14)]
        [Display(Name = "Telefone")]
        public string? telefone { get; set; }
        [Required(ErrorMessage = "O campo de CPF é obrigatório.")]
        [StringLength(18, ErrorMessage = "O {0} deve ter ser preenchido corretamente.", MinimumLength = 14)]
        [Display(Name = "CPF")]
        public string? cpf { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Você deve definir uma senha!")]
        public required string password { get; set; }
        [StringLength(200)]
        public string? imagem { get; set; }
        public string? token { get; set; }
        public bool? ativo { get; set; }
        public LoginAPI? loginAPI { get; set; }
        public ICollection<UsuarioFilial>? filiais { get; set; }
        public ICollection<Evento>? eventos { get; set; }
        public ICollection<Voucher>? Vouchers { get; set; }
        public ICollection<Cupom>? Cupons { get; set; }
        public ICollection<Convite>? Convites { get; set; }
        public WhatsApp? WhatsApp { get; set; }
        public ICollection<ListaPresente>? listaPresentes { get; set; }
        public ICollection<ReciboPresente>? recibosPresentes { get; set; }
 
    }
}

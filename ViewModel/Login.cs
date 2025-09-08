using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.ViewModel
{
    public class Login
    {
        [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de e-mail válido.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Você deve digitar uma senha!")]
        public string? Senha { get; set; }
    }
}

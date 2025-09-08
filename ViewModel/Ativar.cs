using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.ViewModel
{
    public class Ativar
    {
        public int codFilial { get; set; }
        public int codUsuario { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Você deve digitar uma senha!")]
        public required string senha { get; set; }
        [Required(ErrorMessage = "Você deve preencher este campo!")]
        [DataType(DataType.Password)]
        public required string confirmarSenha { get; set; }
    }
}

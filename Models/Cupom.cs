using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Cupom
    {
        [Key]
        public int codCupom { get; set; }
        public int codCriador { get; set; }
        [Required(ErrorMessage = "Você deve deve selecionar uma filial!")]
        public int codFilial { get; set; }
        public virtual UsuarioFilial? UsuarioFilial{ get; set; }
        [Display(Name = "Desconto")]
        [Required(ErrorMessage = "Você deve definir o valor do desconto!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ter 2 casas decimais somente!")]
        public double descontoCupom { get; set; }
        [Display(Name = "Validade")]
        [Required(ErrorMessage = "Você deve definir a data de experiração do cupom!")]
        public DateTime validadeCupom { get; set; }
        public bool statusCupom { get; set; }
        public bool usadoCupom { get; set; }
        [Display(Name = "Token")]
        [Required(ErrorMessage = "Você deve deve Definir um token!")]
        [StringLength(20, ErrorMessage = "O campo código deve ter pelo menos 4 caracteres.", MinimumLength = 4)]
        public required string tokenCupom { get; set; }
        public int? codCampanha { get; set; }
        public Campanha? Campanha { get; set; }
        public string? produtos { get; set; }
        public int? codcliente { get; set; }
        public Usuario? usuario { get; set; }
    }
}

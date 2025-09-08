using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Campanha
    {
        [Key]
        public int codCampanha { get; set; }
        [Required(ErrorMessage = "Você deve preencher o campo nome!")]
        [StringLength(100, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres e no máximo 100.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public string? nomeCampanha { get; set; }
        public DateTime dataPostCampanha { get; set; }
        public bool campanhaAtiva { get; set; }
        public string? matriculaFuncionario { get; set; }
        public string? linkCardapio { get; set; }
        public int tipoCardapio { get; set; }
        public string? typeServise { get; set; }
        public int codUsuario { get; set; }
        [Required(ErrorMessage = "Você deve selecionar uma filial!")]
        [Display(Name = "Filial")]
        public int codFilial { get; set; }
        public virtual UsuarioFilial? Usuariofilial { get; set; }
        public ICollection<ProdutoCampanha>? produtosCampanha { get; set; }
        public ICollection<Voucher>? Vouchers { get; set; }
        public ICollection<Cupom>? Cupons { get; set; }
    }
}

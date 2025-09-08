using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BixWeb.Models
{
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int codProduto { get; set; }
        [StringLength(100, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres e no máximo 100.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public string? nomeProduto { get; set; }
        [StringLength(400, ErrorMessage = "O campo descrição deve ter no máximo 400 caracteres.")]
        [Display(Name = "Descrição")]
        public string? descricaoDetalhada { get; set; }
        public int codCategoria { get; set; }
        public virtual Categoria? Categoria { get; set; }
        [StringLength(200)]
        public string? imagem { get; set; }
        public bool? chamaModificadores { get; set; }
        public ICollection<Preco>? Precos { get; set; }
        public ICollection<Modificador>? Modificadores { get; set; }
        public ICollection<ProdutoCampanha>? ProdutosCampanha { get; set; }
    }

    public class ProdutoUai
    {
        public int codigo { get; set; }
        public string? descricaoCupom { get; set; }
        public required string descricaoDetalhada { get; set; }
        public int codGrupo { get; set; }
        public string? imagem { get; set; }
        public bool? chamaModificadores { get; set; }
    }
}

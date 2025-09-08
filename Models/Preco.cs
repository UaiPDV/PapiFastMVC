using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Preco
    {
        [Key]
        public int codPreco { get; set; }
        [Required(ErrorMessage = "Você deve preencher o campo preço!")]
        public double valor { get; set; }
        [ForeignKey("Filial")]
        public int codFilial { get; set; }
        public virtual Filial? Filial { get; set; }
        [ForeignKey("Produto")]
        public int codProduto { get; set; }
        public virtual Produto? Produto { get; set; }
    }
}

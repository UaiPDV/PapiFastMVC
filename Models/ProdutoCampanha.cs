using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BixWeb.Models
{
    public class ProdutoCampanha
    {
        [Key]
        public int codProdCamp { get; set; }
        [ForeignKey("Produto")]
        public int codProduto { get; set; }
        public virtual Produto? Produto { get; set; }
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [RegularExpression(@"^\d+([\,]\d{1,2})?$", ErrorMessage = "O valor deve ter até duas casas decimais.")]
        public double valor { get; set; }
        [ForeignKey("Campanha")]
        public int? codCampanha { get; set; }
        public Campanha? Campanha { get; set; }
        public int? codEvento { get; set; }
        public Evento? Evento { get; set; }
    }
}

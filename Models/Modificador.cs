using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Modificador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int codigo { get; set; }
        public string? classificacaoGrupo { get; set; }
        public int ordemGrupo { get; set; }
        public string? descricaoGrupo { get; set; }
        public int ordemModificador { get; set; }
        public int codEmpresa { get; set; }
        public int codmodificador { get; set; }
        public string? classificacao { get; set; }
        public double preco { get; set; }
        public bool ativo { get; set; }
        public required string descricaoModificador { get; set; }
        public int qtdMinima { get; set; }
        public int qtdMaxima { get; set; }
        [ForeignKey("Produto")]
        public int codproduto { get; set; }
        public virtual Produto? Produto { get; set; }
        public int codGrupo { get; set; }
        [ForeignKey("Filial")]
        public int codfilial { get; set; }
        public virtual Filial? Filial { get; set; }
    }
}

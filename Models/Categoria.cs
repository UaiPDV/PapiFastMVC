using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Categoria
    {
        [Key]
        public int codCategoria { get; set; }
        [StringLength(100, ErrorMessage = "O campo nome deve ter pelo menos 4 caracteres e no máximo 100.", MinimumLength = 4)]
        [Display(Name = "Nome")]
        public required string nome { get; set; }
        public string? corIcone { get; set; }
        public int codRefExterna { get; set; }
        public bool ativo { get; set; }
        public ICollection<Produto>? Produtos { get; set; }
    }
}

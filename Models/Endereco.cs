using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BixWeb.Models
{
    public class Endereco
    {
        [Key]
        public int codEndereco { get; set; }
        [StringLength(150, ErrorMessage = "O campo logradouro deve ser preenchido corretamente.", MinimumLength = 4)]
        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "O campo Rua/Avenida é obrigatório.")]
        public string? logradouro { get; set; }
        [Required(ErrorMessage = "O campo numeral é obrigatório.")]
        public int numeroendereco { get; set; }
        [StringLength(150, ErrorMessage = "O campo bairro deve ser preenchido corretamente.", MinimumLength = 4)]
        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        public string? bairro { get; set; }
        [StringLength(150, ErrorMessage = "O campo estado deve ser preenchido corretamente.", MinimumLength = 2)]
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        public string? estado { get; set; }
        [StringLength(150, ErrorMessage = "O campo cidade deve ser preenchido corretamente.", MinimumLength = 4)]
        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O Cidade CEP é obrigatório.")]
        public string? cidade { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [StringLength(9, ErrorMessage = "O campo CEP deve ter 8 caracteres.", MinimumLength = 8)]
        public string? cep { get; set; }
        public string? refenciaEndereco { get; set; }
        public ICollection<Evento>? Eventos { get; set; }
        public Filial? Filial { get; set; }
    }
}

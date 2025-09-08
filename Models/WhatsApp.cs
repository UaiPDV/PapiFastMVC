using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BixWeb.Models
{
    public class WhatsApp
    {
        [Key]
        public int codWhats {  get; set; }
        public string? sessao { get; set; }
        public required string status { get; set; }
        public required string ntelefone { get; set; }
        public string? qrCode { get; set; }
        public DateTime criado { get; set; }
        public DateTime atualizado { get; set; }
        [ForeignKey("Filial")]
        public int? codFilial { get; set; }
        public virtual Filial? Filial { get; set; }
        [ForeignKey("Usuario")]
        public int? codUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}

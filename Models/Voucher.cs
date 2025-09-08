using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Voucher
    {
        [Key]
        public int codVoucher { get; set; }
        public string? tokenVoucher { get; set; }
        [Required(ErrorMessage = "Você deve definir um valor para o voucher!")]
        public decimal valorOriginal { get; set; } 
        public decimal valorDisponivel { get; set; }
        public DateTime validadeVoucher { get; set; }
        public DateTime criacaoVoucher { get; set; }
        public bool utilizado { get; set; }
        public bool ativo { get; set; }
        public bool exlusivo { get; set; } //se o voucher é exclusivo para um cliente ou não
        public int? codCampanha { get; set; }
        public Campanha? Campanha { get; set; }
        public int codCriador { get; set; }
        public int codFilial { get; set; }//filial a qual pertence 
        public UsuarioFilial? UsuarioFilial { get; set; }
        public int? codCliente { get; set; }//cliente que comprou
        public Usuario? Cliente { get; set; }
    }
}

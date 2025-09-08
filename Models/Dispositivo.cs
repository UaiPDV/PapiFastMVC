using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class Dispositivo
    {
        [Key]
        public int codDipositivo { get; set; }
        public required string enderecoMac { get; set; }
        public bool ativo {  get; set; }
        public string? dispositivo { get; set; }
        public string? tipoDisposito { get; set; }
        public int codFilial { get; set; }
        public Filial? Filial { get; set; }
        public ICollection<RegistroLogin>? RegistroLogins { get; set; }
    }
}

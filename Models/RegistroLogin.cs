using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class RegistroLogin
    {
        [Key]
        public int codRegistro {  get; set; }
        public DateTime inicioLogin { get; set; }
        public DateTime? fimLogin { get; set; }
        public bool ativo { get; set; }
        public int codDispositivo { get; set; }
        public Dispositivo? Dispositivo { get; set; }
        public UsuarioFilial? UsuarioFilial { get; set; }
    }
}

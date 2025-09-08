namespace BixWeb.Models
{
    public class UsuarioFilial
    {
        public int codUsuario {  get; set; }
        public Usuario? Usuario { get; set; }
        public int codFilial { get; set; }
        public Filial? Filial { get; set; }
        public int pin { get; set; }
        public bool ativo { get; set; }
        public string? cargoUsuario { get; set; }
        public string? tipoUsuario { get; set; }
        public string? token { get;set ; } //matricula ou token de cliente
        public DateTime dataCadastro { get; set; }
        public int? codRegistro { get; set; }
        public RegistroLogin? RegistroLogin { get; set; }
        public ICollection<Evento>? Eventos { get; set; }
        public ICollection<Campanha>? Campanhas { get; set; }
        public ICollection<Voucher>? Vouchers { get; set; }   
        public ICollection<Cupom>? Cupons { get; set; }  
        public ICollection<Convite>? Convites { get; set; }
    }
}

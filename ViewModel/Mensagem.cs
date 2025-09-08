namespace BixWeb.ViewModel
{
    public class Mensagem
    {
        public string? token { get; set; }
        public string? numero { get; set; }
        public string? urlImagem { get; set; }
        public string? textoConvite { get; set; }
        public string? textoConfirmacao { get; set; }
    }
    public class MensagemWhats
    {
        public string? token { get; set; }
        public string? number { get; set; }
        public string? type { get; set; }
        public string? link { get; set; }
        public string? message { get; set; }
        public string? fileName { get; set; }
    }
    public class MensagemWhatsResponse
    {
        public bool success { get; set; }
        public string? message { get; set; }
    }
}

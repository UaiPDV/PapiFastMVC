namespace BixWeb.Models
{
    public class ListaPresenteRecibo
    {
        public int codPresente { get; set; }
        public Presente? presente { get; set; }
        public int codReciboPresente { get; set; }
        public ReciboPresente? reciboPresente { get; set; }
    }
}

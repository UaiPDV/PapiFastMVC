using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BixWeb.Models
{
    public class Presente
    {
        [Key]
        public int codPresente { get; set; }
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("preco")]
        public double Preco { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }

        [JsonPropertyName("imagem")]
        public string? Imagem { get; set; }
        public int codListaPresente {get;set;}
        public ListaPresente? listaPresente { get; set; }
        public ICollection<ReciboPresente>? listaPresenteRecibos { get; set; }
    }
}

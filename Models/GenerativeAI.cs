using System.ComponentModel.DataAnnotations;
namespace BixWeb.Models
{
    public class GenerativeIA
    {
        [Key]
        public int codIA { get; set; }
        public string? tipoIA { get; set; }
        public string? chaveIA { get; set; }
    }
}

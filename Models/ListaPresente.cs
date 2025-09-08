using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class ListaPresente
    {
        [Key]
        public int codListaPres { get; set; }
        public int codConvite { get; set; }
        public Convite? convite { get; set; }
        public int codUsuario { get; set; }
        public Usuario? usuario { get; set; }
        public DateTime dataLista { get; set; }
        public ICollection<Presente>? presentes{get; set;}
    }
}

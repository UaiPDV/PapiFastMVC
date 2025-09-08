using System.ComponentModel.DataAnnotations;

namespace BixWeb.Models
{
    public class LoginAPI
    {
        [Key]
        public int codLoginAPI { get; set; }
        public required string client_id { get; set; }
        public required string username { get; set; }
        public required string password { get; set; }
        public required string client_secret { get; set; }
        public int codUsuario { get; set; }
        public Usuario? usuario { get; set; }
    }
}

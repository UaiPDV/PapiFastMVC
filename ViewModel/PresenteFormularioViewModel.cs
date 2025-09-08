using X.PagedList;

namespace BixWeb.ViewModel
{
    public class PresenteFormularioViewModel
    {
        public IPagedList<Models.Presente>? Presentes { get; set; }

        // Campos do formulário
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public int codConvite { get; set; }
        public string? CPF { get; set; }
        public List<int> CodPresenteSelecionados { get; set; } = new();
        public string? MensagemErro { get; set; }
    }
}

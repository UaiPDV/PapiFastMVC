using BixWeb.Models;
using BixWeb.ViewModel;

namespace BixWeb.Services
{
    public interface IPageGeneratorService1
    {
        public Task<List<ViewModel.Presente>> ObterRecomendacoes(string pedido);
        public Task<string> GerarImagens(string prompt);
        public Task<string> ObterPromptImagem(string produto, string descricao, string valor);
        bool gerarCuponPDF(int id, string caminho, string link);
        bool EnviarCupom(string caminho, string email, string nome);
    }
}

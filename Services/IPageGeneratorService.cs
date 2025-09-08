using BixWeb.Models;
using BixWeb.ViewModel;

namespace BixWeb.Services
{
    public interface IPageGeneratorService
    {
        bool GenerateHtml1(string diretorio, int id, int tipo);
        bool GenerateHtml2(string diretorio, int id, int tipo);
		bool GenerateHtml3(string diretorio, Campanha campanha);
		bool GenerateHtml4(string diretorio, Campanha campanha);
		bool GenerateHtml5(string diretorio, Campanha campanha);
		bool GenerateHtml6(string diretorio, int id, int tipo);
        public bool AtivarContaCliente(Usuario usuario, UsuarioFilial usuarioFilial);
        public bool ComprovanteConvites(Usuario usuario, string link, Evento evento);
        public bool GerarCupon(int id, string caminho, string link);
		public bool GerarVoucher(int id, string caminho, string link);
		public bool GerarIngresso(int id, string caminho, string link);
        public bool EnviarReciboPresente(string caminho, ReciboPresente recibo);
        public bool EnviarCupom(string caminho, string email, string nome);
        public Task<bool> EnviarCupomWhats(string caminho, string telefone, string nome, string nomeFilial, string token, string produtos,string validade);
        public bool EnviarVoucher(string caminho, string email, string nome);
        public Task<bool> EnviarVoucherWhats(string caminho, string telefone, string nome, string nomeFilial, string token);
        public bool EnviarConvite(Convidado convidado, string link);
        public Task<bool> EnviarConviteWhats(Mensagem mensagem);
        public bool EnviarIngresso(string caminho, string email, string? nome);
        public Task<bool> EnviarIngressoWhats(MensagemWhats mensagem);
        public bool EnviarToken(string email, string nome, string token);
        public bool EnviarNotPresente(string email, string nome, ReciboPresente recibopresente);
        public Task<bool> EnviarNotPresenteWhats(string token, string nome, string telefone, ReciboPresente recibopresente);
        public bool EnviarNotPresenca(Convidado convidado);
        public Task<bool> EnviarNotPresencaWhats(Convidado convidado);
        public void copyarquivo(string sourceDirName, string destDirName);
        public void Layout1(Anuncio anuncio, ProdutoCampanha produto);
        public void Layout2(Anuncio anuncio,int fontAce, ProdutoCampanha produto, string link);
        public void Layout3(Anuncio anuncio, ProdutoCampanha produto);
        public void Layout4(Anuncio anuncio, ProdutoCampanha produto);
        public string GerarPDF(Campanha campanha, List<ProdutoCampanha> produto, List<IFormFile> anuncios, int tipo, string diretorio);
        public string GerarIMG(Campanha campanha, List<ProdutoCampanha> produto, List<IFormFile> anuncios, int tipo, string diretorio);
    }
}

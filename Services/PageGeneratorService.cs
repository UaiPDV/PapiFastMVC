using BixWeb.Models;
using BixWeb.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;
using ZXing;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO.Compression;
using System.Net.Mail;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Text.Json;
using System.Net.Sockets;
using static iTextSharp.text.pdf.AcroFields;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Google.Apis.CustomSearchAPI.v1.Data;
using iText.StyledXmlParser.Jsoup.Parser;
using BixWeb.Migrations;

namespace BixWeb.Services
{
    public class PageGeneratorService : IPageGeneratorService
    {
        private readonly DbPrint _context;
        private readonly HttpClient _httpClient;
        private readonly string _email;
        private readonly string _password;
        private readonly string _smtp;

        // Injeção de dependência do DbContext
        public PageGeneratorService(DbPrint context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context; _httpClient = httpClient;
            _email = configuration["EmailSettings:Email"];
            _password = configuration["EmailSettings:Password"];
            _smtp = configuration["EmailSettings:SMTPClient"];
        }
        public bool GenerateHtml1(string targetPath, int id, int tipo)
        {
            List<ProdutoCampanha> produtosCampanha= new List<ProdutoCampanha>();
            int codFilial = 0;
            if (tipo==1)
            {
                var campanha = _context.Campanhas.Find(id);
                if (campanha!=null)
                {
                    codFilial = campanha.codFilial;
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();

            }
            else
            {
                var evento = _context.Eventos.Find(id);
                if (evento != null)
                {
                    if (evento.codFilial != null)
                    {

                        codFilial = (int)evento.codFilial;
                    }
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codEvento == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();
                
            }
            var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
            var filial = _context.Filiais.Find(codFilial);

            using (StreamWriter sw = new StreamWriter(targetPath + "/Index.html"))
            {
                sw.WriteLine("<!DOCTYPE html>\r\n" +
                    "<html style=\"font-size: 16px;\" lang=\"pt-BR\">\r\n" +
                    "<head>\r\n" +
                    "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <meta charset=\"utf-8\">\r\n" +
                    "    <meta name=\"keywords\" content=\"Um novo conceito de lanches\">\r\n" +
                    "    <meta name=\"description\" content=\"PrintPDV é Show para Ponto de Venda\">\r\n" +
                    "    <meta name=\"page_type\" content=\"np-template-header-footer-from-plugin\">\r\n" +
                    "    <title>Seja Bem Vindo</title>\r\n    <link rel=\"stylesheet\" href=\"nicepage.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"Seja-Bem-Vindo.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\">" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>\r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/mark.js/8.11.1/jquery.mark.min.js\"></script>" +
                    "    <script class=\"u-script\" type=\"text/javascript\" src=\"nicepage.js\" defer=\"\"></script>\r\n" +
                    "    <meta name=\"generator\" content=\"PrintPDV - Unidade de Atendimento Integrada\">\r\n" +
                    "    <meta property=\"og:title\" content=\"Acesse e Veja as opções no cardápio\">\r\n" +
                    "    <meta property=\"og:image\" content=\"https://PrintPDV.com.br/mangalarga/imagens/Logo7.jpg\">\r\n" +
                    "    <meta property=\"og:url\" content=\"https://PrintPDV.com.br/mangalarga/\">\r\n" +
                    "    <link rel=\"canonical\" href=\"Para todos os momentos.\">\r\n" +
                    "    <link rel=\"icon\" href=\"imagens/logo.png\">\r\n" +
                    "    <link id=\"u-theme-google-font\" rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Roboto+Slab:100,200,300,400,500,600,700,800,900|Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i\">\r\n" +
                    "    <script type=\"application/ld+json\">{\r\n\t\t\"@context\": \"http://schema.org\",\r\n\t\t\"@type\": \"Organization\",\r\n\t\t\"name\": \"Cardápio \",\r\n\t\t\"url\": \"Para todos os momentos.\"}\r\n    </script>\r\n" +
                    "    <meta name=\"theme-color\" content=\"#0052ff\">\r\n" +
                    "    <meta property=\"og:description\" content=\"PrintPDV é Show para Ponto de Venda\">\r\n" +
                    "    <meta property=\"og:type\" content=\"website\">\r\n" +
                    "</head>");
                sw.WriteLine("<body class=\"u-body u-xl-mode content\">\r\n" +
                    "    <header class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\">\r\n" +
                    "        <section class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\" style=\"background-image: url(" + filial.logoHome + ") !Important;\"></section>" +
                    "    </header>\r\n" +
                    "    <div class=\"form header\">\r\n" +
                    "        <input type=\"search\" name=\"SearchString\" class=\"form-control form-input\" placeholder=\"Buscar Produto\">\r\n" +
                    "        <i class=\"fa fa-search\" ></i>\r\n" +
                    "        <div class=\"butonss\">\r\n" +
                    "            <button data-search=\"next\" class=\"icon\"><i class=\"fa fa-chevron-circle-down\"></i></button>\r\n" +
                    "            <button data-search=\"prev\" class=\"icon\"><i class=\"fa fa-chevron-circle-up\"></i></button>\r\n" +
                "        </div>\r\n" +
                    "    </div>");
                foreach (var categoriaGrupo in Categorias)
                {
                    var categoria = categoriaGrupo.Key;
                    sw.WriteLine("    <section class=\"u-clearfix u-custom-color-3 u-section-1\" src=\"\" id=\"sec-8951\" style=\"background-color:#" + categoria.corIcone + " !important;\">\r\n" +
                        "        <div class=\"u-align-left u-clearfix u-sheet u-valign-middle-lg u-valign-middle-md u-valign-middle-sm u-valign-middle-xs u-sheet-1\">\r\n" +
                        "            <h3 class=\"u-align-center u-text u-text-1\">" + categoria.nome + "</h3>\r\n" +
                        "        </div>\r\n" +
                        "    </section>");
                    sw.WriteLine("    <section class=\"u-clearfix u-gradient u-section-2\" id=\"sec-588b\">\r\n" +
                        "        <div class=\"u-expanded-width u-list u-list-1\">\r\n" +
                        "            <div class=\"u-repeater u-repeater-1\">");

                    foreach (var produtoCampanha in categoriaGrupo)
                    {
                        sw.WriteLine("                <div class=\"u-container-style u-list-item u-repeater-item\">\r\n" +
                                "                    <div class=\"u-container-layout u-similar-container u-container-layout-1\">\r\n" +
                                "                        <div class=\"u-align-left-xs u-image u-image-circle u-image-1\" alt=\"\" data-image-width=\"1300\" data-image-height=\"956\" style=\"background-image: url(" + produtoCampanha.Produto.imagem + ") !important; \"></div>\r\n" +
                                "                        <h4 class=\"u-align-right u-text u-text-custom-color-3 u-text-default-xs u-text-1\" style=\"color:#" + categoria.corIcone + " !important;\">" + produtoCampanha.Produto.nomeProduto + "</h4>");
                        var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
                        sw.WriteLine("                        <h4 class=\"u-align-right u-text u-text-default-xs u-text-grey-60 u-text-2\">" + valorFormatado + "</h4>");
                        sw.WriteLine("                    </div>\r\n" +
                            "                </div>");


                    }
                    sw.WriteLine("            </div>\r\n" +
                        "        </div>\r\n" +
                        "    </section>");
                }
                sw.WriteLine("    <footer class=\"u-align-left u-clearfix u-footer\" id=\"sec-8dad\">" +
                    "        <div class=\"u-align-left-xs u-clearfix u-sheet u-sheet-1\">\r\n" +
                    "            <a href=\"https://api.whatsapp.com/send?text=https://uaipdv.com.br/mangalarga/\" class=\"u-active-none u-align-center u-btn u-button-style u-hover-none u-none u-text-hover-palette-2-base u-text-palette-1-base u-btn-1\">" +
                    "                <span class=\"u-file-icon u-icon u-icon-1\"><img src=\"imagens/733585.png\" alt=\"\"></span>&nbsp;Compartilhar\r\n" +
                    "            </a>\r\n      " +
                    "        </div>\r\n" +
                    "    </footer>");
                sw.WriteLine("</body>\r\n</html>");
                sw.Close();
            }
            if (System.IO.File.Exists(targetPath + "/Index.html"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		public bool GenerateHtml2(string targetPath, int id, int tipo)
		{
            List<ProdutoCampanha> produtosCampanha = new List<ProdutoCampanha>();
            int codFilial = 0;
            if (tipo == 1)
            {
                var campanha = _context.Campanhas.Find(id);
                if (campanha != null)
                {
                    codFilial = campanha.codFilial;
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();

            }
            else
            {
                var evento = _context.Eventos.Find(id);
                if (evento != null)
                {
                    if (evento.codFilial!=null)
                    {
                        codFilial = (int)evento.codFilial;
                    }
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codEvento == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();

            }
            var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
            var filial = _context.Filiais.Find(codFilial);


            using (StreamWriter streamWriter = new StreamWriter(targetPath + "/Index.html"))
			{
				streamWriter.Write("<!DOCTYPE html>\r\n<html lang=\"pt-br\">\r\n<head>\r\n" +
                    "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n" +
                    "    <title>Cardapio de Buscar</title>\r\n" +
                    "    <meta charset=\"utf-8\">    <link rel=\"icon\" href=\"images/logo.png\">    <!-- Google Font -->\r\n" +
                    "    <link href=\"https://fonts.googleapis.com/css2?family=Poppins:wght@400;500&display=swap\"\r\n" +
                    "          rel=\"stylesheet\" />\r\n" +
                    "    <!-- Stylesheet -->\r\n" +
                    "    <link rel=\"stylesheet\" href=\"style.css\" />\r\n" +
                    "    \r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>\r\n" +
                    "    <script src=\"script.js\"></script>\r\n" +
                    "</head>\r\n");
				streamWriter.Write("<body>\r\n" +
                    "    <header class=\" u-header \" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\">\r\n" +
                    "         <section class=\" u-header \" id=\"sec-4448\"  style=\"background-image: url(" + filial.logoHome + ");\" data-image-width=\"1280\" data-image-height=\"724\"></section>\r\n" +
                    "    </header>\r\n" +
                    "    <div class=\"busca\">\r\n" +
                    "        <input id=\"filtro\" type=\"text\" placeholder=\"Busca Rápida\">\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"blocos\">\r\n");
				foreach (var categoriaGrupo in Categorias)
				{
					var categoria = categoriaGrupo.Key;
					string[] strArray = categoria.nome.Split(' ');
					char ch1 = ' ';
					char ch2 = strArray[0].FirstOrDefault<char>();
					for (int index = 0; index < strArray.Length; ++index)
					{
						if (index == 1)
							ch1 = strArray[index].FirstOrDefault<char>();
					}
					streamWriter.WriteLine("        <p>");

					foreach (var produtoCampanha in categoriaGrupo)
					{
						streamWriter.WriteLine("            <div class=\"bloco\" style=\"background-color:#" + categoria.corIcone + ";\">\r\n" +
                            "                <div class=\"quadrado\">\r\n");
						if (produtoCampanha.Produto.imagem == "" || produtoCampanha.Produto.imagem == null)
							streamWriter.WriteLine("                    <div id=\"texto\" class=\"circulo\">\r\n" +
                                "                        <h2 class=\"texto\">" + ch2.ToString() + "</h2>\r\n" +
                                "                    </div>\r\n");
						else
							streamWriter.WriteLine("                    <div class=\"imagens\" alt=\"\" data-image-width=\"1300\" data-image-height=\"956\" style=\"background-image: url(" + produtoCampanha.Produto.imagem + "); \">\r\n" +
                                "                </div>\r\n");
						streamWriter.WriteLine("                    </div>\r\n" +
                            "                <h3>" + produtoCampanha.Produto.descricaoDetalhada + " / " + categoria.nome + "</h3>");
						string str = string.Format((IFormatProvider)CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", (object)produtoCampanha.valor);
						streamWriter.WriteLine("                <p>" + str + "</p>");
						streamWriter.WriteLine("            </div>");
					}
					streamWriter.WriteLine("        </p>");
				}
				streamWriter.WriteLine("    </div>\r\n" +
                    "</body>\r\n" +
                    "</html>");
				streamWriter.Close();
			}
			return System.IO.File.Exists(targetPath + "/Index.html");
		}
		public bool GenerateHtml3(string targetPath, Campanha campanha)
		{
			var produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == campanha.codCampanha)
					.Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s=>s.Produto.nomeProduto);

			var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
			var filial = _context.Filiais.Find(campanha.codFilial);

			using (StreamWriter streamWriter1 = new StreamWriter(targetPath + "/Index.html"))
			{
				streamWriter1.WriteLine("<!DOCTYPE html>\r\n" +
                    "<html style=\"font-size: 16px;\" lang=\"pt-BR\">\r\n<head>\r\n" +
                    "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <meta charset=\"utf-8\">\r\n" +
                    "    <meta name=\"keywords\" content=\"Um novo conceito de para divulgar seus produtos\">\r\n" +
                    "    <meta name=\"description\" content=\"PrintPDV é Show para seu Ponto de Venda\">\r\n" +
                    "    <meta name=\"page_type\" content=\"np-template-header-footer-from-plugin\">\r\n" +
                    "    <title>Faça seu pedido</title>\r\n" +
                    "    <link rel=\"stylesheet\" href=\"nicepage.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"Seja-Bem-Vindo.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\">\r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>\r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/mark.js/8.11.1/jquery.mark.min.js\"></script>\r\n" +
                    "    <script class=\"u-script\" type=\"text/javascript\" src=\"nicepage.js\" defer=\"\"></script>\r\n" +
                    "    <meta name=\"generator\" content=\"BIX - Unidade de Atendimento Integrada\">\r\n" +
                    "    <meta property=\"og:title\" content=\"Acesse e Veja as opções no cardápio\">\r\n" +
                    "    <link rel=\"canonical\" href=\"Para todos os momentos.\">\r\n" +
                    "    <link rel=\"icon\" href=\"images/logo.png\">\r\n" +
                    "    <link id=\"u-theme-google-font\" rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Roboto+Slab:100,200,300,400,500,600,700,800,900|Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i\">\r\n" +
                    "    <script type=\"application/ld+json\">\r\n" +
                    "        {\r\n" +
                    "                \"@context\": \"http://schema.org\",\r\n" +
                    "                \"@type\": \"Organization\",\r\n" +
                    "                \"name\": \"Cardápio \",\r\n" +
                    "                \"url\": \"Para todos os momentos.\"}\r\n" +
                    "    </script>\r\n" +
                    "    <meta name=\"theme-color\" content=\"#0052ff\">\r\n" +
                    "    <meta property=\"og:description\" content=\"PrintPDV é Show para Ponto de Venda\">\r\n" +
                    "    <meta property=\"og:type\" content=\"website\">\r\n</head>");
				streamWriter1.WriteLine("<body class=\"u-body u-xl-mode content\">\r\n" +
                    "    <header class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\">\r\n" +
                    "        <section class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\" style=\"background-image: url(" + filial.logoHome + ") !Important;\"></section>\r\n" +
                    "    </header>\r\n" +
                    "    <div class=\"form\">\r\n");
				foreach (var categoriaGrupo in Categorias)
                {
                    var categoria = categoriaGrupo.Key;
					streamWriter1.Write("        <button class=\"btn eff\" onclick=\"categoria('.Cat" + categoria.codCategoria.ToString() + "')\" style=\"background: #" + categoria.corIcone + "\">" + categoria.nome + "</button>\r\n");

				}
				streamWriter1.Write("    </div>\r\n" +
                    "    <div class=\"flutuante eff\" onclick=\"pedido(lista)\">\r\n" +
                    "        <div id=\"mesa\">\r\n" +
                    "            <h3 id=\"NumMesa\">Mesa: </h3>\r\n" +
                    "        </div>\r\n" +
                    "        <div class=\"itens\">\r\n" +
                    "            <h4 class=\"flutuante-text\" style=\"margin-right:3px\" id=\"total\"> 0 </h4>\r\n" +
                    "            <h4 class=\"flutuante-text\">Itens</h4>\r\n" +
                    "        </div>\r\n" +
                    "        <h5 style=\"margin:0 0 0 0\">Valor:</h5>\r\n" +
                    "        <h5 class=\"conta\">R$ 0,00</h5>\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"ButtonZap\" onclick=\"pedido(lista)\">\r\n" +
                    "        <button class=\"pedido eff\"><span class=\"zapIcon u-file-icon u-icon u-icon-1\"><img src=\"images/733585.png\" alt=\"\"></span>&nbsp; <span class=\"mostrar\"> Enviar Pedido</span></button>\r\n" +
                    "    </div>\r\n");
				foreach (var categoriaGrupo in Categorias)
				{
					var categoria = categoriaGrupo.Key;
					streamWriter1.Write("    <section class=\"Cat" + categoria.codCategoria.ToString() + " u-clearfix u-custom-color-3 u-section-1\" src=\"\" id=\"Cat" + categoria.codCategoria.ToString() + "\" style=\"background-color:#" + categoria.corIcone + " !important;\">\r\n" +
                        "        <div class=\"u-align-left u-clearfix u-sheet u-valign-middle-lg u-valign-middle-md u-valign-middle-sm u-valign-middle-xs u-sheet-1\">\r\n" +
                        "            <h3 class=\"u-align-center u-text u-text-1\">" + categoria.nome + "</h3>\r\n" +
                        "        </div>\r\n" +
                        "    </section>");
					streamWriter1.Write("    <section class=\"u-clearfix u-gradient u-section-2\" id=\"sec-588b\" style=\"border-color: #" + categoria.corIcone + "\">\r\n" +
                        "        <div class=\"u-expanded-width u-list u-list-1\">\r\n" +
                        "            <div class=\"u-repeater u-repeater-1\">");
                    foreach (var produtoCampanha in categoriaGrupo)
                    {
						string str1 = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
						StreamWriter streamWriter2 = streamWriter1;
						string[] strArray = new string[19];
						strArray[0] = "<div class=\"u-container-style u-list-item u-repeater-item\">\r\n" +
							"                    <div class=\"u-container-layout u-similar-container u-container-layout-1\">\r\n" +
							"                        <div class=\"u-align-left-xs u-image u-image-circle u-image-1\" alt=\"\" data-image-width=\"1300\" data-image-height=\"956\" style=\"background-image: url(";
						strArray[1] = produtoCampanha.Produto.imagem;
						strArray[2] = ") !important; \"></div>\r\n" +
							"                        <h4 class=\"nome";
						int codProduto = produtoCampanha.Produto.codProduto;
						strArray[3] = codProduto.ToString();
						strArray[4] = " u-align-right u-text u-text-custom-color-3 u-text-default-xs u-text-1\" style=\"color:#";
						strArray[5] = categoria.corIcone;
						strArray[6] = " !important;\">";
						strArray[7] = produtoCampanha.Produto.nomeProduto;
						strArray[8] = "</h4>\r\n" +
							"                        <h4 class=\"t";
						codProduto = produtoCampanha.Produto.codProduto;
						strArray[9] = codProduto.ToString();
						strArray[10] = " u-align-right u-text u-text-default-xs u-text-grey-60 u-text-2\">";
						strArray[11] = str1;
						strArray[12] = "</h4>\r\n" +
							"                        <div class=\"add-text\">\r\n" +
							"                            <button class=\"icon\" onclick=\"subtrair(";
						strArray[13] = produtoCampanha.Produto.codProduto.ToString();
						strArray[14] = ",lista)\">\r\n" +
							"                                <i class='fa fa-minus'></i>\r\n" +
							"                            </button>\r\n" +
							"                            <h3 id=\"t";
						strArray[15] = produtoCampanha.Produto.codProduto.ToString();
						strArray[16] = "\" class=\"add\">0</h3>\r\n" +
							"                            <button class=\"icon\" onclick=\"somar(";
						strArray[17] = produtoCampanha.Produto.codProduto.ToString();
						strArray[18] = ",lista)\">\r\n" +
							"                                <i class='fa fa-plus'></i>\r\n" +
							"                            </button>\r\n" +
							"                        </div>\r\n" +
							"                    </div>\r\n" +
							"                </div>";
						string str2 = string.Concat(strArray);
						streamWriter2.Write(str2);
					}
					
					streamWriter1.Write("            </div>\r\n" +
                        "        </div>\r\n" +
                        "    </section>\r\n");
				}
				streamWriter1.Write("<footer class=\"u-align-left u-clearfix u-footer\" id=\"sec-8dad\">\r\n" +
                    "        <div class=\"u-align-left-xs u-clearfix u-sheet u-sheet-1\">\r\n" +
                    "            <button href=\"#\" onclick=\"pedido(lista)\" class=\"u-active-none u-align-center u-btn u-button-style u-hover-none u-none u-text-hover-palette-2-base u-text-palette-1-base u-btn-1\">\r\n" +
                    "            </button>\r\n" +
                    "        </div>\r\n" +
                    "    </footer>");
				streamWriter1.Write("    <script src=\"funcoes.js\">\r\n" +
                    "    </script>\r\n" +
                    "    <script>\r\n" +
                    "        function pedido(li) {\r\n" +
                    "            if (li.length > 0) {\r\n" +
                    "                var Fonenumber = '55" + filial.telefone + "';\r\n" +
                    "                sendRequest(li, FoneCliente,1)\r\n" +
                    "            }\r\n" +
                    "        }\r\n" +
                    "    </script>\r\n" +
                    "</body>\r\n" +
                    "</html>\r\n");
				streamWriter1.Close();
			}
			return System.IO.File.Exists(targetPath + "/Index.html");
		}
		public bool GenerateHtml4(string targetPath, Campanha campanha)
		{
			var produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == campanha.codCampanha)
					.Include(p => p.Produto).ThenInclude(prod => prod.Categoria).Include(s=>s.Produto.Modificadores).OrderBy(s => s.Produto.nomeProduto);

			var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
			var filial = _context.Filiais.Find(campanha.codFilial);

			using (StreamWriter streamWriter1 = new StreamWriter(targetPath + "/Index.html"))
			{
				streamWriter1.WriteLine("<!DOCTYPE html>\r\n<html style=\"font-size: 16px;\" lang=\"pt-BR\">\r\n" +
                    "<head>\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <meta charset=\"utf-8\">\r\n    <meta name=\"keywords\" content=\"Um novo conceito de para divulgar seus produtos\">\r\n" +
                    "    <meta name=\"description\" content=\"PrintPDV é Show para seu Ponto de Venda\">\r\n" +
                    "    <meta name=\"page_type\" content=\"np-template-header-footer-from-plugin\">\r\n" +
                    "    <title>Faça seu pedido</title>\r\n    <link rel=\"stylesheet\" href=\"nicepage.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"Seja-Bem-Vindo.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\">\r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>\r\n" +
                    "    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/mark.js/8.11.1/jquery.mark.min.js\"></script>\r\n" +
                    "    <script class=\"u-script\" type=\"text/javascript\" src=\"nicepage.js\" defer=\"\"></script>\r\n" +
                    "    <meta name=\"generator\" content=\"BIX - Unidade de Atendimento Integrada\">\r\n" +
                    "    <meta property=\"og:title\" content=\"Acesse e Veja as opções no cardápio\">\r\n" +
                    "    <link rel=\"canonical\" href=\"Para todos os momentos.\">\r\n" +
                    "    <link rel=\"icon\" href=\"images/logo.png\">\r\n" +
                    "    <link id=\"u-theme-google-font\" rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Roboto+Slab:100,200,300,400,500,600,700,800,900|Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i\">\r\n" +
                    "    <script type=\"application/ld+json\">\r\n" +
                    "        {\r\n" +
                    "                \"@context\": \"http://schema.org\",\r\n" +
                    "                \"@type\": \"Organization\",\r\n" +
                    "                \"name\": \"Cardápio \",\r\n" +
                    "                \"url\": \"Para todos os momentos.\"}\r\n" +
                    "    </script>\r\n" +
                    "    <meta name=\"theme-color\" content=\"#0052ff\">\r\n" +
                    "    <meta property=\"og:description\" content=\"PrintPDV é Show para Ponto de Venda\">\r\n" +
                    "    <meta property=\"og:type\" content=\"website\">\r\n</head>");
				streamWriter1.WriteLine("<body class=\"u-body u-xl-mode content\">\r\n" +
                    "    <header class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\">\r\n" +
                    "        <section class=\"u-align-center-xs u-align-left-lg u-align-left-md u-align-left-sm u-align-left-xl u-clearfix u-header u-image u-header\" id=\"sec-4448\" data-image-width=\"1280\" data-image-height=\"724\" style=\"background-image: url(" + filial.logoHome + ") !Important;\"></section>\r\n" +
                    "    </header>\r\n" +
                    "    <div class=\"form\">\r\n");

				foreach (var categoriaGrupo in Categorias)
                {
                    var categoria = categoriaGrupo.Key;
					streamWriter1.Write("        <button class=\"btn eff\" onclick=\"categoria('.Cat" + categoria.codCategoria.ToString() + "')\" style=\"background: #" + categoria.corIcone + "\">" + categoria.nome + "</button>\r\n");

				}

				streamWriter1.Write("    </div>\r\n" +
                    "    <div class=\"flutuante eff\" onclick=\"pedido(lista)\">\r\n" +
                    "        <div id=\"mesa\">\r\n" +
                    "            <h3 id=\"NumMesa\">Mesa: </h3>\r\n" +
                    "        </div>\r\n" +
                    "        <div class=\"itens\">\r\n" +
                    "            <h4 class=\"flutuante-text\" style=\"margin-right:3px\" id=\"total\"> 0 </h4>\r\n" +
                    "            <h4 class=\"flutuante-text\">Itens</h4>\r\n" +
                    "        </div>\r\n" +
                    "        <h5 style=\"margin:0 0 0 0\">Valor:</h5>\r\n" +
                    "        <h5 class=\"conta\">R$ 0,00</h5>\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"ButtonZap\" onclick=\"pedido(lista)\">\r\n" +
                    "        <button class=\"pedido eff\"><span class=\"zapIcon u-file-icon u-icon u-icon-1\"><img src=\"images/733585.png\" alt=\"\"></span>&nbsp; <span class=\"mostrar\"> Enviar Pedido</span></button>\r\n" +
                    "    </div>\r\n");
				foreach (var categoriaGrupo in Categorias)
				{
					var categoria = categoriaGrupo.Key;
					streamWriter1.Write("    <section class=\"Cat" + categoria.codCategoria.ToString() + " u-clearfix u-custom-color-3 u-section-1\" src=\"\" id=\"Cat" + categoria.codCategoria.ToString() + "\" style=\"background-color:#" + categoria.corIcone + " !important;\">\r\n" +
                        "        <div class=\"u-align-left u-clearfix u-sheet u-valign-middle-lg u-valign-middle-md u-valign-middle-sm u-valign-middle-xs u-sheet-1\">\r\n" +
                        "            <h3 class=\"u-align-center u-text u-text-1\">" + categoria.nome + "</h3>\r\n" +
                        "        </div>\r\n" +
                        "    </section>");
					streamWriter1.Write("    <section class=\"u-clearfix u-gradient u-section-2\" id=\"sec-588b\" style=\"border-color: #" + categoria.corIcone + "\">\r\n" +
                        "        <div class=\"u-expanded-width u-list u-list-1\">\r\n" +
                        "            <div class=\"u-repeater u-repeater-1\">");

                    foreach (var produtoCampanha in categoriaGrupo)
                    {
						string str1 = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", (object)produtoCampanha.valor);
						if (produtoCampanha.Produto.Modificadores != null && produtoCampanha.Produto.Modificadores.Count() > 0)
							streamWriter1.Write("<div class=\"u-container-style u-list-item u-repeater-item\">\r\n" +
                                "                    <div class=\"u-container-layout u-similar-container u-container-layout-1\">\r\n" +
                                "                        <div class=\"u-align-left-xs u-image u-image-circle u-image-1\" alt=\"\" data-image-width=\"1300\" data-image-height=\"956\" style=\"background-image: url(" + produtoCampanha.Produto.imagem + ") !important; \"></div>\r\n" +
                                "                        <h4 class=\"nome" + produtoCampanha.codProduto.ToString() + " u-align-right u-text u-text-custom-color-3 u-text-default-xs u-text-1\" style=\"color:#" +categoria.corIcone + " !important;\">" + produtoCampanha.Produto.nomeProduto + "</h4>\r\n" +
                                "                        <h4 class=\"t" + produtoCampanha.codProduto.ToString() + " u-align-right u-text u-text-default-xs u-text-grey-60 u-text-2\">" + str1 + "</h4>\r\n" +
                                "                        <div class=\"add-text\">\r\n" +
                                "                            <button class=\"icon\" onclick=\"subtrair(" + produtoCampanha.codProduto.ToString() + ",lista),modSub(" + produtoCampanha.codProduto.ToString() + ")\">\r\n" +
                                "                                <i class='fa fa-minus'></i>\r\n" +
                                "                            </button>\r\n" +
                                "                            <h3 id=\"t" + produtoCampanha.codProduto.ToString() + "\" class=\"add\">0</h3>\r\n" +
                                "                            <button class=\"icon\" onclick=\"somar(" + produtoCampanha.codProduto.ToString() + ",lista),modSoma(" + produtoCampanha.codProduto.ToString() + ")\">\r\n" +
                                "                                <i class='fa fa-plus'></i>\r\n" +
                                "                            </button>\r\n" +
                                "                        </div>\r\n" +
                                "                    </div>\r\n" +
                                "                </div>");
						else
							streamWriter1.Write("<div class=\"u-container-style u-list-item u-repeater-item\">\r\n" +
                                "                    <div class=\"u-container-layout u-similar-container u-container-layout-1\">\r\n" +
                                "                        <div class=\"u-align-left-xs u-image u-image-circle u-image-1\" alt=\"\" data-image-width=\"1300\" data-image-height=\"956\" style=\"background-image: url(" + produtoCampanha.Produto.imagem + ") !important; \"></div>\r\n" +
                                "                        <h4 class=\"nome" + produtoCampanha.codProduto.ToString() + " u-align-right u-text u-text-custom-color-3 u-text-default-xs u-text-1\" style=\"color:#" + categoria.corIcone + " !important;\">" + produtoCampanha.Produto.nomeProduto + "</h4>\r\n" +
                                "                        <h4 class=\"t" + produtoCampanha.codProduto.ToString() + " u-align-right u-text u-text-default-xs u-text-grey-60 u-text-2\">" + str1 + "</h4>\r\n" +
                                "                        <div class=\"add-text\">\r\n" +
                                "                            <button class=\"icon\" onclick=\"subtrair(" + produtoCampanha.codProduto.ToString() + ",lista)\">\r\n" +
                                "                                <i class='fa fa-minus'></i>\r\n" +
                                "                            </button>\r\n" +
                                "                            <h3 id=\"t" + produtoCampanha.codProduto.ToString() + "\" class=\"add\">0</h3>\r\n" +
                                "                            <button class=\"icon\" onclick=\"somar(" + produtoCampanha.codProduto.ToString() + ",lista)\">\r\n" +
                                "                                <i class='fa fa-plus'></i>\r\n" +
                                "                            </button>\r\n" +
                                "                        </div>\r\n" +
                                "                    </div>\r\n" +
                                "                </div>");
						if (produtoCampanha.Produto.Modificadores != null && produtoCampanha.Produto.Modificadores.Count() > 0)
						{
							StreamWriter streamWriter2 = streamWriter1;
							string[] strArray = new string[9];
							strArray[0] = "<div class=\"panel\" id=\"adicional";
							int codProduto = produtoCampanha.codProduto;
							strArray[1] = codProduto.ToString();
							strArray[2] = "\">   <div class=\"mod1\" id=\"mod";
							codProduto = produtoCampanha.codProduto;
							strArray[3] = codProduto.ToString();
							strArray[4] = "-1\">\r\n" +
                                "                        <section class=\"u-clearfix u-custom-color-3 u-section-1\" src=\"\" style=\"background-color:#";
							strArray[5] = categoria.corIcone;
							strArray[6] = " !important; width:95%;\">\r\n" +
                                "                            <div class=\"u-align-left u-clearfix u-sheet u-valign-middle-lg u-valign-middle-md u-valign-middle-sm u-valign-middle-xs u-sheet-1\">\r\n" +
                                "                                <h3 class=\"u-align-center u-text u-text-1\" style=\"margin:auto;\">Adicional ";
							strArray[7] = produtoCampanha.Produto.nomeProduto;
							strArray[8] = " n1</h3>\r\n" +
                                "                            </div>\r\n" +
                                "                        </section>";
							string str2 = string.Concat(strArray);
							streamWriter2.WriteLine(str2);
						}
						foreach (Modificador modificador in produtoCampanha.Produto.Modificadores)
						{
							string str3 = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", modificador.preco);
							StreamWriter streamWriter3 = streamWriter1;
							string[] strArray = new string[19];
							strArray[0] = "                        <div class=\"u-container-layout u-similar-container u-container-layout-1 display-mod\">\r\n" +
                                "                            <h4 class=\"nomemod";
							int num = modificador.codigo;
							strArray[1] = num.ToString();
							strArray[2] = " name-adi u-align-left-xs u-text u-text-custom-color-3 u-text-default-xs u-text-stilo\" style=\"color:#";
							strArray[3] = categoria.corIcone;
							strArray[4] = "!important;\">";
							strArray[5] = modificador.descricaoModificador;
							strArray[6] = "</h4>\r\n" +
                                "                            <h4 class=\"modt";
							num = modificador.codigo;
							strArray[7] = num.ToString();
							strArray[8] = " addValor u-align-right u-text u-text-default-xs u-text-grey-60 u-text-2 \" style=\"width:45%;\">";
							strArray[9] = str3;
							strArray[10] = "</h4>\r\n" +
                                "                            <div class=\"add-text icon-add-text\">\r\n" +
                                "                                <button id=\"c1\" class=\"icon sub\" onclick=\"subtrairmod(this,";
							num = modificador.codigo;
							strArray[11] = num.ToString();
							strArray[12] = ",listamod)\">\r\n" +
                                "                                    <i class='fa fa-minus'></i>\r\n" +
                                "                                </button>\r\n" +
                                "                                <h3 id=\"modt";
							num = modificador.codigo;
							strArray[13] = num.ToString();
							strArray[14] = "-1\" class=\"add icon-adi qtdmod\">0</h3>\r\n" +
                                "                                <button id=\"c1\" class=\"accordion icon \" onclick=\"somarmod(this,";
							num = modificador.codigo;
							strArray[15] = num.ToString();
							strArray[16] = ",listamod,";
							num = modificador.codproduto;
							strArray[17] = num.ToString();
							strArray[18] = ")\">\r\n" +
                                "                                    <i class='fa fa-plus'></i>\r\n" +
                                "                                </button>\r\n" +
                                "                            </div>\r\n" +
                                "                        </div>";
							string str4 = string.Concat(strArray);
							streamWriter3.WriteLine(str4);
						}
						streamWriter1.WriteLine("                        </div>\r\n" +
                            "                    </div>");

					}
					streamWriter1.Write("            </div>\r\n" +
                        "        </div>\r\n" +
                        "    </section>\r\n");
				}
				streamWriter1.Write("<footer class=\"u-align-left u-clearfix u-footer\" id=\"sec-8dad\">\r\n        <div class=\"u-align-left-xs u-clearfix u-sheet u-sheet-1\">\r\n            <button href=\"#\" onclick=\"pedido(lista)\" class=\"u-active-none u-align-center u-btn u-button-style u-hover-none u-none u-text-hover-palette-2-base u-text-palette-1-base u-btn-1\">\r\n            </button>\r\n        </div>\r\n    </footer>");
				streamWriter1.Write("    <script src=\"funcoes.js\">\r\n" +
                    "    </script>\r\n" +
                    "    <script>\r\n" +
                    "        function pedido(li) {\r\n" +
                    "            if (li.length > 0) {\r\n" +
                    "                var Fonenumber = '55" + filial.telefone + "';\r\n" +
                    "                enviar(li, Fonenumber)\r\n" +
                    "            }\r\n" +
                    "        }\r\n" +
                    "    </script>\r\n" +
                    "</body>\r\n" +
                    "</html>\r\n");
				streamWriter1.Close();
			}
			return System.IO.File.Exists(targetPath + "/Index.html");
		}
		public bool GenerateHtml5(string targetPath, Campanha campanha)
		{
			var produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == campanha.codCampanha)
					.Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto);

			var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
			var filial = _context.Filiais.Find(campanha.codFilial);

			using (StreamWriter streamWriter1 = new StreamWriter(targetPath + "/Index.html"))
			{
				streamWriter1.WriteLine("<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n" +
                    "    <meta charset=\"UTF-8\">\r\n" +
                    "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"styles/cardapio.css\">\r\n\r\n" +
                    "    <title>Cardápio Online</title>\r\n</head>\r\n<body>\r\n\r\n" +
                    "    <div class=\"banner\">\r\n" +
                    "        <img src=\"" + filial.logoHome + "\" alt=\"Banner exemplo 1\">\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"flutuante eff\" onclick=\"pedido(lista)\">\r\n" +
                    "        <div id=\"mesa\">\r\n" +
                    "            <h3 id=\"NumMesa\">Mesa: </h3>\r\n" +
                    "        </div>\r\n" +
                    "        <div class=\"itens\">\r\n" +
                    "            <h4 class=\"flutuante-text\" style=\"margin-right:3px\" id=\"total\"> 0 </h4>\r\n" +
                    "            <h4 class=\"flutuante-text\">Itens</h4>\r\n" +
                    "        </div>\r\n" +
                    "        <h4 style=\"margin:0 0 0 0\">Valor:</h4>\r\n" +
                    "        <h3 class=\"conta\">R$ 0,00</h3>\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"ButtonZap\" onclick=\"pedido(lista)\">\r\n" +
                    "        <button class=\"pedido eff\"><span class=\"zapIcon \"><img class=\"img-fluid\" src=\"images/733585.png\" alt=\"\"></span>&nbsp; <span class=\"mostrar\"> Enviar Pedido</span></button>\r\n" +
                    "    </div>\r\n" +
                    "    <div class=\"menu\">\r\n" +
                    "        <div class=\"category-nav\">\r\n" +
                    "            <ul class=\"category-list\">");

                foreach (var categoriaGrupo in Categorias)
                {
                    var categoria = categoriaGrupo.Key;
                    streamWriter1.WriteLine("<li><a href=\"#G" + categoria.codCategoria.ToString() + "\">" + categoria.nome + "</a></li>");
                }
                streamWriter1.WriteLine("           </ul>\r\n" +
                    "        </div>");
				foreach (var categoriaGrupo in Categorias)
				{
					var categoria = categoriaGrupo.Key;
					streamWriter1.WriteLine("        <div class=\"category\" id=\"G" + categoria.codCategoria.ToString() + "\">\r\n" +
                        "            <h2>" + categoria.nome + "</h2>");

					foreach (var produtoCampanha in categoriaGrupo)
					{
						string str1 = string.Format((IFormatProvider)CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", (object)produtoCampanha.valor);
						StreamWriter streamWriter2 = streamWriter1;
						string[] strArray = new string[19];
						strArray[0] = "    <div class=\"menu-item\">\r\n" +
                            "                <div class=\"item-image\" style=\"background-image: url('";
						strArray[1] = produtoCampanha.Produto.imagem;
						strArray[2] = "');\"></div>\r\n" +
                            "                <div class=\"item-details\">\r\n" +
                            "                    <h3 class=\"nome";
						int codProduto = produtoCampanha.Produto.codProduto;
						strArray[3] = codProduto.ToString();
						strArray[4] = "\">";
						strArray[5] = produtoCampanha.Produto.nomeProduto;
						strArray[6] = "</h3>\r\n" +
                            "                    <p class=\"item-description\">";
						strArray[7] = produtoCampanha.Produto.descricaoDetalhada;
						strArray[8] = "</p>\r\n" +
                            "                </div>\r\n" +
                            "                <div class=\"item-price\">\r\n" +
                            "                    <div class=\"text-price\">\r\n" +
                            "                        <p class=\"t";
						codProduto = produtoCampanha.Produto.codProduto;
						strArray[9] = codProduto.ToString();
						strArray[10] = "\">";
						strArray[11] = str1;
						strArray[12] = "</p>\r\n" +
                            "                    </div>\r\n" +
                            "                    <span class=\"quantity-control\" onclick=\"subtrair(";
						strArray[13] = produtoCampanha.Produto.codProduto.ToString();
						strArray[14] = ",lista)\">-</span>\r\n" +
                            "                    <span class=\"quantity\" id=\"t";
						strArray[15] = produtoCampanha.codProduto.ToString();
						strArray[16] = "\">0</span>\r\n" +
                            "                    <span class=\"quantity-control\" onclick=\"somar(";
						strArray[17] = produtoCampanha.codProduto.ToString();
						strArray[18] = ",lista)\">+</span>\r\n" +
                            "                </div>\r\n" +
                            "            </div>";
						string str2 = string.Concat(strArray);
						streamWriter2.WriteLine(str2);
					}
					streamWriter1.WriteLine("        </div>");
				}
				streamWriter1.WriteLine("    <script src=\"funcoes.js\"></script>\r\n" +
                    "    <script>\r\n" +
                    "        function pedido(li) {\r\n" +
                    "            if (li.length > 0) {\r\n" +
                    "                var Fonenumber = '" + filial.telefone + "';\r\n" +
                    "                enviar(li, Fonenumber)\r\n" +
                    "            }\r\n" +
                    "        }\r\n" +
                    "    </script>\r\n" +
                    "</body>\r\n" +
                    "</html>");
				streamWriter1.Close();
			}
			return System.IO.File.Exists(targetPath + "/Index.html");
		}
		public bool GenerateHtml6(string targetPath, int id, int tipo)
		{
            List<ProdutoCampanha> produtosCampanha = new List<ProdutoCampanha>();
            int codFilial = 0;
            if (tipo == 1)
            {
                var campanha = _context.Campanhas.Find(id);
                if (campanha != null)
                {
                    codFilial = campanha.codFilial;
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codCampanha == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();

            }
            else
            {
                var evento = _context.Eventos.Find(id);
                if (evento != null)
                {
                    if (evento.codFilial!=null)
                    {
                        codFilial = (int)evento.codFilial;
                    }
                }
                produtosCampanha = _context.ProdutosCampanha.Where(c => c.codEvento == id)
                    .Include(p => p.Produto).ThenInclude(prod => prod.Categoria).OrderBy(s => s.Produto.nomeProduto).ToList();

            }
            var Categorias = produtosCampanha.GroupBy(p => p.Produto.Categoria).ToList();
            var filial = _context.Filiais.Find(codFilial);
            filial.Endereco = _context.Enderecos.Find(filial.codEndereco);

            if (filial.logoHome == null)
				filial.logoHome = "images/default-logo.png";

			using (StreamWriter streamWriter = new StreamWriter(targetPath + "/Index.html"))
			{
				streamWriter.WriteLine("<!DOCTYPE html>\r\n" +
                    "<html style=\"font-size: 16px;\" lang=\"pt\">\r\n" +
                    "<head>\r\n" +
                    "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <meta charset=\"utf-8\">\r\n" +
                    "    <meta name=\"keywords\" content=\"Sample Headline, Sample Headline\">\r\n" +
                    "    <meta name=\"description\" content=\"\">\r\n" +
                    "    <title>Home - Cardápio</title>\r\n" +
                    "    <link rel=\"stylesheet\" href=\"nicepage.css\" media=\"screen\">\r\n" +
                    "    <link rel=\"stylesheet\" href=\"Página-Inicial.css\" media=\"screen\">\r\n" +
                    "    <script class=\"u-script\" type=\"text/javascript\" src=\"jquery.js\" defer=\"\"></script>\r\n" +
                    "    <script class=\"u-script\" type=\"text/javascript\" src=\"nicepage.js\" defer=\"\"></script>\r\n" +
                    "    <meta name=\"generator\" content=\"Nicepage 6.3.1, nicepage.com\">\r\n" +
                    "    <link id=\"u-theme-google-font\" rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i|Open+Sans:300,300i,400,400i,500,500i,600,600i,700,700i,800,800i\">\r\n" +
                    "    <link id=\"u-page-google-font\" rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Merriweather:300,300i,400,400i,700,700i,900,900i\">\r\n" +
                    "    <script type=\"application/ld+json\">        {\r\n" +
                    "                \"@context\": \"http://schema.org\",\r\n" +
                    "                \"@type\": \"Organization\",\r\n" +
                    "                \"name\": \"\",\r\n" +
                    "                \"logo\": \"images/default-logo.png\"\r\n" +
                    "        }</script>\r\n" +
                    "    <meta name=\"theme-color\" content=\"#478ac9\">\r\n" +
                    "    <meta property=\"og:title\" content=\"Home - Cardápio\">\r\n" +
                    "    <meta property=\"og:type\" content=\"website\">\r\n" +
                    "    <meta data-intl-tel-input-cdn-path=\"intlTelInput/\">\r\n</head>\r\n");
				streamWriter.WriteLine("<body data-home-page=\"Página-Inicial.html\" data-home-page-title=\"Página Inicial\" data-path-to-root=\"./\" data-include-products=\"true\" class=\"u-body u-overlap u-xl-mode\" data-lang=\"pt\">\r\n" +
                    "    <header class=\"u-clearfix u-header u-white u-header\" id=\"sec-fce1\">\r\n" +
                    "        <div class=\"u-clearfix u-sheet u-sheet-1\">\r\n" +
                    "            <a href=\"#\" class=\"u-image u-logo u-image-1\" data-image-width=\"80\" data-image-height=\"40\">\r\n" +
                    "                <img src=\"" + filial.logoHome + "\" class=\"u-logo-image u-logo-image-1\">\r\n" +
                    "            </a>\r\n" +
                    "            <nav class=\"u-align-left u-font-size-14 u-menu u-menu-hamburger u-nav-spacing-25 u-offcanvas u-menu-1\" data-responsive-from=\"XL\">\r\n" +
                    "                <div class=\"menu-collapse\">\r\n" +
                    "                    <a class=\"u-button-style u-nav-link\" href=\"#\" style=\"padding: 4px 0px; font-size: calc(1em + 8px);\">\r\n" +
                    "                        <svg class=\"u-svg-link\" preserveAspectRatio=\"xMidYMin slice\" viewBox=\"0 0 302 302\" style=\"\"><use xmlns:xlink=\"http://www.w3.org/1999/xlink\" xlink:href=\"#svg-7b92\"></use></svg>\r\n" +
                    "                        <svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" id=\"svg-7b92\" x=\"0px\" y=\"0px\" viewBox=\"0 0 302 302\" style=\"enable-background:new 0 0 302 302;\" xml:space=\"preserve\" class=\"u-svg-content\"><g><rect y=\"36\" width=\"302\" height=\"30\"></rect><rect y=\"236\" width=\"302\" height=\"30\"></rect><rect y=\"136\" width=\"302\" height=\"30\"></rect>\r\n</g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg>\r\n" +
                    "                    </a>\r\n" +
                    "                </div>\r\n" +
                    "                <div class=\"u-custom-menu u-nav-container\">\r\n" +
                    "                    <ul class=\"u-nav u-unstyled u-nav-1\">\r\n" +
                    "                        <li class=\"u-nav-item\">\r\n" +
                    "                            <a class=\"u-button-style u-nav-link u-text-active-palette-1-base u-text-hover-palette-2-base\" href=\"#\" style=\"padding: 10px 20px;\">Página Inicial</a>\r\n" +
                    "                        </li>\r\n" +
                    "                        <li class=\"u-nav-item\">\r\n" +
                    "                            <a class=\"u-button-style u-nav-link u-text-active-palette-1-base u-text-hover-palette-2-base\" href=\"#\" style=\"padding: 10px 20px;\">Sobre</a>\r\n" +
                    "                        </li>\r\n" +
                    "                        <li class=\"u-nav-item\">\r\n" +
                    "                            <a class=\"u-button-style u-nav-link u-text-active-palette-1-base u-text-hover-palette-2-base\" href=\"#\" style=\"padding: 10px 20px;\">Contato</a>\r\n" +
                    "                        </li>\r\n" +
                    "                    </ul>\r\n" +
                    "                </div>\r\n" +
                    "                <div class=\"u-custom-menu u-nav-container-collapse\">\r\n" +
                    "                    <div class=\"u-align-center u-black u-container-style u-inner-container-layout u-opacity u-opacity-95 u-sidenav\">\r\n" +
                    "                        <div class=\"u-inner-container-layout u-sidenav-overflow\">\r\n" +
                    "                            <div class=\"u-menu-close\"></div>\r\n" +
                    "                            <ul class=\"u-align-center u-nav u-popupmenu-items u-unstyled u-nav-2\">\r\n" +
                    "                                <li class=\"u-nav-item\">\r\n" +
                    "                                    <p class=\"u-button-style u-nav-link\">" + filial.nome + "</p>\r\n" +
                    "                                </li>\r\n" +
                    "                                <li class=\"u-nav-item\">\r\n" +
                    "                                    <p class=\"u-button-style u-nav-link\">Endereço: " + filial.Endereco.logradouro + ", n" + filial.Endereco.numeroendereco.ToString() + " </p>\r\n" +
                    "                                </li>\r\n" +
                    "                                <li class=\"u-nav-item\">\r\n" +
                    "                                    <p class=\"u-button-style u-nav-link\">Cidade: " + filial.Endereco.cidade + ", " + filial.Endereco.numeroendereco.ToString() + ". Bairro: " + filial.Endereco.bairro + "</p>\r\n" +
                    "                                </li>\r\n" +
                    "                                <li class=\"u-nav-item\">\r\n" +
                    "                                    <p class=\"u-button-style u-nav-link\">Contato: " + filial.telefone + "</p>\r\n" +
                    "                                </li>\r\n" +
                    "                            </ul>\r\n" +
                    "                        </div>\r\n" +
                    "                    </div>\r\n" +
                    "                    <div class=\"u-black u-menu-overlay u-opacity u-opacity-70\"></div>\r\n" +
                    "                </div>\r\n" +
                    "            </nav>\r\n" +
                    "        </div>\r\n" +
                    "    </header>\r\n");
				streamWriter.WriteLine("<section class=\"u-clearfix u-palette-4-light-3 u-section-1\" id=\"sec-b6c5\">\r\n\r\n" +
                    "        <div class=\"u-container-style u-expanded-width-sm u-expanded-width-xs u-group u-shape-rectangle u-group-1\">\r\n" +
                    "            <div class=\"u-container-layout u-container-layout-1\" id=\"filtroCategoria\">\r\n" +
                    "                <div id=\"divBusca\">\r\n" +
                    "                    <input type=\"text\" id=\"txtBusca\" placeholder=\"O que você procura:\" />\r\n" +
                    "                    <svg xmlns=\"http://www.w3.org/2000/svg\" width=\"20\" height=\"20\" fill=\"currentColor\" id=\"btnBusca\" class=\"bi bi-search iconP\" viewBox=\"0 0 16 16\">\r\n" +
                    "                        <path d=\"M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001q.044.06.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1 1 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0\" />\r\n" +
                    "                    </svg>\r\n" +
                    "                </div>\r\n" +
                    "                <div class=\"categorias\">\r\n" +
                    "                    <div class=\"u-container-style u-expanded-width-lg u-expanded-width-md u-expanded-width-xl u-group u-shape-rectangle u-group-3\">\r\n" +
                    "                        <div onclick=\"filtrarPorCategoria(this,'Todos')\" class=\"button u-container-layout u-container-layout-3 barraEfect\">\r\n" +
                    "                            <h3 class=\"u-text u-text-2\">Todos</h3>\r\n" +
                    "                        </div>\r\n" +
                    "                    </div>");

                foreach (var categoriaGrupo in Categorias)
                {
                    var categoria = categoriaGrupo.Key;
                    streamWriter.WriteLine("                <div class=\"u-container-style u-expanded-width-lg u-expanded-width-md u-expanded-width-xl u-group u-shape-rectangle u-group-3\">\r\n" +
                        "                        <div onclick=\"filtrarPorCategoria(this,'" + categoria.nome + "')\" class=\"button u-container-layout u-container-layout-3\">\r\n" +
                        "                            <h3 class=\"u-text u-text-2\">" + categoria.nome + "</h3>\r\n" +
                        "                        </div>\r\n" +
                        "                    </div>");
                }
                streamWriter.WriteLine("                </div>\r\n" +
                    "            </div>\r\n" +
                    "        </div>\r\n" +
                    "        <h4 id=\"categoria\" class=\"u-text u-text-default u-text-grey-60 u-text-4\">Todos</h4>\r\n" +
                    "        <div class=\"custom-expanded u-list u-preserve-proportions u-list-1\">\r\n" +
                    "            <div class=\"u-repeater u-repeater-1\">\r\n");
				foreach (var categoriaGrupo in Categorias)
				{
					var categoria = categoriaGrupo.Key;
					
                    foreach (var produtoCampanha in categoriaGrupo)
					{
						string str = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
						if (produtoCampanha.Produto.imagem == null || produtoCampanha.Produto.imagem == "")
							produtoCampanha.Produto.imagem = "images/produto.png";
						streamWriter.WriteLine("                <div id=\"" + categoria.nome + "\" class=\"item bebidas u-container-style u-list-item u-repeater-item u-white u-list-item-1 bloco\">\r\n" +
                            "                    <div class=\"divPlay\">\r\n" +
                            "                        <img class=\"image\" src=\"" + produtoCampanha.Produto.imagem + "\" alt=\"\" data-image-width=\"1280\" data-image-height=\"853\">\r\n" +
                            "                        <div class=\"texto\">\r\n" +
                            "                            <h4 class=\"u-text \">" + categoria.nome + "</h4>\r\n" +
                            "                            <h3 class=\"u-text \">" + produtoCampanha.Produto.nomeProduto + "</h3>\r\n" +
                            "                            <h3 class=\"u-text \">" + str + "</h3>\r\n" +
                            "                        </div>\r\n" +
                            "                    </div>\r\n" +
                            "                </div>");

					}
				}
				streamWriter.WriteLine("            </div>\r\n" +
                    "        </div>\r\n\r\n" +
                    "    </section>\r\n" +
                    "    <footer class=\"u-align-center u-clearfix u-footer u-grey-80 u-footer\" id=\"sec-8ad5\">\r\n" +
                    "        <div class=\"u-clearfix u-sheet u-sheet-1\">\r\n" +
                    "            <p class=\"u-small-text u-text u-text-variant u-text-1\">&copy;BIX desenvolvimentos.</p>\r\n" +
                    "        </div>\r\n" +
                    "    </footer>\r\n" +
                    "    <script type=\"text/javascript\" src=\"funcao.js\" defer=\"\"></script>\r\n</body>\r\n</html>");
			}
			return System.IO.File.Exists(targetPath + "/Index.html");
		}
		public void Layout1(Anuncio anuncio, ProdutoCampanha produto)
        {
            string diretorio = Directory.GetCurrentDirectory();
            Bitmap forma = new Bitmap(diretorio + "/wwwroot/imagens/forma.png");

            Bitmap bmp = new Bitmap(anuncio.largura, anuncio.altura);
            using (Graphics canvas = Graphics.FromImage(bmp))
            {

                // Define o retângulo que preencherá o canvas
                RectangleF rect = new RectangleF(0, 0, anuncio.largura, anuncio.altura);

                // Cria o degradê amarelo
                // Cria o gradiente radial
                LinearGradientBrush gradient = new LinearGradientBrush(
                    rect,
                    ColorTranslator.FromHtml("#efca4c"), // Cor amarela original (no centro)
                    ColorTranslator.FromHtml("#f7b322"), // Cor amarela mais clara (na borda)
                    LinearGradientMode.BackwardDiagonal);
                canvas.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var PosX = anuncio.largura * 0.03;
                var PosY = anuncio.altura * 0.85;
                canvas.FillRectangle(gradient, rect);
                System.Drawing.Font arial = new System.Drawing.Font("Arial", anuncio.fontProd, FontStyle.Bold);
                rect = new System.Drawing.Rectangle((int)PosX, (int)PosY, (int)(anuncio.largura - anuncio.largura * 0.02), (int)(anuncio.altura - anuncio.altura * 0.85));
                canvas.DrawString(produto.Produto.nomeProduto, arial, Brushes.DarkBlue, rect);
                System.Drawing.Rectangle Barra = new System.Drawing.Rectangle(0, 0, (int)(anuncio.largura * 0.02), anuncio.altura);
                SolidBrush cor = new SolidBrush(Color.DarkBlue);
                canvas.FillRectangle(cor, Barra);

                int itemWidth;
                int itemHeight;
                var tamWidht = (anuncio.largura / 2) * 0.55;
                var tamalt = (anuncio.altura / 2) * 0.55;
                if (anuncio.logo != null)
                {
                    itemWidth = (int)(tamalt * anuncio.logo.Width / ((float)anuncio.logo.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.logo.Height / anuncio.logo.Width;
                    }
                    PosX = anuncio.largura * 0.02; // Posição X da logo no centro
                    PosY = anuncio.altura * 0.02; // Posição Y da logo no centro inferior
                    canvas.DrawImage(anuncio.logo, (float)PosX, (float)PosY, itemWidth, itemHeight);
                }

                tamalt = (anuncio.altura / 1.4);
                itemWidth = (int)(tamalt * forma.Width / ((float)forma.Height));
                itemHeight = (int)tamalt;

                PosX = (anuncio.largura / 2) - itemWidth / 2; // Posição X da logo no centro
                PosY = (anuncio.altura * 0.1); // Posição Y da logo no centro inferior
                canvas.DrawImage(forma, (float)PosX, (float)PosY, itemWidth, itemHeight);

                if (anuncio.produto != null)
                {
                    tamWidht = itemWidth * 0.6;
                    tamalt = itemHeight * 0.6;
                    itemWidth = (int)(tamalt * anuncio.produto.Width / ((float)anuncio.produto.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.produto.Height / anuncio.produto.Width;
                    }
                    if (itemHeight >= itemWidth)
                    {
                        PosY = (anuncio.altura * 0.25); // Posição Y da logo no centro inferior
                    }
                    else
                    {
                        PosY = (anuncio.altura * 0.35);
                    }
                    PosX = (anuncio.largura - itemWidth) / 2; // Posição X da logo no centro
                    canvas.DrawImage(ResizeImage(anuncio.produto), (float)PosX, (float)PosY, itemWidth, itemHeight);
                }
                PosX = anuncio.largura * 0.64;
                PosY = anuncio.altura * 0.69;
                arial = new System.Drawing.Font("Arial", anuncio.fontValor, FontStyle.Bold);
                var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.valor);
                canvas.DrawString(valorFormatado, arial, Brushes.DarkBlue, (float)PosX, (float)PosY);

            }

            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            if (myImageCodecInfo != null)
            {
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                if (anuncio.logo != null)
                {
                    anuncio.logo.Dispose();
                }
                bmp.Save(anuncio.caminho, myImageCodecInfo, myEncoderParameters);
            }
            
        }
        public void Layout2(Anuncio anuncio, int fontAce, ProdutoCampanha produto, string link) 
        {
            string diretorio = Directory.GetCurrentDirectory();
            Bitmap forma = new Bitmap(diretorio + "/wwwroot/imagens/forma.png");
            //Bitmap QRcode = new Bitmap(@"C:\Users\Alexs\OneDrive\Área de Trabalho\testes\QRCode.png");
            Bitmap Seta = new Bitmap(diretorio + "/wwwroot/imagens/seta.png");


            Bitmap bmp = new Bitmap(anuncio.largura, anuncio.altura);
            using (Graphics canvas = Graphics.FromImage(bmp))
            {

                // Define o retângulo que preencherá o canvas
                RectangleF rect = new RectangleF(0, 0, anuncio.largura, anuncio.altura);

                // Cria o degradê amarelo
                // Cria o gradiente radial
                LinearGradientBrush gradient = new LinearGradientBrush(
                    rect,
                    ColorTranslator.FromHtml("#efca4c"), // Cor amarela original (no centro)
                    ColorTranslator.FromHtml("#f7b322"), // Cor amarela mais clara (na borda)
                    LinearGradientMode.BackwardDiagonal);
                canvas.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var PosX = anuncio.largura * 0.03;
                var PosY = anuncio.altura * 0.85;
                canvas.FillRectangle(gradient, rect);
                System.Drawing.Font arial = new System.Drawing.Font("Arial", anuncio.fontProd, FontStyle.Bold);
                rect = new System.Drawing.Rectangle((int)PosX, (int)PosY, (int)(anuncio.largura - anuncio.largura * 0.02), (int)(anuncio.altura - anuncio.altura * 0.85));
                canvas.DrawString(produto.Produto.nomeProduto, arial, Brushes.DarkBlue, rect);
                System.Drawing.Rectangle Barra = new System.Drawing.Rectangle(0, 0, (int)(anuncio.largura * 0.02), anuncio.altura);
                SolidBrush cor = new SolidBrush(Color.DarkBlue);
                canvas.FillRectangle(cor, Barra);

                int itemWidth;
                int itemHeight;
                var tamWidht = (anuncio.largura / 2) * 0.55;
                var tamalt = (anuncio.altura / 2) * 0.55;
                if (anuncio.logo != null)
                {
                    itemWidth = (int)(tamalt * anuncio.logo.Width / ((float)anuncio.logo.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.logo.Height / anuncio.logo.Width;
                    }

                    PosX = anuncio.largura * 0.03; // Posição X da logo no centro
                    PosY = anuncio.altura * 0.02; // Posição Y da logo no centro inferior
                    canvas.DrawImage(anuncio.logo, (float)PosX, (float)PosY, itemWidth, itemHeight);

                }

                tamalt = (anuncio.altura / 1.4);
                itemWidth = (int)(tamalt * forma.Width / ((float)forma.Height));
                itemHeight = (int)tamalt;

                PosX = (anuncio.largura * 0.3); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.1); // Posição Y da logo no centro inferior
                canvas.DrawImage(forma, (float)PosX, (float)PosY, itemWidth, itemHeight);

                if (anuncio.produto != null)
                {
                    tamWidht = itemWidth * 0.6;
                    tamalt = itemHeight * 0.6;
                    itemWidth = (int)(tamalt * anuncio.produto.Width / ((float)anuncio.produto.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.produto.Height / anuncio.produto.Width;
                    }
                    if (itemHeight >= itemWidth)
                    {
                        PosX = anuncio.largura * 0.38; // Posição X da logo no centro
                        PosY = (anuncio.altura * 0.25); // Posição Y da logo no centro inferior

                    }
                    else
                    {
                        PosX = anuncio.largura * 0.38; // Posição X da logo no centro
                        PosY = (anuncio.altura * 0.35); // Posição Y da logo no centro inferior

                    }
                    canvas.DrawImage(ResizeImage(anuncio.produto), (float)PosX, (float)PosY, itemWidth, itemHeight);
                }

                PosX = anuncio.largura * 0.03;
                PosY = anuncio.altura * 0.69;
                arial = new System.Drawing.Font("Arial", anuncio.fontValor, FontStyle.Bold);
                var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.valor);
                canvas.DrawString(valorFormatado, arial, Brushes.DarkBlue, (float)PosX, (float)PosY);

                tamalt = (anuncio.altura / 3);
                itemWidth = (int)(tamalt);
                itemHeight = (int)tamalt;

                PosX = (anuncio.largura * 0.73); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.18); // Posição Y da logo no centro inferior
                var qrcode = GenerateQRCode(link,itemWidth, itemHeight);

                canvas.DrawImage(qrcode, (float)PosX, (float)PosY, itemWidth, itemHeight);

                tamalt = (anuncio.altura / 9);
                itemWidth = (int)(tamalt * Seta.Width / ((float)Seta.Height));
                itemHeight = (int)tamalt;

                PosX = (anuncio.largura * 0.79); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.53); // Posição Y da logo no centro inferior
                canvas.DrawImage(Seta, (float)PosX, (float)PosY, itemWidth, itemHeight);

                PosX = (anuncio.largura * 0.77); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.66); // Posição Y da logo no centro inferior
                arial = new System.Drawing.Font("Arial", fontAce, FontStyle.Bold);
                canvas.DrawString("Acesse o\nCardápio", arial, Brushes.Black, (float)PosX, (float)PosY);

            }
            if (anuncio.logo != null)
            {
                anuncio.logo.Dispose();
            }
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(anuncio.caminho, myImageCodecInfo, myEncoderParameters);
        }
        public void Layout3(Anuncio anuncio, ProdutoCampanha produto)
        {
            string diretorio = Directory.GetCurrentDirectory();
            Bitmap forma = new Bitmap(diretorio + "/wwwroot/imagens/forma.png");
            Bitmap promo = new Bitmap(diretorio + "/wwwroot/imagens/promocao.png");

            Bitmap bmp = new Bitmap(anuncio.largura, anuncio.altura);
            using (Graphics canvas = Graphics.FromImage(bmp))
            {
                // Define o retângulo que preencherá o canvas
                RectangleF rect = new RectangleF(0, 0, anuncio.largura, anuncio.altura);

                // Cria o degradê amarelo
                // Cria o gradiente radial
                LinearGradientBrush gradient = new LinearGradientBrush(
                    rect,
                    ColorTranslator.FromHtml("#efca4c"), // Cor amarela original (no centro)
                    ColorTranslator.FromHtml("#f7b322"), // Cor amarela mais clara (na borda)
                    LinearGradientMode.BackwardDiagonal);
                canvas.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var PosX = anuncio.largura * 0.03;
                var PosY = anuncio.altura * 0.85;
                canvas.FillRectangle(gradient, rect);
                System.Drawing.Font arial = new System.Drawing.Font("Arial", anuncio.fontProd, FontStyle.Bold);
                rect = new System.Drawing.Rectangle((int)PosX, (int)PosY, (int)(anuncio.largura - anuncio.largura * 0.02), (int)(anuncio.altura - anuncio.altura * 0.85));
                canvas.DrawString(produto.Produto.nomeProduto, arial, Brushes.DarkBlue, rect);
                System.Drawing.Rectangle Barra = new System.Drawing.Rectangle(0, 0, (int)(anuncio.largura * 0.02), anuncio.altura);
                SolidBrush cor = new SolidBrush(Color.DarkBlue);
                canvas.FillRectangle(cor, Barra);

                var tamWidht = (anuncio.largura / 2) * 0.5;
                var tamalt = (anuncio.altura / 2) * 0.5;
                int itemWidth;
                int itemHeight;
                if (anuncio.logo != null)
                {
                    itemWidth = (int)(tamalt * anuncio.logo.Width / ((float)anuncio.logo.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.logo.Height / anuncio.logo.Width;
                    }

                    PosX = anuncio.largura * 0.02; // Posição X da logo no centro
                    PosY = anuncio.altura * 0.02; // Posição Y da logo no centro inferior
                    canvas.DrawImage(anuncio.logo, (float)PosX, (float)PosY, itemWidth, itemHeight);

                }

                tamalt = (anuncio.altura / 1.4);
                itemWidth = (int)(tamalt * forma.Width / ((float)forma.Height));
                itemHeight = (int)tamalt;

                PosX = (anuncio.largura * 0.25); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.1); // Posição Y da logo no centro inferior
                canvas.DrawImage(forma, (float)PosX, (float)PosY, itemWidth, itemHeight);

                if (anuncio.produto != null)
                {
                    tamWidht = itemWidth * 0.6;
                    tamalt = itemHeight * 0.6;
                    itemWidth = (int)(tamalt * anuncio.produto.Width / ((float)anuncio.produto.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * anuncio.produto.Height / anuncio.produto.Width;
                    }
                    if (itemHeight >= itemWidth)
                    {
                        PosY = (anuncio.altura * 0.25); // Posição Y da logo no centro inferior
                    }
                    else
                    {
                        PosY = (anuncio.altura * 0.35); // Posição Y da logo no centro inferior
                    }
                    PosX = anuncio.largura * 0.33; // Posição X da logo no centro
                    canvas.DrawImage(ResizeImage(anuncio.produto), (float)PosX, (float)PosY, itemWidth, itemHeight);

                }

                PosX = anuncio.largura * 0.67;
                PosY = anuncio.altura * 0.45;
                arial = new System.Drawing.Font("Arial", anuncio.fontValor, FontStyle.Bold);
                var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.valor);
                canvas.DrawString(valorFormatado, arial, Brushes.DarkBlue, (float)PosX, (float)PosY);

                tamWidht = (anuncio.altura / 1.6);
                itemHeight = (int)(tamWidht * promo.Height / ((float)promo.Width));
                itemWidth = (int)tamWidht;

                PosX = (anuncio.largura * 0.643); // Posição X da logo no centro
                PosY = (anuncio.altura * 0.56); // Posição Y da logo no centro inferior
                canvas.DrawImage(promo, (float)PosX, (float)PosY, itemWidth, itemHeight);

            }
            if (anuncio.logo != null)
            {
                anuncio.logo.Dispose();
            }
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(anuncio.caminho, myImageCodecInfo, myEncoderParameters);
        }
        public void Layout4(Anuncio anuncio, ProdutoCampanha produto)
        {
            int sizeWidth = anuncio.largura;
            int sizeHeight = anuncio.altura;
            string diretorio = Directory.GetCurrentDirectory();
            Bitmap product = anuncio.produto;
            Bitmap forma = new Bitmap(diretorio + "/wwwroot/imagens/forma.png");
            Bitmap logo = anuncio.logo;
            Bitmap promo = new Bitmap(diretorio + "/wwwroot/imagens/promocao.png");

            Bitmap bmp = new Bitmap(sizeWidth, sizeHeight);
            using (Graphics canvas = Graphics.FromImage(bmp))
            {
                // Define o retângulo que preencherá o canvas
                RectangleF rect = new RectangleF(0, 0, sizeWidth, sizeHeight);

                // Cria o degradê amarelo
                // Cria o gradiente radial
                LinearGradientBrush gradient = new LinearGradientBrush(
                    rect,
                    ColorTranslator.FromHtml("#efca4c"), // Cor amarela original (no centro)
                    ColorTranslator.FromHtml("#f7b322"), // Cor amarela mais clara (na borda)
                    LinearGradientMode.BackwardDiagonal);
                canvas.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var PosX = sizeWidth * 0.03;
                var PosY = sizeHeight * 0.85;
                canvas.FillRectangle(gradient, rect);
                FontFamily fontFamily = new FontFamily("Arial");
                System.Drawing.Font arial = new System.Drawing.Font(fontFamily, anuncio.fontProd, FontStyle.Bold);
                rect = new System.Drawing.Rectangle((int)PosX, (int)PosY, (int)(sizeWidth - sizeWidth * 0.02), (int)(sizeHeight - sizeHeight * 0.85));
                canvas.DrawString(produto.Produto.nomeProduto, arial, Brushes.DarkBlue, rect);
                System.Drawing.Rectangle Barra = new System.Drawing.Rectangle(0, 0, (int)(sizeWidth * 0.02), sizeHeight);
                SolidBrush cor = new SolidBrush(Color.DarkBlue);
                canvas.FillRectangle(cor, Barra);

                int itemWidth;
                int itemHeight;
                var tamWidht = (sizeWidth / 2) * 0.5;
                var tamalt = (sizeHeight / 2) * 0.5;
                if (anuncio.logo != null)
                {
                    itemWidth = (int)(tamalt * logo.Width / ((float)logo.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * logo.Height / logo.Width;
                    }

                    PosX = (sizeWidth * 0.99) - itemWidth; // Posição X da logo no centro
                    PosY = sizeHeight * 0.02; // Posição Y da logo no centro inferior
                    canvas.DrawImage(logo, (float)PosX, (float)PosY, itemWidth, itemHeight);

                }

                tamalt = (sizeHeight / 1.4);
                itemWidth = (int)(tamalt * forma.Width / ((float)forma.Height));
                itemHeight = (int)tamalt;

                PosX = (sizeWidth * 0.35); // Posição X da logo no centro
                PosY = (sizeHeight * 0.1); // Posição Y da logo no centro inferior
                canvas.DrawImage(forma, (float)PosX, (float)PosY, itemWidth, itemHeight);

                if (anuncio.produto != null)
                {
                    tamWidht = itemWidth * 0.6;
                    tamalt = itemHeight * 0.6;
                    itemWidth = (int)(tamalt * product.Width / ((float)product.Height));
                    itemHeight = (int)tamalt;
                    if (itemWidth > tamWidht)
                    {
                        itemWidth = (int)tamWidht;
                        itemHeight = (int)tamWidht * product.Height / product.Width;
                    }
                    if (itemHeight >= itemWidth)
                    {
                        PosY = (sizeHeight * 0.25);
                    }
                    else
                    {
                        PosY = (sizeHeight * 0.35); // Posição Y da logo no centro inferior

                    }
                    PosX = sizeWidth * 0.43; // Posição X da logo no centro
                    canvas.DrawImage(ResizeImage(product), (float)PosX, (float)PosY, itemWidth, itemHeight);

                }

                PosX = sizeWidth * 0.02;
                PosY = sizeHeight * 0.185;
                arial = new System.Drawing.Font(fontFamily, anuncio.fontValor, FontStyle.Bold);
                var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.valor);
                canvas.DrawString(valorFormatado, arial, Brushes.DarkBlue, (float)PosX, (float)PosY);

                tamWidht = (sizeHeight / 1.3);
                itemHeight = (int)(tamWidht * promo.Height / ((float)promo.Width));
                itemWidth = (int)tamWidht;

                PosX = (sizeWidth * 0.03); // Posição X da logo no centro
                PosY = (sizeHeight * 0.01); // Posição Y da logo no centro inferior
                canvas.DrawImage(promo, (float)PosX, (float)PosY, itemWidth, itemHeight);

            }
            if (anuncio.logo != null)
            {
                anuncio.logo.Dispose();
            }
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(anuncio.caminho, myImageCodecInfo, myEncoderParameters);
        }
        public string GerarPDF(Campanha campanha, List<ProdutoCampanha> produto, List<IFormFile> anuncios, int tipo, string diretorio )
        {
            List<string> listaImagens = new List<string>();
            var categorias= produto.OrderBy(s=>s.Produto.Categoria.nome).GroupBy(p => p.Produto.Categoria).ToList();
            
            
            if (anuncios.Count > 0 && anuncios != null)
            {
                foreach (var item in anuncios)
                {
                    if (item != null)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        string name = diretorio + "/Imagens/" + item.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            item.CopyToAsync(stream);
                        }
                        listaImagens.Add(name);
                    }
                }
            }
            if (System.IO.File.Exists(diretorio + "/CardapioPDF.pdf"))
            {
                System.IO.File.Delete(diretorio + "/CardapioPDF.pdf");

            }
            switch (tipo)
            {
                case 0:
                    int num = 0;
                    List<List<ProdutoCampanha>> produtoCampanhaListList = new List<List<ProdutoCampanha>>();
                    Document document = new Document();
                    PdfWriter.GetInstance(document,new FileStream(diretorio + "/CardapioPDF.pdf", FileMode.Create));
                    document.Open();
                    document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                    
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy((s => s.Produto.nomeProduto)).ToList().partition(8))
                        {
                            string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                            Cardapio(produtosCad, str, campanha.Usuariofilial.Filial, grupo);
                            if (System.IO.File.Exists(str))
                            {
                                Bitmap bitmap = new Bitmap(str);
                                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance((System.Drawing.Image)bitmap, ImageFormat.Bmp);
                                instance.ScaleToFit(document.PageSize);
                                instance.SetAbsolutePosition(0.0f, 0.0f);
                                document.Add(instance);
                                document.NewPage();
                                bitmap.Dispose();
                            }
                            ++num;
                        }
                    }
                    foreach (List<string> imagens in listaImagens.partition<string>(2))
                    {
                        string str =diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                        AnuncioCardapio(imagens, str, campanha.Usuariofilial.Filial, 1);
                        if (System.IO.File.Exists(str))
                        {
                            Bitmap bitmap = new Bitmap(str);
                            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance((System.Drawing.Image)bitmap, ImageFormat.Bmp);
                            instance.ScaleToFit(document.PageSize);
                            instance.SetAbsolutePosition(0.0f, 0.0f);
                            document.Add((IElement)instance);
                            document.NewPage();
                            bitmap.Dispose();
                        }
                        ++num;
                    }
                    document.Close();

                    break;
                case 1:
                    num = 0;
                    document = new Document();
                    PdfWriter.GetInstance(document,new FileStream(diretorio+ "/CardapioPDF.pdf", FileMode.Create));
                    document.Open();
                    document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where((s => s.Produto.Categoria == grupo)).OrderBy((s => s.Produto.nomeProduto)).ToList().partition(8))
                        {
                            string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                            Cardapio1(produtosCad, str, campanha.Usuariofilial.Filial, grupo);
                            if (System.IO.File.Exists(str))
                            {
                                Bitmap bitmap = new Bitmap(str);
                                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance((System.Drawing.Image)bitmap, ImageFormat.Bmp);
                                instance.ScaleToFit(document.PageSize);
                                instance.SetAbsolutePosition(0.0f, 0.0f);
                                document.Add(instance);
                                document.NewPage();
                                bitmap.Dispose();
                            }
                            ++num;
                        }
                    }
                    foreach (List<string> imagens in listaImagens.partition<string>(2))
                    {
                        string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                        AnuncioCardapio(imagens, str, campanha.Usuariofilial.Filial, 1);
                        if (System.IO.File.Exists(str))
                        {
                            Bitmap bitmap = new Bitmap(str);
                            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance((System.Drawing.Image)bitmap, ImageFormat.Bmp);
                            instance.ScaleToFit(document.PageSize);
                            instance.SetAbsolutePosition(0.0f, 0.0f);
                            document.Add(instance);
                            document.NewPage();
                            bitmap.Dispose();
                        }
                        ++num;
                    }
                    document.Close();
                    break;
                case 2:
                    List<List<string>> stringListList1 = listaImagens.partition<string>(2);
                    num = 0;
                    document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(diretorio + "/CardapioPDF.pdf", FileMode.Create));
                    document.Open();
                    document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(7))
                        {
                            string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                            Cardapio2(produtosCad, str, campanha.Usuariofilial.Filial, grupo);
                            if (System.IO.File.Exists(str))
                            {
                                Bitmap bitmap = new Bitmap(str);
                                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance((System.Drawing.Image)bitmap, ImageFormat.Bmp);
                                instance.ScaleToFit(document.PageSize);
                                instance.SetAbsolutePosition(0.0f, 0.0f);
                                document.Add(instance);
                                document.NewPage();
                                bitmap.Dispose();
                            }
                            ++num;
                        }
                    }
                    foreach (List<string> imagens in stringListList1)
                    {
                        string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                        this.AnuncioCardapio1(imagens, str, campanha.Usuariofilial.Filial);
                        if (System.IO.File.Exists(str))
                        {
                            Bitmap bitmap = new Bitmap(str);
                            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Bmp);
                            instance.ScaleToFit(document.PageSize);
                            instance.SetAbsolutePosition(0.0f, 0.0f);
                            document.Add((IElement)instance);
                            document.NewPage();
                            bitmap.Dispose();
                        }
                        ++num;
                    }
                    document.Close();
                    break;
                case 3:
                    List<List<string>> stringListList2 = listaImagens.partition(2);
                    num = 0;
                    document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(diretorio + "/CardapioPDF.pdf", FileMode.Create));
                    document.Open();
                    document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(6))
                        {
                            string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                            this.Cardapio3(produtosCad, str, campanha.Usuariofilial.Filial, grupo);
                            if (System.IO.File.Exists(str))
                            {
                                Bitmap bitmap = new Bitmap(str);
                                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Bmp);
                                instance.ScaleToFit(document.PageSize);
                                instance.SetAbsolutePosition(0.0f, 0.0f);
                                document.Add(instance);
                                document.NewPage();
                                bitmap.Dispose();
                            }
                            ++num;
                        }
                    }
                    foreach (List<string> imagens in stringListList2)
                    {
                        string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                        AnuncioCardapio(imagens, str, campanha.Usuariofilial.Filial, 3);
                        if (System.IO.File.Exists(str))
                        {
                            Bitmap bitmap = new Bitmap(str);
                            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Bmp);
                            instance.ScaleToFit(document.PageSize);
                            instance.SetAbsolutePosition(0.0f, 0.0f);
                            document.Add((IElement)instance);
                            document.NewPage();
                            bitmap.Dispose();
                        }
                        ++num;
                    }
                    document.Close();
                    break;
                case 4:
                    List<List<string>> stringListList3 =listaImagens.partition<string>(2);
                    num = 0;
                    document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(diretorio + "/CardapioPDF.pdf", FileMode.Create));
                    document.Open();
                    document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(6))
                        {
                            string str =diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                            this.Cardapio4(produtosCad, str, campanha.Usuariofilial.Filial, grupo);
                            if (System.IO.File.Exists(str))
                            {
                                Bitmap bitmap = new Bitmap(str);
                                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Bmp);
                                instance.ScaleToFit(document.PageSize);
                                instance.SetAbsolutePosition(0.0f, 0.0f);
                                document.Add(instance);
                                document.NewPage();
                                bitmap.Dispose();
                            }
                            ++num;
                        }
                    }
                    foreach (List<string> imagens in stringListList3)
                    {
                        string str = diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg";
                        this.AnuncioCardapio1(imagens, str, campanha.Usuariofilial.Filial);
                        if (System.IO.File.Exists(str))
                        {
                            Bitmap bitmap = new Bitmap(str);
                            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Bmp);
                            instance.ScaleToFit(document.PageSize);
                            instance.SetAbsolutePosition(0.0f, 0.0f);
                            document.Add(instance);
                            document.NewPage();
                            bitmap.Dispose();
                        }
                        ++num;
                    }
                    document.Close();
                    break;

            }
            Directory.Delete(diretorio + "/Imagens", true);
            var pathToTheFile = Path.Combine( "Empresas", campanha.codFilial.ToString(), "CardapioPDF.pdf"); // exemplo de caminho
            return pathToTheFile;
        }
        public string GerarIMG(Campanha campanha, List<ProdutoCampanha> produto, List<IFormFile> anuncios, int tipo, string diretorio) 
        {
            var categorias = produto.OrderBy(s => s.Produto.Categoria.nome).GroupBy(p => p.Produto.Categoria).ToList(); 
            List<string> values = new List<string>();
            try
            {
                if (!Directory.Exists(diretorio + "/Imagens"))
                {
                    Directory.CreateDirectory(diretorio + "/Imagens");
                }
                
                if (anuncios.Count > 0)
                {
                    if (anuncios != null)
                    {
                        foreach (IFormFile anuncio in anuncios)
                        {
                            if (anuncio != null)
                            {
                                Path.GetFileName(anuncio.FileName);
                                string path = diretorio + "/Imagens/" + anuncio.FileName;
                                using (FileStream target = new FileStream(path, FileMode.Create))
                                    anuncio.CopyToAsync((Stream)target);
                                values.Add(path);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar GerarIMG: {ex}");
            }
            List<List<ProdutoCampanha>> produtoCampanhaListList;
            switch (tipo)
            {
                case 0:
                    produtoCampanhaListList = new List<List<ProdutoCampanha>>();
                    int num = 0;
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(8))
                        {
                            Cardapio(produtosCad, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, grupo);
                            ++num;
                        }
                    }
                    using (List<List<string>>.Enumerator enumerator = values.partition(2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            AnuncioCardapio(enumerator.Current, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, 0);
                            ++num;
                        }
                        break;
                    }
                case 1:
                    num = 0;
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(8))
                        {
                            Cardapio1(produtosCad, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, grupo);
                            ++num;
                        }
                    }
                    using (List<List<string>>.Enumerator enumerator = values.partition(2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            AnuncioCardapio(enumerator.Current, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, 0);
                            ++num;
                        }
                        break;
                    }
                case 2:
                    num = 0;
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(7))
                        {
                            Cardapio2(produtosCad, diretorio + "/Imagens/Cardapio" + diretorio.ToString() + ".jpg", campanha.Usuariofilial.Filial, grupo);
                            ++num;
                        }
                    }
                    using (List<List<string>>.Enumerator enumerator = values.partition(2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            AnuncioCardapio1(enumerator.Current, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial);
                            ++num;
                        }
                        break;
                    }
                case 3:
                    num = 0;
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(6))
                        {
                            Cardapio3(produtosCad, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, grupo);
                            ++num;
                        }
                    }
                    using (List<List<string>>.Enumerator enumerator = values.partition(2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            AnuncioCardapio(enumerator.Current, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, 3);
                            ++num;
                        break;
                    }
                case 4:
                    num = 0;
                    foreach (var grupos in categorias)
                    {
                        var grupo = grupos.Key;
                        foreach (List<ProdutoCampanha> produtosCad in produto.Where(s => s.Produto.Categoria == grupo).OrderBy(s => s.Produto.nomeProduto).ToList().partition(6))
                        {
                            Cardapio4(produtosCad, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial, grupo);
                            ++num;
                        }
                    }
                    using (List<List<string>>.Enumerator enumerator = values.partition(2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            AnuncioCardapio1(enumerator.Current, diretorio + "/Imagens/Cardapio" + num.ToString() + ".jpg", campanha.Usuariofilial.Filial);
                            ++num;
                        break;
                    }
            }
            if (values != null)
            {
                foreach (string path in values)
                    System.IO.File.Delete(path);
            }
            if (System.IO.File.Exists(diretorio + "/Cardapio.zip"))
            {
                System.IO.File.Delete(diretorio + "/Cardapio.zip");
                ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/Cardapio.zip");
            }
            else
            {
                ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/Cardapio.zip");
            }
            Directory.Delete(diretorio + "/Imagens", true);
            string path1 = Path.Combine("Empresas", campanha.codFilial.ToString(), "Cardapio.zip");
            return path1;
        }
        public void AnuncioCardapio(List<string> imagens, string caminho, Filial filial, int Id)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            Bitmap bitmap1 = new Bitmap(currentDirectory + "/wwwroot/imagens/cardapiofundo.png");
            if (Id == 1)
                bitmap1 = new Bitmap(currentDirectory + "/wwwroot/imagens/cardapio1.png");
            Graphics graphics = Graphics.FromImage(bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) / bitmap2.Height);
                    float height = num2;
                    if (width > num1)
                    {
                        width = num1;
                        height = (int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage(bitmap2,x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar AnuncioCardapio: {ex}");
            }
            float height1 = 500f;
            int num3 = 1200;
            int num4 = 620;
            int y1 = 500;
            foreach (string imagen in imagens)
            {
                System.Drawing.Image image =new Bitmap(imagen);
                float width = (int)((num4 * image.Width) / image.Height);
                height1 = num4;
                if (width > num3)
                {
                    width = num3;
                    height1 = (int)((num3 * image.Height) / image.Width);
                }
                int x = (int)((bitmap1.Width / 2) -width / 2.0);
                graphics.DrawImage(image,x, y1, width, height1);
                y1 = (int)(height1 + y1 + 30.0);
                image.Dispose();
            }
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            int x1 = (int)(bitmap1.Width * 0.07);
            float num5 = (int)(bitmap1.Width * 0.9);
            Brush brush =new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.White));
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Italic);
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, 2005, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font1, brush, layoutRectangle);
            string s = filial.Endereco.logradouro+ ", " + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            System.Drawing.Font font2 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x1 + 5, 2085, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(s, font2, brush, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle(920, 2070, 560, (int)height1);
            System.Drawing.Font font3 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font3, brush, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1220, 2015, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font4 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font4, brush,layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void AnuncioCardapio1(List<string> imagens, string caminho, Filial filial)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapiofundo1.png");
            Graphics graphics = Graphics.FromImage(bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (System.IO.File.Exists(filial.logoHome))
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) /bitmap2.Height);
                    float height = num2;
                    if (width > num1)
                    {
                        width = num1;
                        height =(int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage(bitmap2, x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar AuncioCardapio2: {ex}");
            }
            int num3 = 1200;
            int num4 = 580;
            int y1 = 520;
            foreach (string imagen in imagens)
            {
                System.Drawing.Image image = new Bitmap(imagen);
                float width = (int)((num4 * image.Width) / image.Height);
                float height = num4;
                if (width > num3)
                {
                    width = num3;
                    height = (int)((num3 * image.Height) / image.Width);
                }
                int x = (int)((bitmap1.Width / 2) - width / 2.0);
                graphics.DrawImage(image, x, y1, width, height);
                y1 = (int)(height +y1 + 30.0);
                image.Dispose();
            }
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            int x1 = (int)(bitmap1.Width * 0.07);
            float num5 = (int)(bitmap1.Width * 0.9);
            Brush brush = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.White));
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Italic);
            float height1 = 580f;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, 2005, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font1, brush, layoutRectangle);
            string s = filial.Endereco.logradouro + ", " + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            System.Drawing.Font font2 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x1 + 5, 2085, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(s, font2, brush, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle((int)(x1 + num5 * 0.6), 2065, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font3 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font3, brush, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1270, 2015, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font4 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font4, brush, layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void Cardapio(List<ProdutoCampanha> produtosCad, string caminho, Filial filial, Categoria categoria)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapiofundo.png");
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) / (double)bitmap2.Height);
                    float height = num2;
                    if ((double)width > num1)
                    {
                        width = (float)num1;
                        height = (float)(int)((double)(num1 * bitmap2.Height) / (double)bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage((System.Drawing.Image)bitmap2, x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            Brush brush1 = new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(System.Drawing.Color.Black, 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            float width1 = (int)(bitmap1.Width * 0.9);
            float height1 = 100f;
            int x1 = (int)(bitmap1.Width * 0.05);
            int y1 = 500;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)width1, (int)height1);
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 15f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)((double)width1 * 0.6), 100);
            graphics.DrawString(categoria.nome, font1, brush1, (RectangleF)layoutRectangle);
            graphics.DrawLine(pen, x1 + 10, y1 + 70, (int)(x1 + (double)width1 * 0.95), y1 + 70);
            int y2 = y1 + 120;
            int x2 = (int)(bitmap1.Width * 0.07);
            foreach (ProdutoCampanha produtoCampanha in produtosCad)
            {
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 9f, FontStyle.Bold);
                layoutRectangle = new System.Drawing.Rectangle(x2, y2, (int)((double)width1 * 0.6), 100);
                graphics.DrawString(produtoCampanha.Produto.nomeProduto, font2, brush1, (RectangleF)layoutRectangle);
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 6f, FontStyle.Regular);
                if (produtoCampanha.Produto.descricaoDetalhada.Length > 37)
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 85, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, (RectangleF)layoutRectangle);
                }
                else
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 45, (int)((double)width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, (RectangleF)layoutRectangle);
                }
                int y3 = y2 + 15;
                layoutRectangle = new System.Drawing.Rectangle((int)((double)x2 + (double)width1 * 0.6), y3, (int)(width1 * 0.3), (int)height1);
                System.Drawing.Font font4 = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
                string s = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
                graphics.DrawString(s, font4, brush1, (RectangleF)layoutRectangle, format);
                int num3 = y3 + 35;
                graphics.DrawLine(pen, (int)(x2 + width1 * 0.5), num3, (int)(x2 + width1 * 0.7), num3);
                y2 = num3 + 75;
            }
            Brush brush2 = new SolidBrush(System.Drawing.Color.White);
            System.Drawing.Font font5 = new System.Drawing.Font("Arial", 10f, FontStyle.Italic);
            layoutRectangle = new System.Drawing.Rectangle(x2, 2005, (int)(width1 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font5, brush2, (RectangleF)layoutRectangle);
            System.Drawing.Font font6 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x2 + 5, 2085, (int)(width1 * 0.7), (int)height1);
            string s1 = filial.Endereco.logradouro + "," + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            graphics.DrawString(s1, font6, brush2, (RectangleF)layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.6), 2060, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font7 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font7, brush2, (RectangleF)layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1245, 2015, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font8 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font8, brush2, (RectangleF)layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void Cardapio1(List<ProdutoCampanha> produtosCad,string caminho,Filial filial,Categoria grupo)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapio1.png");
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) / bitmap2.Height);
                    float height = num2;
                    if ((double)width >num1)
                    {
                        width = num1;
                        height = (int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage((System.Drawing.Image)bitmap2, x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao Funcão cardapio1: {ex}");
            }
            Brush brush1 = new SolidBrush(System.Drawing.Color.Yellow);
            Pen pen = new Pen(System.Drawing.Color.Yellow, 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            float width1 = (int)(bitmap1.Width * 0.9);
            float height1 = 100f;
            int x1 = (int)(bitmap1.Width * 0.05);
            int y1 = 550;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)width1, 100);
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Bold);
            graphics.DrawString(grupo.nome, font1, brush1, layoutRectangle);
            graphics.DrawLine(pen, x1 + 10, y1 + 70, (int)(x1 + width1 * 0.95), y1 + 70);
            int y2 = y1 + 120;
            int x2 = (int)(bitmap1.Width * 0.07);
            foreach (ProdutoCampanha produtoCampanha in produtosCad)
            {
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
                layoutRectangle = new System.Drawing.Rectangle(x2, y2, (int)(width1 * 0.7), 100);
                graphics.DrawString(produtoCampanha.Produto.nomeProduto, font2, brush1, layoutRectangle);
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 6f, FontStyle.Regular);
                if (produtoCampanha.Produto.descricaoDetalhada.Length > 37)
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 90, (int)((double)width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, layoutRectangle);
                }
                else
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 45, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, layoutRectangle);
                }
                int y3 = y2 + 15;
                layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.6), y3, (int)(width1 * 0.3), (int)height1);
                System.Drawing.Font font4 = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
                string s = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
                graphics.DrawString(s, font4, brush1, (RectangleF)layoutRectangle, format);
                int num3 = y3 + 35;
                graphics.DrawLine(pen, (int)(x2 +width1 * 0.6), num3, (int)(x2 + width1 * 0.7), num3);
                y2 = num3 + 100;
            }
            Brush brush2 = new SolidBrush(System.Drawing.Color.White);
            System.Drawing.Font font5 = new System.Drawing.Font("Arial", 10f, FontStyle.Italic);
            layoutRectangle = new System.Drawing.Rectangle(x2, 2005, (int)(width1 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font5, brush2, layoutRectangle);
            System.Drawing.Font font6 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x2 + 5, 2085, (int)(width1 * 0.7), (int)height1);
            string s1 = filial.Endereco.logradouro + "," + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            graphics.DrawString(s1, font6, brush2, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle((int)(x2 +width1 * 0.6), 2070, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font7 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font7, brush2, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1245, 2015, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font8 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font8, brush2,layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void Cardapio2(List<ProdutoCampanha> produtosCad,string caminho,Filial filial,Categoria grupo)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapiofundo1.png");
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) /bitmap2.Height);
                    float height = num2;
                    if (width > num1)
                    {
                        width = num1;
                        height = (int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage((System.Drawing.Image)bitmap2, x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Cardapio2: {ex}");
            }
            float width1 = (int)(bitmap1.Width * 0.8);
            float height1 = 150f;
            int x1 = (int)(bitmap1.Width * 0.11);
            int num3 = (int)(bitmap1.Height * 0.03);
            Brush brush1 = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Yellow));
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Bold);
            int y1 = 540;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)width1, (int)height1);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            Pen pen = new Pen(System.Drawing.Color.FromKnownColor(KnownColor.Yellow), 2f);
            graphics.DrawString(grupo.nome, font1, brush1, layoutRectangle);
            graphics.DrawLine(pen, x1 + 10, y1 + 70, (int)(x1 +width1 * 0.95), y1 + 70);
            int y2 = y1 + 120;
            int x2 = (int)(bitmap1.Width * 0.125);
            foreach (ProdutoCampanha produtoCampanha in produtosCad)
            {
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
                layoutRectangle = new System.Drawing.Rectangle(x2, y2, (int)((double)width1 * 0.6), 100);
                graphics.DrawString(produtoCampanha.Produto.nomeProduto, font2, brush1, layoutRectangle);
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 7f, FontStyle.Regular);
                if (produtoCampanha.Produto.descricaoDetalhada.Length > 34)
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 100, (int)(width1 * 0.65), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1,layoutRectangle);
                }
                else
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2, y2 + 55, (int)((double)width1 * 0.65), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, layoutRectangle);
                }
                int y3 = y2 + 15;
                layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.6), y3, (int)(width1 * 0.3), (int)height1);
                System.Drawing.Font font4 = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
                string s = string.Format((IFormatProvider)CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
                graphics.DrawString(s, font4, brush1, (RectangleF)layoutRectangle, format);
                int num4 = y3 + 23;
                graphics.DrawLine(pen, (int)(x2 +width1 * 0.55), num4, (int)(x2 + width1 * 0.68), num4);
                y2 = num4 + 110;
            }
            int x3 = (int)(bitmap1.Width * 0.07);
            float num5 = (int)(bitmap1.Width * 0.9);
            Brush brush2 = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.White));
            System.Drawing.Font font5 = new System.Drawing.Font("Arial", 10f, FontStyle.Italic);
            layoutRectangle = new System.Drawing.Rectangle(x3, 2005, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font5, brush2,layoutRectangle);
            string s1 = filial.Endereco.logradouro + ", " + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            System.Drawing.Font font6 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x3 + 5, 2085, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(s1, font6, brush2, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle((int)(x3 +num5 * 0.6), 2065, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font7 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font7, brush2, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1270, 2015, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font8 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font8, brush2, layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void Cardapio3(List<ProdutoCampanha> produtosCad,string caminho, Filial filial,Categoria grupo)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapiofundo.png");
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = (int)((num2 * bitmap2.Width) /bitmap2.Height);
                    float height = num2;
                    if (width > num1)
                    {
                        width = num1;
                        height = (int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage((System.Drawing.Image)bitmap2,x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Cardapio3: {ex}");
            }
            Brush brush1 = (Brush)new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(System.Drawing.Color.Black, 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            float width1 = (int)(bitmap1.Width * 0.9);
            float height1 = 150f;
            int x1 = 78;
            int y1 = 520;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)width1, (int)height1);
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Bold);
            graphics.DrawString(grupo.nome, font1, brush1, layoutRectangle);
            graphics.DrawLine(pen, x1 + 10, y1 + 70, (int)(x1 + width1 * 0.95), y1 + 70);
            int y2 = y1 + 120;
            int x2 = (int)(bitmap1.Width * 0.1);
            foreach (ProdutoCampanha produtoCampanha in produtosCad)
            {
                if (produtoCampanha.Produto.imagem != "")
                {
                    Bitmap imagem = new Bitmap(new WebClient().OpenRead(produtoCampanha.Produto.imagem));
                    graphics.DrawImage(ImagemCircular(imagem, arrendodar()), x2, y2, 135, 135);
                }
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
                layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 30, (int)(width1 * 0.5), 100);
                graphics.DrawString(produtoCampanha.Produto.nomeProduto, font2, brush1, layoutRectangle);
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 6f, FontStyle.Regular);
                if (produtoCampanha.Produto.descricaoDetalhada.Length > 26)
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 130, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1,layoutRectangle);
                }
                else
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 85, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada+ ")", font3, brush1, layoutRectangle);
                }
                int y3 = y2 + 45;
                layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.55), y3, (int)(width1 * 0.3), (int)height1);
                System.Drawing.Font font4 = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
                string s = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}",produtoCampanha.valor);
                graphics.DrawString(s, font4, brush1, layoutRectangle, format);
                int num3 = y3 + 35;
                graphics.DrawLine(pen, (int)(x2 + width1 * 0.5), num3, (int)(x2 + width1 * 0.68), num3);
                y2 = num3 + 115;
            }
            Brush brush2 = new SolidBrush(System.Drawing.Color.White);
            System.Drawing.Font font5 = new System.Drawing.Font("Arial", 10f, FontStyle.Italic);
            layoutRectangle = new System.Drawing.Rectangle(x2, 2005, (int)(width1 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font5, brush2, layoutRectangle);
            System.Drawing.Font font6 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x2 + 5, 2085, (int)(width1 * 0.7), (int)height1);
            string s1 = filial.Endereco.logradouro + ", " + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            graphics.DrawString(s1, font6, brush2, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.6), 2060, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font7 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font7, brush2, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1245, 2015, (int)(width1 * 0.4), (int)height1);
            System.Drawing.Font font8 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font8, brush2, layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        public void Cardapio4(List<ProdutoCampanha> produtosCad,string caminho,Filial filial,Categoria grupo)
        {
            Bitmap bitmap1 = new Bitmap(Directory.GetCurrentDirectory() + "/wwwroot/imagens/cardapiofundo1.png");
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int num1 = 600;
            int num2 = 350;
            try
            {
                if (filial.logoHome != "")
                {
                    Bitmap bitmap2 = new Bitmap(new WebClient().OpenRead(filial.logoHome));
                    float width = ((num2 * bitmap2.Width) / bitmap2.Height);
                    float height = num2;
                    if (width > num1)
                    {
                        width = num1;
                        height = (int)((num1 * bitmap2.Height) / bitmap2.Width);
                    }
                    int x = (int)((bitmap1.Width / 2) - width / 2.0);
                    int y = (int)(bitmap1.Height * 0.03);
                    graphics.DrawImage((System.Drawing.Image)bitmap2, x, y, width, height);
                    bitmap2.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Cardapio4: {ex}");
            }
            int num3 = (int)(bitmap1.Height * 0.03);
            float width1 = (int)(bitmap1.Width * 0.8);
            float height1 = 150f;
            int x1 = (int)(bitmap1.Width * 0.12);
            Brush brush1 = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Yellow));
            System.Drawing.Font font1 = new System.Drawing.Font("Arial", 14f, FontStyle.Bold);
            int y1 = 555;
            System.Drawing.Rectangle layoutRectangle = new System.Drawing.Rectangle(x1, y1, (int)width1, (int)height1);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            Pen pen = new Pen(System.Drawing.Color.FromKnownColor(KnownColor.Yellow), 2f);
            graphics.DrawString(grupo.nome, font1, brush1, layoutRectangle);
            graphics.DrawLine(pen, x1 + 10, y1 + 70, (int)(x1 +width1 * 0.95), y1 + 70);
            int y2 = y1 + 120;
            int x2 = (int)(bitmap1.Width * 0.14);
            foreach (ProdutoCampanha produtoCampanha in produtosCad)
            {
                if (produtoCampanha.Produto.imagem != "")
                {
                    Bitmap imagem = new Bitmap(new WebClient().OpenRead(produtoCampanha.Produto.imagem));
                    graphics.DrawImage(ImagemCircular(imagem, arrendodar()), x2, y2, 140, 140);
                }
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
                layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 30, (int)((double)width1 * 0.5), 100);
                graphics.DrawString(produtoCampanha.Produto.nomeProduto, font2, brush1, layoutRectangle);
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 6f, FontStyle.Regular);
                if (produtoCampanha.Produto.descricaoDetalhada.Length > 26)
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 130, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1, layoutRectangle);
                }
                else
                {
                    layoutRectangle = new System.Drawing.Rectangle(x2 + 190, y2 + 85, (int)(width1 * 0.5), (int)height1 / 2);
                    graphics.DrawString("(" + produtoCampanha.Produto.descricaoDetalhada + ")", font3, brush1,layoutRectangle);
                }
                int y3 = y2 + 45;
                layoutRectangle = new System.Drawing.Rectangle((int)(x2 + width1 * 0.65), y3, (int)(width1 * 0.25), (int)height1);
                System.Drawing.Font font4 = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
                string s = string.Format((IFormatProvider)CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCampanha.valor);
                graphics.DrawString(s, font4, brush1, layoutRectangle, format);
                int num4 = y3 + 35;
                graphics.DrawLine(pen, (int)(x2 + width1 * 0.62), num4, (int)(x2 +width1 * 0.72), num4);
                y2 = num4 + 110;
            }
            int x3 = (int)(bitmap1.Width * 0.07);
            float num5 = (int)(bitmap1.Width * 0.9);
            Brush brush2 = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.White));
            System.Drawing.Font font5 = new System.Drawing.Font("Arial", 10f, FontStyle.Italic);
            layoutRectangle = new System.Drawing.Rectangle(x3, 2005, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(filial.nome, font5, brush2, layoutRectangle);
            string s1 = filial.Endereco.logradouro + ", " + filial.Endereco.numeroendereco.ToString() + " - " + filial.Endereco.bairro + ", " + filial.Endereco.cidade;
            System.Drawing.Font font6 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            layoutRectangle = new System.Drawing.Rectangle(x3 + 5, 2085, (int)(num5 * 0.7), (int)height1);
            graphics.DrawString(s1, font6, brush2, layoutRectangle);
            layoutRectangle = new System.Drawing.Rectangle(920, 2070, 560, (int)height1);
            System.Drawing.Font font7 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
            graphics.DrawString(filial.telefone, font7, brush2, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            layoutRectangle = new System.Drawing.Rectangle(1220, 2015, (int)(num5 * 0.4), (int)height1);
            System.Drawing.Font font8 = new System.Drawing.Font("Arial", 8f, FontStyle.Bold);
            graphics.DrawString("Contanto:", font8, brush2, layoutRectangle, format);
            bitmap1.Save(caminho);
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(codec => codec.MimeType == mimeType);
        }
        public Bitmap arrendodar()
        {
            Bitmap bitmap = new Bitmap(500, 500);
            //bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Blue);
            Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.White));
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, 500, 500);
            graphics.FillEllipse(brush, rectangle);
            graphics.Dispose();
            return bitmap;
        }
        public Bitmap ImagemCircular(Bitmap imagem, Bitmap circulo)
        {

            Bitmap ImageFinal = new Bitmap(500, 500);
            ImageFinal.MakeTransparent();
            Graphics graphics = Graphics.FromImage(ImageFinal);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.Clear(Color.White);
            var altura = circulo.Height;
            var largura = (int)(altura * imagem.Width / ((float)imagem.Height));
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imagem, 0, 0, altura, largura);

            for (int x = 0; x < circulo.Width; x++)
            {
                for (int y = 0; y < circulo.Height; y++)
                {

                    Color pixelColor = circulo.GetPixel(x, y);
                    if (pixelColor.Name == "ff0000ff")
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }

                }
            }
            return ImageFinal;
        }
        public Bitmap GenerateQRCode(string text, int width, int height)
        {
            var qrCodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 1
                }
            };

            var pixelData = qrCodeWriter.Write(text);

            // Cria o bitmap a partir dos dados de pixel
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    // Copia os dados do pixel para o bitmap
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                return new Bitmap(bitmap);
            }
        }
        public bool AtivarContaCliente(Usuario usuario, UsuarioFilial usuarioFilial) 
        {
            try
            {
				
				StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
                    "<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"pt\">\r\n" +
                    " <head>\r\n" +
                    "  <meta charset=\"UTF-8\">\r\n" +
                    "  <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n" +
                    "  <meta name=\"x-apple-disable-message-reformatting\">\r\n" +
                    "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n" +
                    "  <meta content=\"telephone=no\" name=\"format-detection\">\r\n" +
                    "  <title>Confirmação de Cadastro</title>\r\n" +
                    "  <link href=\"https://fonts.googleapis.com/css?family=Lato:400,400i,700,700i\" rel=\"stylesheet\"><!--<![endif]-->\r\n" +
                    "  <style type=\"text/css\">\r\n" +
                    "#outlook a {\r\n" +
                    "\tpadding:0;\r\n" +
                    "}\r\n" +
                    ".gradiente {\r\n" +
                    "\tbackground-color:#2cb543;\r\n" +
                    "\tcolor:white!important;\r\n" +
                    "}\r\n.es-button {\r\n" +
                    "\tmso-style-priority:100!important;\r\n" +
                    "\ttext-decoration:none!important;\r\n" +
                    "}\r\na[x-apple-data-detectors] {\r\n" +
                    "\tcolor:inherit!important;\r\n" +
                    "\ttext-decoration:none!important;\r\n\tfont-size:inherit!important;\r\n" +
                    "\tfont-family:inherit!important;\r\n" +
                    "\tfont-weight:inherit!important;\r\n" +
                    "\tline-height:inherit!important;\r\n" +
                    "}\r\n" +
                    ".es-desk-hidden {\r\n" +
                    "\tdisplay:none;\r\n" +
                    "\tfloat:left;\r\n" +
                    "\toverflow:hidden;\r\n" +
                    "\twidth:0;\r\n" +
                    "\tmax-height:0;\r\n" +
                    "\tline-height:0;\r\n" +
                    "\tmso-hide:all;\r\n" +
                    "}\r\n.box-shadow {\r\n" +
                    "\tbox-shadow:rgba(0, 0, 0, 0.24) 0px 3px 8px;\r\n" +
                    "\tmargin:15px 20px;\r\n" +
                    "\tpadding:10px 20px;\r\n" +
                    "\tborder-radius:10px;\r\n" +
                    "\tcolor:white !IMPORTANT;\r\n" +
                    "}\r\n" +
                    "@media only screen and (max-width:600px) {p, ul li, ol li, a { line-height:150%!important } h1, h2, h3, h1 a, h2 a, h3 a { line-height:120%!important } h1 { font-size:30px!important; text-align:left } h2 { font-size:24px!important; text-align:left } h3 { font-size:20px!important; text-align:left } .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:30px!important; text-align:left } .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:24px!important; text-align:left } .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px!important; text-align:left } .es-menu td a { font-size:14px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:14px!important } .es-content-body p, .es-content-body ul li, .es-content-body ol li, .es-content-body a { font-size:14px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:14px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=\"gmail-fix\"] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:inline-block!important } a.es-button, button.es-button { font-size:18px!important; display:inline-block!important } .es-adaptive table, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important } .es-m-p0 { padding:0px!important } .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } tr.es-desk-hidden, td.es-desk-hidden, table.es-desk-hidden { width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } tr.es-desk-hidden { display:table-row!important } table.es-desk-hidden { display:table!important } td.es-desk-menu-hidden { display:table-cell!important } .es-menu td { width:1%!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important } .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; max-height:inherit!important } .h-auto { height:auto!important } }\r\n@media screen and (max-width:384px) {.mail-message-content { width:414px!important } }\r\n" +
                    "</style>\r\n" +
                    " </head>");
                stringBuilder.Append(" <body style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n" +
                    "  <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"pt\" style=\"background-color:#EFEFEF\"><!--[if gte mso 9]>\r\n" +
                    "\t\t\t<v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">\r\n" +
                    "\t\t\t\t<v:fill type=\"tile\" color=\"#efefef\"></v:fill>\r\n" +
                    "\t\t\t</v:background>\r\n" +
                    "\t\t<![endif]-->\r\n" +
                    "   <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#EFEFEF\">\r\n" +
                    "     <tr>\r\n" +
                    "      <td valign=\"top\" style=\"padding:0;Margin:0\">\r\n" +
                    "       <table class=\"es-header\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
                    "         <tr>\r\n" +
                    "          <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                    "           <table class=\"es-header-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                    "             <tr>\r\n" +
                    "              <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
                    "               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                 <tr>\r\n" +
                    "                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                    "                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                     <tr>\r\n" +
                    "                      <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_fe0b9ecd8c68b601d31ed85906c6b76cdce45f3da681f46277bca21db3847f13/images/logo2.png\" alt=\"\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"300\"></td>\r\n" +
                    "                     </tr>\r\n" +
                    "                   </table></td>\r\n" +
                    "                 </tr>\r\n" +
                    "               </table></td>\r\n" +
                    "             </tr>\r\n" +
                    "           </table></td>\r\n" +
                    "         </tr>\r\n" +
                    "       </table>\r\n" +
                    "       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
                    "         <tr>\r\n" +
                    "          <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                    "           <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#efefef;width:600px\" role=\"none\">\r\n" +
                    "             <tr>\r\n" +
                    "              <td align=\"left\" style=\"padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px\"><!--[if mso]><table style=\"width:560px\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"width:370px\" valign=\"top\"><![endif]-->\r\n" +
                    "               <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                    "                 <tr>\r\n" +
                    "                  <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:370px\">\r\n" +
                    "                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                     <tr>\r\n" +
                    "                      <td align=\"left\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\"><b>Olá, agradecemos o cadastro em nosso site "+usuario.nome+". Para usar nossos serviços pedimos que acesse o link abaixo para confirmar o cadastro!</b></p></td>\r\n" +
                    "                     </tr>\r\n" +
                    "                   </table></td>\r\n" +
                    "                 </tr>\r\n" +
                    "               </table><!--[if mso]></td><td style=\"width:20px\"></td><td style=\"width:170px\" valign=\"top\"><![endif]-->\r\n" +
                    "               <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
                    "                 <tr>\r\n" +
                    "                  <td align=\"left\" style=\"padding:0;Margin:0;width:170px\">\r\n" +
                    "                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                     <tr>\r\n" +
                    "                      <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_816833e5dfa0c333b402c7e232f123128db71d8b90530f4eab622b2230ebe354/images/sem_titulo_V0a.png\" alt style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"135\"></td>\r\n" +
                    "                     </tr>\r\n" +
                    "                   </table></td>\r\n" +
                    "                 </tr>\r\n" +
                    "               </table><!--[if mso]></td></tr></table><![endif]--></td>\r\n" +
                    "             </tr>\r\n" +
                    "           </table></td>\r\n" +
                    "         </tr>\r\n" +
                    "       </table>\r\n" +
                    "       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
                    "         <tr>\r\n" +
                    "          <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                    "           <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                    "             <tr>\r\n" +
                    "              <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-bottom:10px;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
                    "               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                 <tr>\r\n" +
                    "                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                    "                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                     <tr>\r\n" +
                    "                      <td align=\"center\" style=\"padding:0;Margin:0\"><!--[if mso]><a href=\"\" target=\"_blank\" hidden>\r\n\t<v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" esdevVmlButton href=\"\" \r\n" +
                    "                style=\"height:41px; v-text-anchor:middle; width:219px\" arcsize=\"34%\" strokecolor=\"#322020\" strokeweight=\"1px\" fillcolor=\"#cc0000\">\r\n" +
                    "\t\t<w:anchorlock></w:anchorlock>\r\n" +
                    "\t\t<center style='color:#efefef; font-family:georgia, times, \"times new roman\", serif; font-size:15px; font-weight:400; line-height:15px;  mso-text-raise:1px'>Confirmar Cadastro</center>\r\n" +
                    "\t</v:roundrect></a>\r\n<![endif]--><!--[if !mso]><!-- --><span class=\"msohide es-button-border\" style=\"border-style:solid;border-color:#322020;background:#cc0000;border-width:2px;display:inline-block;border-radius:14px;width:auto;mso-hide:all\"><a href=\""+usuarioFilial.token+"\" class=\"es-button msohide\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#efefef;font-size:18px;display:inline-block;background:#cc0000;border-radius:14px;font-family:georgia, times, 'times new roman', serif;font-weight:normal;font-style:normal;line-height:21.6px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #cc0000;mso-hide:all\">Confirmar Cadastro</a></span><!--<![endif]--></td>\r\n" +
                    "                     </tr>\r\n" +
                    "                   </table></td>\r\n" +
                    "                 </tr>\r\n" +
                    "               </table></td>\r\n" +
                    "             </tr>\r\n" +
                    "           </table></td>\r\n" +
                    "         </tr>\r\n" +
                    "       </table>\r\n" +
                    "       <table class=\"es-footer\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
                    "         <tr>\r\n" +
                    "          <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                    "           <table class=\"es-footer-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                    "             <tr>\r\n" +
                    "              <td align=\"left\" bgcolor=\"#393636\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#393636\">\r\n" +
                    "               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                 <tr>\r\n" +
                    "                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                    "                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                    "                     <tr>\r\n" +
                    "                      <td align=\"center\" bgcolor=\"#393636\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#fefafa;font-size:14px\">@ By BIX&nbsp;</p></td>\r\n" +
                    "                     </tr>\r\n" +
                    "                   </table></td>\r\n" +
                    "                 </tr>\r\n" +
                    "               </table></td>\r\n" +
                    "             </tr>\r\n" +
                    "           </table></td>\r\n" +
                    "         </tr>\r\n" +
                    "       </table></td>\r\n" +
                    "     </tr>\r\n" +
                    "   </table>\r\n" +
                    "  </div>\r\n" +
					" </body>\r\n" +
                    "</html>");

				new SmtpClient(_smtp)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_email, _password),
                    EnableSsl = false
                }.Send(new MailMessage(_email, usuario.email)
                {
                    Subject = "Confirmação de cadastro, Bem vindo " + usuario.nome + ", faça seu primeiro acesso",
                    IsBodyHtml=true,
                    Body = stringBuilder.ToString(),
                    Priority = MailPriority.High
                });

                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar CreateVouchers: {ex}");
                return false;
            }
        }
        public bool ComprovanteConvites(Usuario usuario, string link, Evento evento) 
        {
            try
            {
				
				StringBuilder stringBuilder = new StringBuilder();
                
                stringBuilder.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
                    "<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"pt\">\r\n" +
                    "<head>\r\n" +
                    "    <meta charset=\"UTF-8\" />\r\n" +
                    "    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\" />\r\n" +
                    "    <meta name=\"x-apple-disable-message-reformatting\" />\r\n" +
                    "    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n" +
                    "    <meta content=\"telephone=no\" name=\"format-detection\" />\r\n" +
                    "    <title>Convites</title>\r\n" +
                    "    <link href=\"https://fonts.googleapis.com/css?family=Lato:400,400i,700,700i\" rel=\"stylesheet\" />\r\n" +
                    "    <style type=\"text/css\">\r\n" +
                    "        #outlook a {\r\n" +
                    "            padding: 0;\r\n" +
                    "        }\r\n" +
                    "\r\n" +
                    "        .gradiente {\r\n" +
                    "            background-color: #2cb543;\r\n" +
                    "            color: white !important;\r\n" +
                    "        }\r\n" +
                    "\r\n" +
                    "        .es-button {\r\n" +
                    "            mso-style-priority: 100 !important;\r\n" +
                    "            text-decoration: none !important;\r\n" +
                    "        }\r\n" +
                    "\r\n" +
                    "        a[x-apple-data-detectors] {\r\n" +
                    "            color: inherit !important;\r\n" +
                    "            text-decoration: none !important;\r\n" +
                    "            font-size: inherit !important;\r\n" +
                    "            font-family: inherit !important;\r\n" +
                    "            font-weight: inherit !important;\r\n" +
                    "            line-height: inherit !important;\r\n" +
                    "        }\r\n\r\n" +
                    "        .es-desk-hidden {\r\n" +
                    "            display: none;\r\n" +
                    "            float: left;\r\n" +
                    "            overflow: hidden;\r\n" +
                    "            width: 0;\r\n" +
                    "            max-height: 0;\r\n" +
                    "            line-height: 0;\r\n" +
                    "            mso-hide: all;\r\n" +
                    "        }\r\n\r\n" +
                    "        .box-shadow {\r\n" +
                    "            box-shadow: rgba(0, 0, 0, 0.24) 0px 3px 8px;\r\n" +
                    "            margin: 15px 20px;\r\n" +
                    "            padding: 10px 20px;\r\n" +
                    "            border-radius: 10px;\r\n" +
                    "            color: white !IMPORTANT;\r\n" +
                    "        }\r\n\r\n" +
                    "        @media only screen and (max-width:600px) {\r\n" +
                    "            p, ul li, ol li, a {\r\n" +
                    "                line-height: 150% !important\r\n" +
                    "            }\r\n\r\n" +
                    "            h1, h2, h3, h1 a, h2 a, h3 a {\r\n" +
                    "                line-height: 120% !important\r\n" +
                    "            }\r\n\r\n" +
                    "            h1 {\r\n" +
                    "                font-size: 30px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            h2 {\r\n" +
                    "                font-size: 24px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            h3 {\r\n" +
                    "                font-size: 20px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {\r\n" +
                    "                font-size: 30px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {\r\n" +
                    "                font-size: 24px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {\r\n" +
                    "                font-size: 20px !important;\r\n" +
                    "                text-align: left\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-menu td a {\r\n" +
                    "                font-size: 14px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a {\r\n" +
                    "                font-size: 14px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-content-body p, .es-content-body ul li, .es-content-body ol li, .es-content-body a {\r\n" +
                    "                font-size: 14px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a {\r\n" +
                    "                font-size: 14px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a {\r\n" +
                    "                font-size: 12px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            *[class=\"gmail-fix\"] {\r\n" +
                    "                display: none !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 {\r\n" +
                    "                text-align: center !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 {\r\n" +
                    "                text-align: right !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 {\r\n" +
                    "                text-align: left !important\r\n" +
                    "            }\r\n\r\n" +
                    "                .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {\r\n" +
                    "                    display: inline !important\r\n" +
                    "                }\r\n\r\n" +
                    "            .es-button-border {\r\n" +
                    "                display: inline-block !important\r\n" +
                    "            }\r\n\r\n" +
                    "            a.es-button, button.es-button {\r\n" +
                    "                font-size: 18px !important;\r\n" +
                    "                display: inline-block !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-adaptive table, .es-left, .es-right {\r\n" +
                    "                width: 100% !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {\r\n                width: 100% !important;\r\n" +
                    "                max-width: 600px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-adapt-td {\r\n" +
                    "                display: block !important;\r\n" +
                    "                width: 100% !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .adapt-img {\r\n" +
                    "                width: 100% !important;\r\n" +
                    "                height: auto !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p0 {\r\n" +
                    "                padding: 0px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p0r {\r\n" +
                    "                padding-right: 0px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p0l {\r\n" +
                    "                padding-left: 0px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p0t {\r\n" +
                    "                padding-top: 0px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p0b {\r\n" +
                    "                padding-bottom: 0 !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-m-p20b {\r\n" +
                    "                padding-bottom: 20px !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-mobile-hidden, .es-hidden {\r\n" +
                    "                display: none !important\r\n" +
                    "            }\r\n\r\n" +
                    "            tr.es-desk-hidden, td.es-desk-hidden, table.es-desk-hidden {\r\n" +
                    "                width: auto !important;\r\n" +
                    "                overflow: visible !important;\r\n" +
                    "                float: none !important;\r\n" +
                    "                max-height: inherit !important;\r\n" +
                    "                line-height: inherit !important\r\n" +
                    "            }\r\n\r\n" +
                    "            tr.es-desk-hidden {\r\n" +
                    "                display: table-row !important\r\n" +
                    "            }\r\n\r\n" +
                    "            table.es-desk-hidden {\r\n" +
                    "                display: table !important\r\n" +
                    "            }\r\n\r\n" +
                    "            td.es-desk-menu-hidden {\r\n" +
                    "                display: table-cell !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .es-menu td {\r\n" +
                    "                width: 1% !important\r\n" +
                    "            }\r\n\r\n" +
                    "            table.es-table-not-adapt, .esd-block-html table {\r\n" +
                    "                width: auto !important\r\n" +
                    "            }\r\n\r\n" +
                    "            table.es-social {\r\n" +
                    "                display: inline-block !important\r\n" +
                    "            }\r\n\r\n" +
                    "                table.es-social td {\r\n" +
                    "                    display: inline-block !important\r\n" +
                    "                }\r\n\r\n" +
                    "            .es-desk-hidden {\r\n" +
                    "                display: table-row !important;\r\n" +
                    "                width: auto !important;\r\n" +
                    "                overflow: visible !important;\r\n" +
                    "                max-height: inherit !important\r\n" +
                    "            }\r\n\r\n" +
                    "            .h-auto {\r\n" +
                    "                height: auto !important\r\n" +
                    "            }\r\n" +
                    "        }\r\n\r\n" +
                    "        @media screen and (max-width:384px) {\r\n" +
                    "            .mail-message-content {\r\n" +
                    "                width: 414px !important\r\n" +
                    "            }\r\n" +
                    "        }\r\n" +
                    "    </style>\r\n" +
                    "</head>");
				stringBuilder.Append("<body style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n" +
	            "    <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"pt\" style=\"background-color:#EFEFEF\">\r\n" +
	            "        <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#EFEFEF\">\r\n" +
	            "            <tr>\r\n" +
	            "                <td valign=\"top\" style=\"padding:0;Margin:0\">\r\n" +
	            "                    <table class=\"es-header\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
	            "                        <tr>\r\n" +
	            "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
	            "                                <table class=\"es-header-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
	            "                                    <tr>\r\n" +
	            "                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
	            "                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
	            "                                                <tr>\r\n" +
	            "                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
	            "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
	            "                                                            <tr>\r\n" +
                "                                                                <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_fe0b9ecd8c68b601d31ed85906c6b76cdce45f3da681f46277bca21db3847f13/images/logo2.png\" alt=\"\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"300\"></td>\r\n" +
	            "                                                            </tr>\r\n" +
	            "                                                        </table>\r\n" +
	            "                                                    </td>\r\n" +
	            "                                                </tr>\r\n" +
	            "                                            </table>\r\n" +
	            "                                        </td>\r\n" +
	            "                                    </tr>");
				stringBuilder.Append("                                    <tr>\r\n" +
				"                                        <td align=\"left\" bgcolor=\"#efefef\" style=\"padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#efefef\">\r\n" +
				"                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:360px\">\r\n" +
				"                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                            <tr>\r\n" +
				"                                                                <td align=\"left\" style=\"padding:10px;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:25.5px;color:#000000;font-size:17px\"><b>Olá "+usuario.nome+", seus convites estão prontos para serem enviados. Acesse o site para cadastrar os convidados e enviar os convites!</b><br /></p></td>\r\n" +
				"                                                            </tr>\r\n" +
				"                                                        </table>\r\n" +
				"                                                    </td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                            <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:180px\">\r\n" +
				"                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                            <tr>\r\n" +
				"                                                                <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_ef984836ce7278b7c065c8dcf89840cd00021ebcf250afee46beb54fd184b686/images/sem_titulo_mWs.png\" alt=\"alt\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"170\" /></td>\r\n" +
				"                                                            </tr>\r\n" +
				"                                                        </table>\r\n" +
				"                                                    </td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                        </td>\r\n" +
				"                                    </tr>");
				stringBuilder.Append("<tr>\r\n" +
				"                            <td align=\"left\" bgcolor=\"#efefef\" style=\"padding:20px;Margin:0;background-color:#efefef\">\r\n" +
				"                                <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
				"                                    <tr>\r\n" +
				"                                        <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:180px\">\r\n" +
				"                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\""+evento.bannerEvento+"\" alt=\"alt\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"135\" /></td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                        </td>\r\n" +
				"                                    </tr>\r\n" +
				"                                </table>\r\n" +
				"                                <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
				"                                    <tr>\r\n" +
				"                                        <td align=\"left\" style=\"padding:0;Margin:0;width:360px\">\r\n" +
				"                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td align=\"left\" style=\"padding:0;Margin:0\"><h5 style=\"Margin:0;line-height:19.2px;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;font-size:16px\">Evento: "+evento.nomeEvento+"</h5><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\"><br /></p><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">"+evento.Endereco.logradouro+", n"+evento.Endereco.numeroendereco+" - "+evento.Endereco.bairro+"</p><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">"+evento.Endereco.cidade+", "+evento.Endereco.estado+". CEP: "+evento.Endereco.cep+"</p><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">"+evento.dataInicioEvento.ToString("dd MMMM yyyy HH:mm")+" - "+ evento.dataTerminoEvento.ToString("dd MMMM yyyy HH:mm") + "</p></td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                        </td>\r\n" +
				"                                    </tr>\r\n" +
				"                                </table>\r\n" +
				"                            </td>\r\n" +
				"                        </tr>\r\n" +
				"                    </table>");
				stringBuilder.Append("                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
				"                        <tr>\r\n" +
				"                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
				"                                <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
				"                                    <tr>\r\n" +
				"                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-bottom:10px;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
				"                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
				"                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                            <tr>\r\n" +
				"                                                                <td align=\"center\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:30px;color:#333333;font-size:20px\"><strong>Acessar site:</strong></p></td>\r\n" +
				"                                                            </tr>\r\n" +
				"                                                        </table>\r\n" +
				"                                                    </td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                        </td>\r\n" +
				"                                    </tr>\r\n" +
				"                                </table>\r\n" +
				"                            </td>\r\n" +
				"                        </tr>\r\n" +
				"                    </table>\r\n" +
				"                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
				"                        <tr>\r\n" +
				"                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
				"                                <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
				"                                    <tr>\r\n" +
				"                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"padding:5px;Margin:0;background-color:#32cb4b\">\r\n" +
				"                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                <tr>\r\n" +
				"                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:590px\">\r\n" +
				"                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
				"                                                            <tr>\r\n" +
				"                                                                <td align=\"center\" style=\"padding:0;Margin:0\"><span class=\"msohide es-button-border\" style=\"border-style:solid;border-color:#322020;background:#cc0000;border-width:2px 2px 3px;display:inline-block;border-radius:15px;width:auto;mso-hide:all\"><a href=\""+link+"\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#efefef;font-size:18px;display:inline-block;background:#cc0000;border-radius:15px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:21.6px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #cc0000\">Acessar Site</a></span><!--<![endif]--></td>\r\n" +
				"                                                            </tr>\r\n" +
				"                                                        </table>\r\n" +
				"                                                    </td>\r\n" +
				"                                                </tr>\r\n" +
				"                                            </table>\r\n" +
				"                                        </td>\r\n" +
				"                                    </tr>\r\n" +
				"                                </table>\r\n" +
				"                            </td>\r\n" +
				"                        </tr>\r\n" +
				"                    </table>");
				stringBuilder.Append("                    <table class=\"es-footer\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
	            "                        <tr>\r\n" +
	            "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
	            "                                <table class=\"es-footer-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
	            "                                    <tr>\r\n" +
	            "                                        <td align=\"left\" bgcolor=\"#393636\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#393636\">\r\n" +
	            "                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
	            "                                                <tr>\r\n" +
	            "                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
	            "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
	            "                                                            <tr>\r\n" +
	            "                                                                <td align=\"center\" bgcolor=\"#393636\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#fefafa;font-size:14px\">@ By BIX&nbsp;</p></td>\r\n" +
	            "                                                            </tr>\r\n" +
	            "                                                        </table>\r\n" +
	            "                                                    </td>\r\n" +
	            "                                                </tr>\r\n" +
	            "                                            </table>\r\n" +
	            "                                        </td>\r\n" +
	            "                                    </tr>\r\n" +
	            "                                </table>\r\n" +
	            "                            </td>\r\n" +
	            "                        </tr>\r\n" +
	            "                    </table>\r\n" +
	            "                </td>\r\n" +
	            "            </tr>\r\n" +
	            "        </table>\r\n" +
	            "    </div>\r\n" +
	            "</body>\r\n" +
	            "</html>");

				new SmtpClient(_smtp)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_email, _password),
                    EnableSsl = false
                }.Send(new MailMessage(_email, usuario.email)
                {
                    Subject = "Confirmação de pedido de convites para evento " + evento.nomeEvento ,
                    IsBodyHtml=true,
                    Body = stringBuilder.ToString(),
                    Priority = MailPriority.High
                });

                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar CreateVouchers: {ex}");
                return false;
            }
        }
        public bool EnviarConvite(Convidado convidado, string link) 
        {
            try
            {
                convidado.Convite = _context.Convites.Where(s => s.codConvite == convidado.codConvite).Include(s=>s.Usuario).Include(s=>s.Evento).ThenInclude(s=>s.Endereco).FirstOrDefault();
                convidado.Ingresso = _context.Ingressos.Where(s => s.codIngresso == convidado.codIngresso).FirstOrDefault();
                if (convidado !=null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
                        "<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"pt\">\r\n" +
                        " <head>\r\n" +
                        "  <meta charset=\"UTF-8\">\r\n" +
                        "  <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n" +
                        "  <meta name=\"x-apple-disable-message-reformatting\">\r\n" +
                        "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n" +
                        "  <meta content=\"telephone=no\" name=\"format-detection\">\r\n" +
                        "  <title>Empty template</title>\r\n" +
                        "  <link href=\"https://fonts.googleapis.com/css?family=Lato:400,400i,700,700i\" rel=\"stylesheet\"><!--<![endif]-->\r\n" +
                        "  <style type=\"text/css\">\r\n" +
                        "#outlook a {\r\n" +
                        "\tpadding:0;\r\n" +
                        "}\r\n" +
                        ".gradiente {\r\n" +
                        "\tbackground-color:#2cb543;\r\n\tcolor:white!important;\r\n}\r\n.es-button {\r\n\tmso-style-priority:100!important;\r\n\ttext-decoration:none!important;\r\n}\r\na[x-apple-data-detectors] {\r\n\tcolor:inherit!important;\r\n\ttext-decoration:none!important;\r\n" +
                        "\tfont-size:inherit!important;\r\n" +
                        "\tfont-family:inherit!important;\r\n" +
                        "\tfont-weight:inherit!important;\r\n\tline-height:inherit!important;\r\n" +
                        "}\r\n.es-desk-hidden {\r\n\tdisplay:none;\r\n" +
                        "\tfloat:left;\r\n\toverflow:hidden;\r\n" +
                        "\twidth:0;\r\n" +
                        "\tmax-height:0;\r\n\tline-height:0;\r\n\tmso-hide:all;\r\n" +
                        "}\r\n.box-shadow {\r\n" +
                        "\tbox-shadow:rgba(0, 0, 0, 0.24) 0px 3px 8px;\r\n" +
                        "\tmargin:15px 20px;\r\n\tpadding:10px 20px;\r\n" +
                        "\tborder-radius:10px;\r\n\tcolor:white !IMPORTANT;\r\n" +
                        "}\r\n" +
                        "@media only screen and (max-width:600px) {p, ul li, ol li, a { line-height:150%!important }" +
                        " h1, h2, h3, h1 a, h2 a, h3 a { line-height:120%!important } h1 { font-size:30px!important; text-align:left }" +
                        " h2 { font-size:24px!important; text-align:left } h3 { font-size:20px!important; text-align:left } .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:30px!important; text-align:left }" +
                        " .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:24px!important; text-align:left }" +
                        " .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px!important; text-align:left } .es-menu td a { font-size:14px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:14px!important }" +
                        " .es-content-body p, .es-content-body ul li, .es-content-body ol li, .es-content-body a { font-size:14px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:14px!important }" +
                        " .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=\"gmail-fix\"] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important }" +
                        " .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important }" +
                        " .es-button-border { display:inline-block!important } a.es-button, button.es-button { font-size:18px!important; display:inline-block!important } .es-adaptive table, .es-left, .es-right { width:100%!important }" +
                        " .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important }" +
                        " .es-m-p0 { padding:0px!important }" +
                        " .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important }" +
                        " .es-mobile-hidden, .es-hidden { display:none!important } tr.es-desk-hidden, td.es-desk-hidden, table.es-desk-hidden { width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } tr.es-desk-hidden { display:table-row!important }" +
                        " table.es-desk-hidden { display:table!important } td.es-desk-menu-hidden { display:table-cell!important } .es-menu td { width:1%!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important }" +
                        " .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; max-height:inherit!important } .h-auto { height:auto!important } }\r\n@media screen and (max-width:384px) {.mail-message-content { width:414px!important } }\r\n" +
                        "</style>\r\n" +
                        " </head>");
                    stringBuilder.Append("<body style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n" +
                        "<img src=\""+link+"/Convites/VisualisarEmail/"+convidado.codConvidado+"\" style=\"display:none;\" width=\"1\" height=\"1\">" +
                        "    <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"pt\" style=\"background-color:#EFEFEF\">\r\n" +
                        "        <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#EFEFEF\">\r\n" +
                        "            <tr>\r\n" +
                        "                <td valign=\"top\" style=\"padding:0;Margin:0\">\r\n" +
                        "                    <table class=\"es-header\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
                        "                        <tr>\r\n" +
                        "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                <table class=\"es-header-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_fe0b9ecd8c68b601d31ed85906c6b76cdce45f3da681f46277bca21db3847f13/images/logo2.png\" alt=alt style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"300\" /></td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                    <tr>");
                    stringBuilder.Append("                                        <td align=\"left\" bgcolor=\"#efefef\" style=\"padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#efefef\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:360px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"left\" style=\"padding:10px;Margin:0\">" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:25.5px;color:#000000;font-size:17px\"><b>Olá, voçê está sendo convidado para o evento " + convidado.Convite.Evento.nomeEvento + ", por " + convidado.Convite.Usuario.nome + " Divirta-se!</b></p>" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#000000;font-size:14px\">" + convidado.Convite.msgConvite + "</p>\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#000000;font-size:14px\"><br /></p>\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#000000;font-size:14px\">Mais Informações do evento <a target=\"_blank\" href=\"" + convidado.Convite.Evento.linkEvento + "\">Clique aqui</a></p>\r\n" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                            <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:180px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"" + convidado.Convite.imagemConvite + "\" alt=alt style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"180\" /></td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:10px;Margin:0;font-size:0\">\r\n" +
                        "                                                                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" dir=\"ltr\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                                        <tr>\r\n" +
                        "                                                                            <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:10px\"><a target=\"_blank\" href=\"https://testes\" style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2CB543;font-size:14px\"><img src=\"https://ecvvmha.stripocdn.email/content/assets/img/social-icons/circle-colored/facebook-circle-colored.png\" alt=\"" + convidado.Convite.facebook + "\" title=\"Facebook\" width=\"32\" height=\"32\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" /></a></td>\r\n" +
                        "                                                                            <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><a target=\"_blank\" href=\"https://testeinsta\" style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2CB543;font-size:14px\"><img src=\"https://ecvvmha.stripocdn.email/content/assets/img/social-icons/circle-colored/instagram-circle-colored.png\" alt=\"" + convidado.Convite.instagram + "\" title=\"Instagram\" width=\"32\" height=\"32\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" /></a></td>\r\n" +
                        "                                                                        </tr>\r\n" +
                        "                                                                    </table>\r\n" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" bgcolor=\"#efefef\" style=\"padding:20px;Margin:0;background-color:#efefef\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:180px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\">" +
                        "                                                                   <img class=\"adapt-img\" src=\"" + convidado.Convite.Evento.bannerEvento + "\" alt=alt style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"135\" />\r\n" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                            <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:360px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"left\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                                                   <h5 style=\"Margin:0;line-height:19.2px;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;font-size:16px\">Evento: " + convidado.Convite.Evento.nomeEvento + "</h5>" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\"><br /></p>\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">" + convidado.Convite.Evento.Endereco.logradouro + ", n" + convidado.Convite.Evento.Endereco.numeroendereco + "-" + convidado.Convite.Evento.Endereco.bairro + "</p>\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">CEP: " + convidado.Convite.Evento.Endereco.cep + ", " + convidado.Convite.Evento.Endereco.cidade + " , " + convidado.Convite.Evento.Endereco.estado + "</p>\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#333333;font-size:14px\">" + convidado.Convite.Evento.dataInicioEvento.ToString("dd MMMM yyyy HH:mm") + " - " + convidado.Convite.Evento.dataTerminoEvento.ToString("dd MMMM yyyy HH:mm") + "</p></td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                               </table>\r\n" +
                        "                            </td>\r\n" +
                        "                        </tr>\r\n" +
                        "                    </table>");
                    stringBuilder.Append("                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
                        "                        <tr>\r\n" +
                        "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#efefef;width:600px\" role=\"none\">\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" style=\"padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px\"\r\n" +
                        "                                           <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                        "                                             <tr>\r\n" +
                        "                                              <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:370px\">\r\n" +
                        "                                               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                 <tr>\r\n" +
                        "                                                  <td style=\"padding:0;Margin:0\">\r\n" +
                        "                                                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                        "                                                     <tr>\r\n" +
                        "                                                      <td class=\"es-m-p0r es-m-p20b\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:360px\">\r\n" +
                        "                                                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                         <tr>\r\n" +
                        "                                                          <td align=\"left\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:26px;color:#333333;font-size:17px\"><strong>Convite de " + convidado.Convite.Usuario.nome + ":</strong><strong><span style=\"font-family:lato, 'helvetica neue', helvetica, arial, sans-serif\"></span></strong><br></p>\r\n" +
                        "                                                          <div class=\"box-shadow gradiente\" style=\"background-color:#2CB543;color:white !important;box-shadow:#0000003D 0px 3px 8px;margin:15px 20px;padding:10px 20px;border-radius:10px\">\r\n" +
                        "                                                          <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:white;font-size:14px\">Código: " + convidado.Ingresso.ticketIngresso+ "</p>\r\n" +
                        "                                                          <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">Nome:" + convidado.nomeConvidado+ "</p>\r\n" +
                        "                                                          <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">Email:" + convidado.emailConvidado+ "</p>\r\n" +
                        "                                                          <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">Contato:" + convidado.telefoneConvidado+"</p>\r\n" +
                        "                                                          </div></td>\r\n" +
                        "                                                         </tr>\r\n" +
                        "                                                       </table></td>\r\n" +
                        "                                                     </tr>\r\n" +
                        "                                                   </table></td>\r\n" +
                        "                                                 </tr>\r\n" +
                        "                                               </table></td>\r\n" +
                        "                                             </tr>\r\n" +
                        "                                           </table>\r\n" +
                        "                                           <table class=\"es-right\" cellpadding=\"0\" cellspacing=\"0\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
                        "                                             <tr>\r\n" +
                        "                                              <td align=\"left\" style=\"padding:0;Margin:0;width:170px\">\r\n" +
                        "                                               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                 <tr>\r\n" +
                        "                                                  <td align=\"center\" style=\"padding:0;Margin:0;font-size:0px\"><img class=\"adapt-img\" src=\"https://ecvvmha.stripocdn.email/content/guids/CABINET_fe0b9ecd8c68b601d31ed85906c6b76cdce45f3da681f46277bca21db3847f13/images/sem_titulo.png\" alt style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic\" width=\"135\"></td>\r\n" +
                        "                                                 </tr>\r\n" +
                        "                                               </table></td>\r\n" +
                        "                                             </tr>\r\n" +
                        "                                           </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                </table>\r\n" +
                        "                            </td>\r\n" +
                        "                        </tr>\r\n" +
                        "                    </table>");
                    stringBuilder.Append("                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
                        "                        <tr>\r\n" +
                        "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"Margin:0;padding-bottom:10px;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#32cb4b\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                                                   <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:30px;color:#333333;font-size:20px\"><strong>Confirmar Presença:</strong></p>\r\n" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                </table>\r\n" +
                        "                            </td>\r\n" +
                        "                        </tr>\r\n" +
                        "                    </table>\r\n" +
                        "                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n" +
                        "                        <tr>\r\n" +
                        "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" bgcolor=\"#32cb4b\" style=\"padding:20px;Margin:0;background-color:#32cb4b\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td class=\"es-m-p20b\" align=\"left\" style=\"padding:0;Margin:0;width:270px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0\"><span class=\"msohide es-button-border\" style=\"border-style:solid;border-color:#ffffff;background:#13795b;border-width:2px;display:inline-block;border-radius:30px;width:auto;mso-hide:all\">" +
                        "                                                                   <a href=\""+link+"/Convites/Confirmar/"+convidado.codConvidado+ "?resposta=true\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#ffffff;font-size:18px;display:inline-block;background:#13795b;border-radius:30px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:bold;font-style:normal;line-height:21.6px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #13795b\">Sim</a></span>\r\n<!--<![endif]-->" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:270px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" style=\"padding:0;Margin:0\">" +
                        "                                                                   <span class=\"msohide es-button-border\" style=\"border-style:solid;border-color:#ffffff;background:#c6303e;border-width:2px;display:inline-block;border-radius:30px;width:auto;mso-hide:all\">" +
                        "                                                                   <a href=\""+link+"/Convites/ConfirmarPresenca/"+convidado.codConvidado+ "?resposta=false\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#ffffff;font-size:18px;display:inline-block;background:#c6303e;border-radius:30px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:bold;font-style:normal;line-height:21.6px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #c6303e\">Não</a>" +
                        "                                                                   </span><!--<![endif]-->" +
                        "                                                                </td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                </table>\r\n" +
                        "                            </td>\r\n" +
                        "                        </tr>\r\n" +
                        "                    </table>");
                    stringBuilder.Append("                   <table class=\"es-footer\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n" +
                        "                        <tr>\r\n" +
                        "                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n" +
                        "                                <table class=\"es-footer-body\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n" +
                        "                                    <tr>\r\n" +
                        "                                        <td align=\"left\" bgcolor=\"#393636\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-color:#393636\">\r\n" +
                        "                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                <tr>\r\n" +
                        "                                                    <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n" +
                        "                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n" +
                        "                                                            <tr>\r\n" +
                        "                                                                <td align=\"center\" bgcolor=\"#393636\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:lato, 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#fefafa;font-size:14px\">@ By BIX&nbsp;</p></td>\r\n" +
                        "                                                            </tr>\r\n" +
                        "                                                        </table>\r\n" +
                        "                                                    </td>\r\n" +
                        "                                                </tr>\r\n" +
                        "                                            </table>\r\n" +
                        "                                        </td>\r\n" +
                        "                                    </tr>\r\n" +
                        "                                </table>\r\n" +
                        "                            </td>\r\n" +
                        "                        </tr>\r\n" +
                        "                    </table>\r\n" +
                        "                </td>\r\n" +
                        "            </tr>\r\n" +
                        "        </table>\r\n" +
                        "    </div>\r\n" +
                        "</body>\r\n" +
                        "</html>");

                    SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
                    client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                    client.Credentials = new NetworkCredential(_email, _password);
                    client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                    // Criando o e-mail
                    MailMessage mailMessage = new MailMessage();
                    mailMessage = new MailMessage(_email, convidado.emailConvidado);
                    mailMessage.Subject = "Convite de " + convidado.Convite.Usuario.nome + " para evento " + convidado.Convite.Evento.nomeEvento;
                    mailMessage.Body = stringBuilder.ToString();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    client.Send(mailMessage);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarConvite: {ex}");
                return false ;
            }
        }
        public async Task<bool> EnviarConviteWhats(Mensagem mensagem)
        {
            try
            {
                MensagemWhats mensagemWhats = new MensagemWhats
                {
                    token=mensagem.token,
                    number =mensagem.numero,
                    link = mensagem.urlImagem,
                    type = "image",
                    message = mensagem.textoConvite
                };

                var jsonContent = JsonSerializer.Serialize(mensagemWhats);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        mensagemWhats.type = "text";
                        mensagemWhats.link = null;
                        mensagemWhats.message = mensagem.textoConfirmacao;
                        jsonContent = JsonSerializer.Serialize(mensagemWhats);
                        content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);
                        if (response.IsSuccessStatusCode)
                        {
                            responseContent = await response.Content.ReadAsStringAsync();
                            result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                            if (result.success)
                            {
                                return true;
                            }
                            else
                            {
                                ErrorViewModel.LogError($"Erro ao chamar EnviarConviteWhats{DateTime.Now}: {result.message}");
                                return false;
                            }
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            ErrorViewModel.LogError($"Erro ao chamar EnviarConviteWhats: {errorContent}");
                            return false;
                        }
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarConviteWhats{DateTime.Now}: {response.StatusCode}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarConviteWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarConvitesWhats: {ex}");
                return false;
            }

        }
        public bool EnviarCupom(string caminho, string email, string nome)
		{
			try
			{
                caminho = caminho.Replace(" ", "");
                caminho = caminho.Replace("%", "&");
                SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
				client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
				client.Credentials = new NetworkCredential(_email, _password);
				client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

				// Criando o e-mail
				MailMessage mailMessage = new MailMessage();
				mailMessage = new MailMessage(_email, email);
				mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
				mailMessage.Subject = "Você ganhou um cupom de desconto, Aproveite";
				mailMessage.Body = "Prezado " + nome + " abaixo anexado segue um cupom de desconto para utilizar em nosso estabelecimento, agradecemos a preferência!";
				mailMessage.Priority = MailPriority.High;

				if (File.Exists(caminho))
				{
					// Criando o anexo
					Attachment attachment = new Attachment(caminho);
					attachment.Name = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes("Cupom.png"));
					mailMessage.Attachments.Add(attachment);

					client.Send(mailMessage);

					return true;
				}
                else
                {
                    return false;
                }
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar EnviarCupons: {ex}");
				return false;
			}
		}
        public async Task<bool> EnviarCupomWhats(string caminho, string telefone, string nome, string nomeFilial, string token, string produtos, string validade)
        {
            try
            {
                caminho = caminho.Replace(" ", "");
                caminho =caminho.Replace("%", "&");
                telefone = "55" + telefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                MensagemWhats mensagem = new MensagemWhats();
                mensagem.token = token;
                mensagem.number = telefone;
                mensagem.link = caminho;
                mensagem.type = "image";
                mensagem.message = "Olá " + nome + " este é seu cupom de desconte! Válido até: " + validade + "\n" +
                                         "Agradecemos a preferência, este é um cupom de desconto para utilizar nossos serviços!\n" + nomeFilial + "\n" +
                                         "Estes são os produtos disponivéis:\n" + produtos;
                var jsonContent = JsonSerializer.Serialize(mensagem);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarCupomWhats{DateTime.Now}: {result.message}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarCupomWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {ex}");
                return false;
            }
        }
        public bool EnviarVoucher(string caminho, string email, string? nome)
		{
			try
			{
				SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
				client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
				client.Credentials = new NetworkCredential(_email, _password);
				client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

				// Criando o e-mail
				MailMessage mailMessage = new MailMessage();
				mailMessage = new MailMessage(_email, email);
				mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
				mailMessage.Subject = "Comprovante de voucher adquerido";
				mailMessage.Body = "Prezado " + nome + " abaixo anexado segue seu voucher para utilizar em nosso estabelecimento, agradecemos a preferência!";
				mailMessage.Priority = MailPriority.High;

				if (File.Exists(caminho))
				{
					// Criando o anexo
					Attachment attachment = new Attachment(caminho);
					attachment.Name = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes("Voucher.png"));
					mailMessage.Attachments.Add(attachment);

					client.Send(mailMessage);

					return true;
				}
                else
                {
                    return false;
                }
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar EnviarVoucher: {ex}");
				return false;
			}
		}
        public async Task<bool> EnviarVoucherWhats(string caminho, string telefone, string nome, string nomeFilial, string token)
        {
            try
            {
                MensagemWhats mensagem = new MensagemWhats();
                mensagem.link = caminho;
                mensagem.token = token;
                mensagem.type = "image";
                mensagem.number = telefone;
                mensagem.message = "Olá " + nome + " este é o voucher de crédito que você solicitou." + "Agradecemos a preferência!\n " + nomeFilial;

                var jsonContent = JsonSerializer.Serialize(mensagem);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats{DateTime.Now}: {result.message}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {ex}");
                return false;
            }
        }
        public bool EnviarIngresso(string caminho, string email, string? nome)
        {
			try
			{
				SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
				client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
				client.Credentials = new NetworkCredential(_email, _password);
				client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

				// Criando o e-mail
				MailMessage mailMessage = new MailMessage();
				mailMessage = new MailMessage(_email, email);
				mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
				mailMessage.Subject = "Comprovante de ingresso adquerido";
				mailMessage.Body = "Prezado " + nome + " abaixo segue em anexo seu ingresso para utilizar em nosso estabelecimento, agradecemos a preferência!";
				mailMessage.Priority = MailPriority.High;

				if (File.Exists(caminho))
				{
					// Criando o anexo
					Attachment attachment = new Attachment(caminho);
					attachment.Name = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes("ingresso.png"));
					mailMessage.Attachments.Add(attachment);

					client.Send(mailMessage);

					return true;
                }
                else
                {
                    return false;
                }
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar EnviarIngresso: {ex}");
				return false;
			}
		}
        public async Task<bool> EnviarIngressoWhats(MensagemWhats mensagem)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(mensagem);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarIngressoWhats{DateTime.Now}: {result.message}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarIngressoWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarConvitesWhats: {ex}");
                return false;
            }

        }
        public bool EnviarReciboPresente(string caminho, ReciboPresente recibo) 
        {
            try
            {
                SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential(_email, _password);
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage(_email, recibo.emailCliente);
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Comprovante de Compra PapiFast";
                mailMessage.Body = "Seu presenteado irá ficar feliz";
                mailMessage.Priority = MailPriority.High;

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarCupons: {ex}");
                return false;
            }
        }
        public bool EnviarToken(string email, string nome,string token) 
        {
            try
            {
                SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential(_email, _password);
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP
                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage(_email, email);
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Token de Acesso PapiFast";
                mailMessage.Body = "Prezado " + nome + ", abaixo segue seu token de acesso para utilizar validação da compra, agradecemos a preferência!\r\nToken: "+token;
                mailMessage.Priority = MailPriority.High;
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarToken: {ex}");
                return false;
            }
        }
        public bool EnviarNotPresente(string email, string nome, ReciboPresente recibopresente)
        {
            try
            {
                SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential(_email, _password);
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage(_email, email);
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Parabens você recebeu um presente";
                mailMessage.Body = "Prezado " + nome + " você acaba de receber um dos presentes solicitados em seu evento.\n " + recibopresente.nomeCliente + " lhe presenteou com: \n";
                foreach (var presente in recibopresente.Presentes)
                {
                    mailMessage.Body += presente.Nome+"\n";
                }
                mailMessage.Body+= "Agradecemos a preferência!\nMais informações entrar com sua conta e acessar: \n https://printweb.vlks.com.br/ReciboPresentes";
                mailMessage.Priority = MailPriority.High;

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarCupons: {ex}");
                return false;
            }
        }
        public async Task<bool> EnviarNotPresenteWhats(string token, string nome,string telefone, ReciboPresente recibopresente)
        {
            try
            {
                MensagemWhats mensagem = new MensagemWhats();
                mensagem.token = token;
                mensagem.type = "text";
                mensagem.number = telefone;
                mensagem.message = "Olá " + nome + " confirmou presença em seu Evento!\nAgradecemos a preferência!\n ";

                var jsonContent = JsonSerializer.Serialize(mensagem);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats{DateTime.Now}: {result.message}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {ex}");
                return false;
            }
        }
        public bool EnviarNotPresenca(Convidado convidado)
        {
            try
            {
                SmtpClient client = new SmtpClient(_smtp); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential(_email, _password);
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage(_email, convidado.Convite.Usuario.email);
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Confimação de presença em seu evento";
                mailMessage.Body = "Prezado " + convidado.Convite.Usuario.email + ", "+convidado.nomeConvidado+" acaba de confirmar presença em seu evento.\n ";
                mailMessage.Body += "Agradecemos a preferência!\nMais informações entrar com sua conta e acessar a area de convites";
                mailMessage.Priority = MailPriority.High;

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarCupons: {ex}");
                return false;
            }
        }
        public async Task<bool> EnviarNotPresencaWhats(Convidado convidado)
        {
            try
            {
                MensagemWhats mensagem = new MensagemWhats();
                mensagem.token = convidado.Convite.Usuario.token;
                mensagem.type = "text";
                mensagem.number = convidado.Convite.Usuario.telefone;
                mensagem.message = "Olá " + convidado.Convite.Usuario.nome + ". Seu convidado "+convidado.nomeConvidado+" confirmou presença em seu Evento!\nAgradecemos a preferência!\n ";

                var jsonContent = JsonSerializer.Serialize(mensagem);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://cloud.datafychats.com.br/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MensagemWhatsResponse>(responseContent);
                    if (result.success)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats{DateTime.Now}: {result.message}");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EnviarVoucherWhats: {ex}");
                return false;
            }
        }
        public bool GerarCupon(int id, string caminho, string link)
        {
            var cupon = _context.Cupons.Where(s => s.codCupom == id).Include(s => s.UsuarioFilial).ThenInclude(s => s.Filial).ThenInclude(s => s.Endereco).Include(s => s.usuario).Include(s => s.Campanha).FirstOrDefault();
            if (cupon == null)
            {
                return false;
            }
            try
            {
                if (cupon.produtos != null && cupon.produtos != "")
                {
                    string[] produto = cupon.produtos.Split(",");
                    cupon.produtos = "";
                    for (int i = 0; i < produto.Length - 1; i++)
                    {
                        if (produto[i] == "")
                        {
                            cupon.produtos += ".";
                        }
                        else
                        {
                            int codProduto = int.Parse(produto[i]);
                            var dadosproduto = _context.Produtos.Find(codProduto);
                            if (dadosproduto != null)
                            {
                                cupon.produtos += dadosproduto.nomeProduto + ", ";
                            }
                        }
                    }
                }
                string saveimg = caminho + cupon.tokenCupom + "-" + cupon.codCupom.ToString() + ".png";
                caminho = caminho.Replace(" ", "");
                caminho = caminho.Replace("%", "&");
                int width = 1000;
                int height = 500;

                // Criar a imagem em branco
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    using (Graphics graph = Graphics.FromImage(bmp))
                    {
                        // Definir a cor de fundo (branco)
                        graph.Clear(Color.White);

                        // Definir fontes
                        System.Drawing.Font fontTitulo = new System.Drawing.Font("Arial", 24, FontStyle.Bold);
                        System.Drawing.Font fontTexto = new System.Drawing.Font("Arial", 10, FontStyle.Regular);
                        System.Drawing.Font fontTitulo1 = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
                        System.Drawing.Font fontTitulo2 = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
                        System.Drawing.Font fontDestaque = new System.Drawing.Font("Arial", 18, FontStyle.Bold);
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        // Definir pincéis
                        Brush brushPreto = Brushes.Black;

                        // Desenhar QR code (exemplo simples, ideal usar biblioteca externa para gerar um QR code real)
                        // No exemplo estou apenas desenhando um quadrado como marcador de onde o QR code ficaria
                        graph.FillRectangle(brushPreto, new System.Drawing.Rectangle(75, 75, 350, 350));
                        var qrcode = GenerateQRCode(link, 350, 350);

                        graph.DrawImage(qrcode, 75, 75, 350, 350);
                        // Desenhar o cabeçalho da empresa
                        graph.DrawString(cupon.UsuarioFilial.Filial.nome, fontTitulo1, brushPreto, new PointF(725, 110), stringFormat);
                        graph.DrawString("CNPJ: "+ cupon.UsuarioFilial.Filial.cnpj, fontTexto, brushPreto, new PointF(725, 130), stringFormat);
                        graph.DrawString(cupon.UsuarioFilial.Filial.Endereco.logradouro + ", n"+ cupon.UsuarioFilial.Filial.Endereco.numeroendereco + " - "+ cupon.UsuarioFilial.Filial.Endereco.bairro, fontTexto, brushPreto, new PointF(725, 147), stringFormat);
                        graph.DrawString(cupon.UsuarioFilial.Filial.Endereco.cidade+" - "+ cupon.UsuarioFilial.Filial.Endereco.estado, fontTexto, brushPreto, new PointF(725, 165), stringFormat);
                        graph.DrawString("CEP: "+ cupon.UsuarioFilial.Filial.Endereco.cep, fontTexto, brushPreto, new PointF(725, 185), stringFormat);

                        // Linha divisória
                        graph.DrawLine(new Pen(Color.Black, 1), new Point(500, 210), new Point(950, 210));

                        // Desenhar o título do cupom
                        graph.DrawString(cupon.tokenCupom, fontDestaque, brushPreto, new PointF(725, 240), stringFormat);
                        graph.DrawString("Cupom de Desconto", fontTitulo1, brushPreto, new PointF(725, 270), stringFormat);

                        // Informações do cupom
                        if (cupon.Campanha==null)
                        {
                            graph.DrawString("Campanha: _______________________________________", fontTitulo1, brushPreto, new PointF(500, 285));

                        }
                        else
                        {
                            graph.DrawString("Campanha: "+cupon.Campanha.nomeCampanha, fontTitulo1, brushPreto, new PointF(500, 285));

                        }
                        graph.DrawString("Validade: " + cupon.validadeCupom.ToString("MM/dd/yyyy") + "                        Valor do Desconto: "+cupon.descontoCupom+"%", fontTitulo1, brushPreto, new PointF(500, 310));

                        // Informações do cliente
                        if (cupon.usuario==null)
                        {
                            graph.DrawString("Cliente:                                                    CPF:                                       ", fontTitulo2, brushPreto, new PointF(500, 345));
                            graph.DrawString("Email:                                                      Contato:(  )        -                      ", fontTitulo2, brushPreto, new PointF(500, 375));

                        }
                        else
                        {
                            graph.DrawString("Cliente: "+cupon.usuario.nome, fontTitulo2, brushPreto, new PointF(500, 345));
                            graph.DrawString("CPF: "+cupon.usuario.cpf, fontTitulo2, brushPreto, new PointF(745, 345));
                            graph.DrawString("Email: "+cupon.usuario.email, fontTitulo2, brushPreto, new PointF(500, 375));
                            graph.DrawString("Contato: "+cupon.usuario.telefone, fontTitulo2, brushPreto, new PointF(745, 375));

                        }
                        // Definir o retângulo para limitar o texto (com quebra automática de linha)
                        RectangleF rect = new RectangleF(500, 405, 470, 120);
                        // Configurar o formato para quebra de linha
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Near;

                        graph.DrawString("Produtos disponíveis: " + cupon.produtos, fontTitulo2, brushPreto,rect, format);
                        // Salvar a imagem como PNG
                        bmp.Save(saveimg, ImageFormat.Png);

                        if (System.IO.File.Exists(saveimg))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar GerarCupons: {ex}");
                return false;
            }
        }
		public bool GerarVoucher(int id, string caminho, string link)
		{
			var voucher = _context.Vouchers.Where(s => s.codVoucher == id).Include(s => s.UsuarioFilial).ThenInclude(s => s.Filial).ThenInclude(s => s.Endereco).Include(s => s.Cliente).Include(s => s.Campanha).FirstOrDefault();
			if (voucher == null)
			{
				return false;
			}
			try
			{
				string saveimg = caminho + voucher.tokenVoucher +".png";
				int width = 1000;
				int height = 500;

				// Criar a imagem em branco
				using (Bitmap bmp = new Bitmap(width, height))
				{
					using (Graphics graph = Graphics.FromImage(bmp))
					{
						// Definir a cor de fundo (branco)
						graph.Clear(Color.White);

						// Definir fontes
						System.Drawing.Font fontTitulo = new System.Drawing.Font("Arial", 24, FontStyle.Bold);
						System.Drawing.Font fontTexto = new System.Drawing.Font("Arial", 10, FontStyle.Regular);
						System.Drawing.Font fontTitulo1 = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
						System.Drawing.Font fontTitulo2 = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
						System.Drawing.Font fontDestaque = new System.Drawing.Font("Arial", 18, FontStyle.Bold);
						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						// Definir pincéis
						Brush brushPreto = Brushes.Black;

						// Desenhar QR code (exemplo simples, ideal usar biblioteca externa para gerar um QR code real)
						// No exemplo estou apenas desenhando um quadrado como marcador de onde o QR code ficaria
						graph.FillRectangle(brushPreto, new System.Drawing.Rectangle(75, 75, 350, 350));
						var qrcode = GenerateQRCode(link, 350, 350);

						graph.DrawImage(qrcode, 75, 75, 350, 350);
						// Desenhar o cabeçalho da empresa
						graph.DrawString(voucher.UsuarioFilial.Filial.nome, fontTitulo1, brushPreto, new PointF(725, 110), stringFormat);
						graph.DrawString("CNPJ: " + voucher.UsuarioFilial.Filial.cnpj, fontTexto, brushPreto, new PointF(725, 130), stringFormat);
						graph.DrawString(voucher.UsuarioFilial.Filial.Endereco.logradouro + ", n" + voucher.UsuarioFilial.Filial.Endereco.numeroendereco + " - " + voucher.UsuarioFilial.Filial.Endereco.bairro, fontTexto, brushPreto, new PointF(725, 147), stringFormat);
						graph.DrawString(voucher.UsuarioFilial.Filial.Endereco.cidade + " - " + voucher.UsuarioFilial.Filial.Endereco.estado, fontTexto, brushPreto, new PointF(725, 165), stringFormat);
						graph.DrawString("CEP: " + voucher.UsuarioFilial.Filial.Endereco.cep, fontTexto, brushPreto, new PointF(725, 185), stringFormat);

						// Linha divisória
						graph.DrawLine(new Pen(Color.Black, 1), new Point(500, 210), new Point(950, 210));

						// Desenhar o título do cupom
						graph.DrawString("Voucher", fontDestaque, brushPreto, new PointF(725, 240), stringFormat);
						graph.DrawString("Código: "+ voucher.tokenVoucher, fontTitulo1, brushPreto, new PointF(725, 270), stringFormat);

						// Informações do cupom
						if (voucher.Campanha != null)
						{
							graph.DrawString("Campanha: " + voucher.Campanha.nomeCampanha, fontTitulo1, brushPreto, new PointF(500, 285));
						}
						
                        graph.DrawString("Validade: " + voucher.validadeVoucher.ToString("MM/dd/yyyy") + "                     Valor Dísponivel: R$" + voucher.valorDisponivel, fontTitulo1, brushPreto, new PointF(500, 310));

						// Informações do cliente
						if (voucher.Cliente == null)
						{
							graph.DrawString("Cliente:                                                    CPF:                                       ", fontTitulo2, brushPreto, new PointF(500, 345));
							graph.DrawString("Email:                                                      Contato:(  )        -                      ", fontTitulo2, brushPreto, new PointF(500, 375));

						}
						else
						{
							graph.DrawString("Cliente: " + voucher.Cliente.nome, fontTitulo2, brushPreto, new PointF(500, 345));
							graph.DrawString("CPF: " + voucher.Cliente.cpf, fontTitulo2, brushPreto, new PointF(745, 345));
							graph.DrawString("Email: " + voucher.Cliente.email, fontTitulo2, brushPreto, new PointF(500, 375));
							graph.DrawString("Contato: " + voucher.Cliente.telefone, fontTitulo2, brushPreto, new PointF(745, 375));

						}
						// Salvar a imagem como PNG
						bmp.Save(saveimg, ImageFormat.Png);

						if (System.IO.File.Exists(saveimg))
						{
							return true;
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar GerarVoucher: {ex}");
				return false;
			}
		}
        public bool GerarIngresso(int id, string caminho, string link)
        {
			var ingresso = _context.Ingressos.Where(s => s.codIngresso == id).Include(s => s.Lote).ThenInclude(s => s.Evento).ThenInclude(s => s.Endereco).FirstOrDefault();
			if (ingresso == null)
			{
				return false;
			}
			try
			{
				string saveimg = caminho + ingresso.ticketIngresso + ".jpg";
				int width = 1000;
				int height = 500;

				// Criar a imagem em branco
				using (Bitmap bmp = new Bitmap(width, height))
				{
					using (Graphics graph = Graphics.FromImage(bmp))
					{
						// Definir a cor de fundo (branco)
						graph.Clear(Color.White);

						// Definir fontes
						System.Drawing.Font fontTitulo = new System.Drawing.Font("Arial", 24, FontStyle.Bold);
						System.Drawing.Font fontTexto = new System.Drawing.Font("Arial", 10, FontStyle.Regular);
						System.Drawing.Font fontTitulo1 = new System.Drawing.Font("Arial", 11, FontStyle.Bold);
						System.Drawing.Font fontTitulo2 = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
						System.Drawing.Font fontDestaque = new System.Drawing.Font("Arial", 18, FontStyle.Bold);
						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						// Definir pincéis
						Brush brushPreto = Brushes.Black;

						// Desenhar QR code (exemplo simples, ideal usar biblioteca externa para gerar um QR code real)
						// No exemplo estou apenas desenhando um quadrado como marcador de onde o QR code ficaria
						graph.FillRectangle(brushPreto, new System.Drawing.Rectangle(75, 75, 350, 350));
						var qrcode = GenerateQRCode(link, 350, 350);

						graph.DrawImage(qrcode, 75, 75, 350, 350);
						// Desenhar o cabeçalho da empresa
						graph.DrawString(ingresso.Lote.Evento.nomeEvento, fontTitulo1, brushPreto, new PointF(725, 110), stringFormat);
						graph.DrawString(ingresso.Lote.Evento.Endereco.logradouro + ", n" + ingresso.Lote.Evento.Endereco.numeroendereco + " - " + ingresso.Lote.Evento.Endereco.bairro, fontTexto, brushPreto, new PointF(725, 130), stringFormat);
						graph.DrawString(ingresso.Lote.Evento.Endereco.cidade + " - " + ingresso.Lote.Evento.Endereco.estado, fontTexto, brushPreto, new PointF(725, 147), stringFormat);
						graph.DrawString("CEP: " + ingresso.Lote.Evento.Endereco.cep, fontTexto, brushPreto, new PointF(725, 165), stringFormat);

						// Linha divisória
						graph.DrawLine(new Pen(Color.Black, 1), new Point(500, 210), new Point(950, 210));

						// Desenhar o título do cupom
						graph.DrawString("Ingresso", fontDestaque, brushPreto, new PointF(725, 240), stringFormat);
						graph.DrawString("Código: " + ingresso.ticketIngresso+"       Lote:"+ingresso.Lote.numLote, fontTitulo1, brushPreto, new PointF(725, 270), stringFormat);


						graph.DrawString("Data do Evento: " + ingresso.Lote.Evento.dataInicioEvento.ToString("MM/dd/yyyy HH:mm") + "    Data de Fim: " + ingresso.Lote.Evento.dataTerminoEvento.ToString("MM/dd/yyyy HH:mm"), fontTitulo1, brushPreto, new PointF(500, 310));

						// Informações do cliente
						graph.DrawString("Cliente: " + ingresso.NomeIngresso, fontTitulo2, brushPreto, new PointF(500, 345));
						graph.DrawString("Email: " + ingresso.EmailIngresso, fontTitulo2, brushPreto, new PointF(500, 375));


                        ImageCodecInfo myImageCodecInfo;
                        System.Drawing.Imaging.Encoder myEncoder;
                        EncoderParameter myEncoderParameter;
                        EncoderParameters myEncoderParameters;
                        myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bmp.Save(saveimg, myImageCodecInfo, myEncoderParameters);

						if (System.IO.File.Exists(saveimg))
						{
							return true;
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar Geraringresso: {ex}");
				return false;
			}
		}
		static System.Drawing.Image ResizeImage(Bitmap imagem)
        {
            Bitmap ImageFinal = imagem;
            ImageFinal.MakeTransparent();
            Color trasn = Color.FromArgb(0, 0, 0, 0);
            for (int x = 0; x < imagem.Width; x++)
            {
                for (int y = 0; y < imagem.Height; y++)
                {
                    Color pixelColor = imagem.GetPixel(x, y);
                    if (pixelColor == Color.White)
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fffffb"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fffeff"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fcfdff"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fffaf7"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fdffff"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fffefb"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#faffff"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fffdfd"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fdfdfd"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                    if (pixelColor == System.Drawing.ColorTranslator.FromHtml("#fefefe"))
                    {
                        ImageFinal.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            return ImageFinal;
        }
        public void copyarquivo(string sourceDirName, string destDirName)
        {

            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();

            foreach (var file in files)
            {
                // Create the path to the new copy of the file.
                var temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
            }

            // If copySubDirs is true, copy the subdirectories.

            foreach (var subdir in dirs)
            {
                // Create the subdirectory.
                var temppath = Path.Combine(destDirName, subdir.Name);

                // Copy the subdirectories.
                copyarquivo(subdir.FullName, temppath);
            }
        }
    }
}

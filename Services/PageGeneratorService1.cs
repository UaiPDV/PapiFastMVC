using BixWeb.Models;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using ZXing;
using System.Net.Mail;
using System.Net;
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using System.Text.Json;
using BixWeb.ViewModel;
using System.Text.RegularExpressions;

namespace BixWeb.Services
{
    public class PageGeneratorService1 : IPageGeneratorService1
    {
        private readonly DbPrint _context;
        private readonly string? _accessTokenGrokAPI;
        private readonly string? _accessTokenGeminiAPI ;

        // Injeção de dependência do DbContext
        public PageGeneratorService1(IConfiguration configuration,DbPrint context)
        {
            _context = context;
            _accessTokenGrokAPI = configuration["GrokAPI:AccessToken"];
            _accessTokenGeminiAPI = configuration["GeminiAPI:AccessAPI"];
        }

        public async Task<string> GerarImagens(string prompt)
        {
            try
            {
                string apiUrl = "https://api.x.ai/v1/images/generations";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessTokenGrokAPI}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json"); // Adicionando o Accept

                    var requestData = new
                    {
                        model = "grok-2-image",
                        prompt = "" + prompt
                    };

                    string jsonContent = JsonSerializer.Serialize(requestData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonResponse = JsonDocument.Parse(responseBody);
                        string imageUrl = jsonResponse.RootElement.GetProperty("data")[0].GetProperty("url").GetString();
                        if (string.IsNullOrWhiteSpace(imageUrl))
                        {
                            return "Erro";
                        }
                        else
                        {
                            return imageUrl;
                        }
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        ErrorViewModel.LogError($"Erro ao chamar GerarImagens: {response.StatusCode} . \nResposta: {errorResponse}");
                        return "Erro";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar GerarImagens: {ex}");
                return "Erro";
            }
            
        }
        public async Task<List<ViewModel.Presente>> ObterRecomendacoes(string pedido)
        {
            string prompt = $@"Busque recomendações de produtos com as seguintes características: {pedido}
            Retorne a resposta **somente** em formato de objeto JSON **válido**, sem explicações ou textos adicionais.
            Gostaria que para cada produto recomendado verificasse na web a média de preço para o preço
            para imagem do produto gostaria que ficasse vazio
            Formato esperado:
            [
                {{""nome"": ""string"", ""preco"": float, ""descricao"": ""string"", ""imagem"": ""null""}},
                ...
            ]";

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
                };

                string jsonRequestBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                string? ApiUrl = _accessTokenGeminiAPI;
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Desserializar a resposta da API Gemini
                GeminiResponse geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                // Extrair o conteúdo do campo "text"
                string jsonProdutos = geminiResponse.candidates[0].content.parts[0].text;

                // Remover o bloco `json e `
                jsonProdutos = Regex.Replace(jsonProdutos, "`json|`", "");

                List<ViewModel.Presente> produtos = CriarListaProdutos(jsonProdutos);

                foreach (ViewModel.Presente produto in produtos)
                {
                    //string promptImage = "Gere uma imagem do produto " + produto.Nome + " baseado na descrição " + produto.Descricao;
                    //produto.Imagem = await GerarImagens(promptImage); // Implementar a função de busca de imagens.
                    produto.Imagem=await ImageSearch.BuscarImagens(produto.Nome); // Implementar a função de busca de imagens.
                }

                return produtos;
            }
            catch (HttpRequestException ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ObterRecomendacoes: {ex}");
                return new List<ViewModel.Presente>();
            }
            catch (JsonException ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ObterRecomendacoes: {ex}");
                return new List<ViewModel.Presente>();
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ObterRecomendacoes: {ex}");
                return new List<ViewModel.Presente>();
            }
        }
        public async Task<string> ObterPromptImagem(string produto, string descricao,string valor)
        {
            string prompt = $@"Gostaria que escrevesse um prompt em  inglês para gerar a imagem de um anúncio. O anúcio seria do produto {produto}. Levar em consideração essa descrição: {descricao}. Deve também na imagem mostrar
            o nome '{produto}' e abaixo do nome a frase 'por apenas {valor}'. A saída de conter somente o prompt nesse formato:
                {{""prompt"": ""string""}}
            ";

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
                };

                string jsonRequestBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                string? ApiUrl = _accessTokenGeminiAPI;
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Desserializar a resposta da API Gemini
                GeminiResponse geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                // Extrair o conteúdo do campo "text"
                string jsonProdutos = geminiResponse.candidates[0].content.parts[0].text;

                // Remover o bloco `json e `
                jsonProdutos = Regex.Replace(jsonProdutos, "`json|`", "");

                return jsonProdutos;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            catch (JsonException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<ViewModel.Presente> CriarListaProdutos(string jsonData)
        {
            try
            {
                return JsonSerializer.Deserialize<List<ViewModel.Presente>>(jsonData);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao deserializar JSON: {ex.Message}");
                return new List<ViewModel.Presente>();
            }
        }
        public bool gerarCuponPDF(int id, string caminho,string link)
        {
            var cupon = _context.Cupons.Where(s => s.codCupom == id).Include(s => s.UsuarioFilial).ThenInclude(s => s.Filial).ThenInclude(s => s.Endereco).Include(s => s.usuario).Include(s=>s.Campanha).FirstOrDefault();
            if (cupon == null) 
            {
                return false;
            }
            try
            {
                if (cupon.produtos != null && cupon.produtos!="")
                {
                    string[] produto = cupon.produtos.Split(",");
                    cupon.produtos = "";
                    for (int i = 0; i < produto.Length-1; i++)
                    {
                        if (produto[i]=="")
                        {
                            cupon.produtos +=".";
                        }
                        else
                        {
                            int codProduto = int.Parse(produto[i]);
                            var dadosproduto = _context.Produtos.Find(codProduto);
                            if (dadosproduto!=null)
                            {
                                cupon.produtos += dadosproduto.nomeProduto + ", ";
                            }
                        }
                    }
                }
                string caminhoPdf = caminho+cupon.tokenCupom+"-"+cupon.codCupom.ToString()+".pdf";

                // Criar o documento PDF
                using (PdfWriter writer = new PdfWriter(caminhoPdf))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        // Configurar o tamanho da página
                        PageSize pageSize = new PageSize(600, 300);
                        Document document = new Document(pdf, pageSize);
                        PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        // Adicionar cabeçalho (nome da empresa)
                        Paragraph header = new Paragraph(cupon.UsuarioFilial.Filial.nome)
                            .SetFont(boldFont) // Define a fonte para negrito
                            .SetFontSize(12); ;
                        document.Add(header);

                        // Adicionar CNPJ e endereço
                        document.Add(new Paragraph("CNPJ: " + cupon.UsuarioFilial.Filial.cnpj));
                        document.Add(new Paragraph(cupon.UsuarioFilial.Filial.Endereco.logradouro + ", n" + cupon.UsuarioFilial.Filial.Endereco.numeroendereco + " - " + cupon.UsuarioFilial.Filial.Endereco.bairro));
                        document.Add(new Paragraph(cupon.UsuarioFilial.Filial.Endereco.cidade + " - " + cupon.UsuarioFilial.Filial.Endereco.estado));
                        document.Add(new Paragraph("CEP: " + cupon.UsuarioFilial.Filial.Endereco.cep));

                        // Adicionar linha divisória
                        LineSeparator ls = new LineSeparator(new SolidLine());
                        document.Add(ls);
                        
                        // Título "DESCONTO 20%"
                        Paragraph descontoTitulo = new Paragraph(cupon.tokenCupom)
                            .SetFontSize(18)
                            .SetFont(boldFont)
                            .SetFontColor(ColorConstants.BLACK);
                        document.Add(descontoTitulo);

                        // Adicionar subtítulo "Cupom de Desconto"
                        Paragraph subtitulo = new Paragraph("Cupom de Desconto")
                            .SetFontSize(14);
                        document.Add(subtitulo);

                        // Adicionar detalhes da campanha
                        if (cupon.Campanha != null)
                        {
                            document.Add(new Paragraph("Campanha: " + cupon.Campanha.nomeCampanha));

                        }
                        else
                        {
                            document.Add(new Paragraph("Campanha: ______________________________"));
                        }
                        document.Add(new Paragraph("Validade: " + cupon.validadeCupom.ToString("MM/dd/yyyy") + "    Valor do Desconto: " + cupon.descontoCupom));

                        // Adicionar informações do cliente
                        if (cupon.usuario != null)
                        {
                            document.Add(new Paragraph("Cliente: " + cupon.usuario.nome + "    CPF: " + cupon.usuario.cpf));
                            document.Add(new Paragraph("Email: " + cupon.usuario.nome + "    Contato: " + cupon.usuario.telefone));

                        }
                        else
                        {
                            document.Add(new Paragraph("Cliente: ______________________    CPF: ______________________"));
                            document.Add(new Paragraph("Email: ________________________    Contato: (   ) ____________"));
                        }
                        // Adicionar produtos aplicáveis
                        document.Add(new Paragraph("Produtos aplicáveis: " + cupon.produtos));

                        // Adicionar QR Code (caso você tenha a imagem)
                        ImageData imageData = ImageDataFactory.Create(GenerateQRCode(link, caminho+"qrcode-"+cupon.codCupom+".png")); // Caminho para a imagem do QR Code
                        iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imageData);
                        qrCode.SetFixedPosition(50, 100);
                        document.Add(qrCode);
                        // Fechar o documento
                        document.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ProdutosCardapio em GerarPDFCupons: {ex}");
                return false;
            }
        }
        public bool EnviarCupom(string caminho, string email, string nome)
        {
            try
            {
                SmtpClient client = new SmtpClient("mail.uaipdv.com.br"); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padrão para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential("uai@uaipdv.com.br", "M6433vlks*");
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage("uai@uaipdv.com.br", email);
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Você ganhou um cupom de desconto, Aproveite";
                mailMessage.Body = "Prezado "+nome+" abaixo anexado segue um cupom de desconto para utilizar em nosso estabelecimento, agradecemos a preferência!";
                mailMessage.Priority = MailPriority.High;

                if (File.Exists(caminho))
                {
                    // Criando o anexo
                    Attachment attachment = new Attachment(caminho);
                    attachment.Name = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes("Cupom.pdf"));
                    mailMessage.Attachments.Add(attachment);
                }
                client.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ProdutosCardapio em EnviarCupons: {ex}");
                return false;
            }
        }
        public string GenerateQRCode(string text, string caminho)
        {
            var qrCodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Height = 100,
                    Width = 100,
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
                bitmap.Save(caminho, System.Drawing.Imaging.ImageFormat.Png);
                return caminho;
            }
        }
    }
}

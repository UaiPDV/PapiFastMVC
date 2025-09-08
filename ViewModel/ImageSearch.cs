using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;
using Google.Apis.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BixWeb.ViewModel
{
    public class ImageSearch
    {
        private const string API_KEY = "AIzaSyBVRLHWrkK6TR_M1yolFt_bQOYaMUwltzA";
        private const string CSE_ID = "46128c4c06cf34eff";
        public static async Task<string> BuscarImagens(string descricao)
        {
            var service = new CustomSearchAPIService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var listRequest = service.Cse.List();
            listRequest.Q = descricao;
            listRequest.Cx = CSE_ID;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;

            var result = await listRequest.ExecuteAsync();

            if (result.Items != null)
            {
                using (var httpClient = new HttpClient())
                {
                    foreach (var item in result.Items)
                    {
                        string link = item.Link;
                        try
                        {
                            var response = await httpClient.GetAsync(link);
                            response.EnsureSuccessStatusCode(); // Lança uma exceção para códigos de status ruins
                            string contentType = response.Content.Headers.ContentType.ToString();
                            if (contentType.StartsWith("image/"))
                            {
                                return link; // Retorna o primeiro link de imagem válido
                            }
                        }
                        catch (HttpRequestException)
                        {
                            // Ignora imagens com erros
                        }
                    }
                }
            }
            return "Erro"; // Retorna null se nenhuma imagem válida for encontrada
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BixWeb.Models;

namespace BixWeb.ViewModel
{
    public class Presente
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("preco")]
        public double Preco { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("imagem")]
        public string Imagem { get; set; }

        public override string ToString()
        {
            return $"Nome: {Nome}, Preço: {Preco}, Descrição: {Descricao}, Imagem: {Imagem}";
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
        {
            { "nome", Nome },
            { "preco", Preco },
            { "descricao", Descricao },
            { "imagem", Imagem }
        };
        }
    }
    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
    }

    public class Candidate
    {
        public Content content { get; set; }
    }

    public class Content
    {
        public List<Part> parts { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class Recomendacoes
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "AIzaSyAvvc30bHRTS6kBVu40TGyXFMKlrHFmjLw";
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + ApiKey;

        public static List<Presente> CriarListaProdutos(string jsonData)
        {
            try
            {
                return JsonSerializer.Deserialize<List<Presente>>(jsonData);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao deserializar JSON: {ex.Message}");
                return new List<Presente>();
            }
        }

        public static string LimparJson(string texto)
        {
            Match match = Regex.Match(texto, @"\[.*\]", RegexOptions.Singleline);
            return match.Success ? match.Value : null;
        }

        public static async Task<List<Presente>> ObterRecomendacoes(string pedido)
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

                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Desserializar a resposta da API Gemini
                GeminiResponse geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                // Extrair o conteúdo do campo "text"
                string jsonProdutos = geminiResponse.candidates[0].content.parts[0].text;

                // Remover o bloco `json e `
                jsonProdutos = Regex.Replace(jsonProdutos, "`json|`", "");

                List<Presente> produtos = CriarListaProdutos(jsonProdutos);

                foreach (Presente produto in produtos)
                {
                    produto.Imagem = await ImageSearch.BuscarImagens(produto.Nome); // Implementar a função de busca de imagens.
                }

                return produtos;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro na chamada da API: {ex.Message}");
                return new List<Presente>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao serializar/desserializar JSON: {ex.Message}");
                return new List<Presente>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter recomendações: {ex.Message}");
                return new List<Presente>();
            }
        }

    }

}

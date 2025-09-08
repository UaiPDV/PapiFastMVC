using BixWeb.Models;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Caching.Memory;

namespace BixWeb.Services
{
    public class MercadoPagoService
    {
        
        private readonly string _accessToken;
        private readonly IMemoryCache _memoryCache; // Add this field

        public MercadoPagoService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _accessToken = configuration["MercadoPago:AccessToken"];
            MercadoPagoConfig.AccessToken = _accessToken;
            _memoryCache = memoryCache;
        }

        public async Task<string> CriarPreferenciaAsync(ReciboPresente recibo, string link)
        {
            var items = recibo.Presentes.Select(presente => new PreferenceItemRequest
            {
                Id = presente.codPresente.ToString(),
                Title = presente.Nome,
                CurrencyId = "BRL",
                PictureUrl = presente.Imagem,
                Description = presente.Descricao,
                CategoryId = "Presente",
                Quantity = 1,
                UnitPrice = (decimal)presente.Preco
            }).ToList();
            if (recibo.usuario!=null)
            {
                recibo.emailCliente = recibo.usuario.email;
                recibo.nomeCliente = recibo.usuario.nome;
                recibo.cpfCliente = recibo.usuario.cpf;
                recibo.telefoneCliente = recibo.usuario.telefone;
            }
            var nome = recibo.nomeCliente.Split(' ');

            string reciboId = Guid.NewGuid().ToString();
            _memoryCache.Set(reciboId, recibo, TimeSpan.FromHours(1));
            var request = new PreferenceRequest
            {
                Items = items,
                Payer = new PreferencePayerRequest
                {
                    Name = recibo.nomeCliente,
                    Surname = nome[1],
                    Email = recibo.emailCliente
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = link + "/ReciboPresentes/PresentesStatus/1?reciboId="+reciboId,
                    Failure = link + "/Eventos/Presentes?codConvite=" + recibo.codConvite,
                    Pending = link + "/ReciboPresentes/PresentesStatus/2?reciboId="+reciboId,
                },
                AutoReturn = "approved",
                NotificationUrl = link+ "/ReciboPresentes/NotificarPagamento/",
                ExternalReference = reciboId,
                Expires = true,
                ExpirationDateFrom = DateTime.UtcNow,
                ExpirationDateTo = DateTime.UtcNow.AddDays(3)
            };
            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            return preference.InitPoint; // URL para redirecionamento do checkout
        }
        public async Task<string> VendaIngressoAsync(ReciboIngresso recibo, string link, int codevento)
        {
            var items = recibo.ingressos.Select(ingresso => new PreferenceItemRequest
            {
                Id = ingresso.codIngresso.ToString(),
                Title = "Ingresso-"+ingresso.Lote.Evento.nomeEvento,
                CurrencyId = "BRL",
                PictureUrl = link+ "/imagens/ingresso.png",
                Description = "Ingressos para o evento "+ingresso.Lote.Evento.nomeEvento,
                CategoryId = "Presente",
                Quantity = 1,
                UnitPrice = (decimal)ingresso.Lote.valorLote
            }).ToList();

            if (recibo.usuario != null)
            {
                recibo.emailCliente = recibo.usuario.email;
                recibo.nomeCliente = recibo.usuario.nome;
                recibo.cpfCliente = recibo.usuario.cpf;
                recibo.telefoneCliente = recibo.usuario.telefone;
            }
            var nomeParts = recibo.nomeCliente?.Split(' ') ?? Array.Empty<string>();
            string reciboId = Guid.NewGuid().ToString();
            _memoryCache.Set(reciboId, recibo, TimeSpan.FromHours(1));
            var request = new PreferenceRequest
            {
                Items = items,
                Payer = new PreferencePayerRequest
                {
                    Name = recibo.nomeCliente,
                    Surname = nomeParts.Length > 1 ? nomeParts[1] : "",
                    Email = recibo.emailCliente
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = link + "/Ingressos/IngressosStatus/1?reciboId=" + reciboId,
                    Failure = link + "/Ingressos/Venda/" + codevento,
                    Pending = link + "/Ingressos/IngressosStatus/2?reciboId=" + reciboId,
                },
                AutoReturn = "approved",
                NotificationUrl = link + "/Ingressos/NotificarPagamento/",
                ExternalReference = reciboId,
                Expires = true,
                ExpirationDateFrom = DateTime.UtcNow,
                ExpirationDateTo = DateTime.UtcNow.AddDays(3)
            };
            try
            {
                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);
                return preference.InitPoint; // URL para redirecionamento do checkout
                // Use a preferência retornada (ex: preference.InitPoint)
            }
            catch (Exception ex)
            {
                // Trate o erro adequadamente
                ErrorViewModel.LogError($"Erro ao chamar VendaMercadopagoIngresso: {ex}");
                return "Error"; // Retorne uma string vazia ou trate o erro de outra forma
            }
        }

        public async Task<string> CriarPreferenciaAsyncOri()
        {
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Id = "item-ID-1234",
                    Title = "Meu produto",
                    CurrencyId = "BRL",
                    PictureUrl = "https://www.mercadopago.com/org-img/MP3/home/logomp3.gif",
                    Description = "Descrição do Item",
                    CategoryId = "art",
                    Quantity = 1,
                    UnitPrice = 75.76m
                }
            },
                Payer = new PreferencePayerRequest
                {
                    Name = "João",
                    Surname = "Silva",
                    Email = "user@email.com"
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://www.success.com",
                    Failure = "http://www.failure.com",
                    Pending = "http://www.pending.com"
                },
                AutoReturn = "approved",
                NotificationUrl = "https://www.your-site.com/ipn",
                ExternalReference = "Reference_1234",
                Expires = true,
                ExpirationDateFrom = DateTime.UtcNow,
                ExpirationDateTo = DateTime.UtcNow.AddDays(7)
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            return preference.InitPoint; // URL para redirecionamento do checkout
        }
    }
}

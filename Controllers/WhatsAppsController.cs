using BixWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace BixWeb.Controllers
{
    public class WhatsAppsController : Controller
    {
        private readonly DbPrint _context;
        private readonly HttpClient _httpClient;
        public WhatsAppsController(DbPrint context)
        {
            _httpClient = new HttpClient();
            _context = context;
            _httpClient.BaseAddress = new Uri("http://18.231.120.86:8080/");
        }
        public class StatusResponse
        {
            public string status { get; set; }
        }
        public async Task<IActionResult> Index()
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null) 
            {
                int codUsuario = int.Parse(userId);
                if (User.IsInRole("Gerente"))
                {
                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == codUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();

                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");
                }
                else if (User.IsInRole("Funcionario"))
                {
                    var usuarioFiliais = _context.UsuarioFiliais.Where(s => s.codUsuario == codUsuario).Include(s => s.Filial).FirstOrDefault();
                    if (usuarioFiliais != null)
                        ViewBag.Sessao = "filial-" + usuarioFiliais.codFilial;
                        ViewBag.status = VerificarStatusSessaoAsync("filial-" + usuarioFiliais.codFilial).Result;
                }
                else
                {
                    ViewBag.status = VerificarStatusSessaoAsync("usuario-" + userId).Result;
                    ViewBag.Sessao = "usuario-" + userId;
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult VerificarSessao(string id)
        {
            var whatsApp = VerificarStatusSessaoAsync(id).Result;
            
            return Json(whatsApp);
        }
        public async Task<string> VerificarStatusSessaoAsync(string nomeSessao)
        {
            try
            {
                // Requisição GET para a API
                var response = await _httpClient.GetAsync($"status-sessao/{nomeSessao}");

                // Verifica se a requisição foi bem-sucedida
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Tenta deserializar o JSON para extrair o status
                    var resultado = JsonSerializer.Deserialize<StatusResponse>(content);
                    return resultado.status.ToString();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return $"Erro";
                }
                else
                {
                    return $"Não Conectado";

                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Sincronizar: {ex}");
                ViewData["Erro"] = "Erro ao sincronização.";
                return $"Não Conectado";
            }
        }

    }
}

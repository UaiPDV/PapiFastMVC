using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using MercadoPago.Resource.User;
using System.Security.Claims;
using MercadoPago.Client.Payment;
using X.PagedList.Extensions;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using BixWeb.Services;
using BixWeb.ViewModel;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.Extensions.Logging;
using BixWeb.Migrations;

namespace BixWeb.Controllers
{
    public class ReciboPresentesController : Controller
    {
        private readonly DbPrint _context;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly IMemoryCache _memoryCache;
        private readonly IPageGeneratorService _pageGeneratorService;

        public ReciboPresentesController(DbPrint context, IMemoryCache memoryCache, MercadoPagoService mercadoPagoService, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _mercadoPagoService = mercadoPagoService;
            _memoryCache = memoryCache;
            _pageGeneratorService = pageGeneratorService;
        }

        // GET: ReciboPresentes
        [Authorize]
        public async Task<IActionResult> Index(int? page)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int CodUsuario = int.Parse(userId);
            try
            {
                if (User.IsInRole("Gerente") || User.IsInRole("Funcionario") || User.IsInRole("Administrador"))
                {
                    var usuarioFilial = await _context.UsuarioFiliais.Where(s => s.codUsuario == CodUsuario).Include(s => s.Filial).ToListAsync();
                    var recibosPresentes = new List<ReciboPresente>();
                    var eventosFiliais = new List<Evento>();
                    foreach (var item in usuarioFilial)
                    {
                        var eventos = await _context.Eventos.Where(s => s.codFilial == item.codFilial).Include(s => s.Convites).ThenInclude(s => s.recibosPresentes).ThenInclude(s => s.Presentes).ToListAsync();
                        eventosFiliais.AddRange(eventos);
                        foreach (var Evento in eventos)
                        {
                            foreach (var convite in Evento.Convites)
                            {
                                recibosPresentes = recibosPresentes.Union(convite.recibosPresentes).ToList();
                            }
                        }
                    }
                    ViewBag.Eventos = new SelectList(eventosFiliais, "codEvento", "nomeEvento");

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    var recibos = recibosPresentes.OrderBy(s => s.dataCriacao).ToPagedList(pageNumber, pageSize);
                    return View(recibos);
                }
                else
                {
                    var usuario = await _context.Usuario.Include(s => s.Convites).ThenInclude(s => s.recibosPresentes).ThenInclude(s => s.Presentes).Include(s => s.recibosPresentes).ThenInclude(s => s.Presentes).FirstOrDefaultAsync(u => u.codUsuario == CodUsuario);
                    List<ReciboPresente> recibosPresentes = usuario.recibosPresentes.ToList();
                    List<Evento> eventos = new List<Evento>();
                    if (usuario.recibosPresentes.Any())
                    {
                        foreach (var recibo in usuario.recibosPresentes)
                        {
                            eventos.Add(_context.Convites.Where(s => s.codConvite == recibo.codConvite).Include(s => s.Evento).FirstOrDefault()?.Evento);

                        }
                    }
                    foreach (Convite convite in usuario.Convites)
                    {
                        eventos.Add(_context.Eventos.Where(s => s.codEvento == convite.codEvento).FirstOrDefault());

                        if (convite.recibosPresentes != null)
                        {
                            recibosPresentes.AddRange(convite.recibosPresentes);
                        }
                    }
                    ViewBag.Eventos = new SelectList(eventos, "codEvento", "nomeEvento");
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    var recibos = recibosPresentes.OrderBy(s => s.dataCriacao).ToPagedList(pageNumber, pageSize);
                    return View(recibos);
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexEventos: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            
        }
        [HttpGet]
        public IActionResult Convidados(int id)
        {
            Evento evento = _context.Eventos.Include(s => s.Convites).ThenInclude(s => s.Convidados).FirstOrDefault(s => s.codEvento == id);
            // Aqui você consulta seu banco de dados ou serviço para obter os números
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int CodUsuario = int.Parse(userId);
            if (userId!= null && CodUsuario>0)
            {
                foreach (var convite in evento.Convites)
                {
                    if (convite.codCliente==CodUsuario)
                    {

                        var dados = new
                        {
                            total = convite.Convidados.Count(),
                            pendente = convite.Convidados.Where(s => s.confirmacaoConvite == "Aguardando").Count(),
                            recusado = convite.Convidados.Where(s => s.confirmacaoConvite == "Recusado").Count(),
                            confirmado = convite.Convidados.Where(s => s.confirmacaoConvite == "Confirmado").Count()

                        };
                        return Json(dados);
                    }
                }
            }
            var erro = new
            {
                Error = "Erro"

            };
            return Json(erro);
        }
        // GET: ReciboPresentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reciboPresente = await _context.RecibosPresentes
                .Include(r => r.usuario).Include(r=>r.convite).ThenInclude(s=>s.Evento).Include(s=>s.Presentes)
                .FirstOrDefaultAsync(m => m.codReciboPresente == id);
            if (reciboPresente == null)
            {
                return NotFound();
            }

            return View(reciboPresente);
        }

        // GET: ReciboPresentes/Create
        public IActionResult Create()
        {
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "email");
            return View();
        }

        // POST: ReciboPresentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codReciboPresente,nomeCliente,cpfCliente,emailCliente,telefoneCliente,codUsuario,codConvite")] ReciboPresente reciboPresente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reciboPresente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "email", reciboPresente.codUsuario);
            return View(reciboPresente);
        }
        public ActionResult ValidarToken(string email)
        {
            ViewBag.Email = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ValidarToken(string email, string token)
        {
            try
            {
                if (_memoryCache.TryGetValue(email, out string tokenCache))
                {
                    if (token == tokenCache)
                    {
                        _memoryCache.TryGetValue(token, out PresenteFormularioViewModel presenteFormulario);
                        if (presenteFormulario == null)
                        {
                            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                            return View("Error");
                        }
                        _memoryCache.Remove(email); // Remove o token após a validação
                        _memoryCache.Remove(token); // Remove o token após a validação
                        return await Pagamento(presenteFormulario);
                    }
                }
                ViewBag.ErrorMessage = "Token inválido ou expirado. Por favor, solicite um novo token.";
                ViewBag.Email = email;
                return View();
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ValidarToken: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public async Task<IActionResult> Pagamento(PresenteFormularioViewModel presenteFormulario)
        {
            if (string.IsNullOrWhiteSpace(presenteFormulario.Email) || string.IsNullOrWhiteSpace(presenteFormulario.Nome) || string.IsNullOrWhiteSpace(presenteFormulario.CPF))
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            ReciboPresente reciboPresente = new ReciboPresente()
            {
                cpfCliente = presenteFormulario.CPF,
                emailCliente = presenteFormulario.Email,
                nomeCliente = presenteFormulario.Nome,
                telefoneCliente = presenteFormulario.Telefone,
                status = "Pendente",
            };
            
            reciboPresente.codConvite = presenteFormulario.codConvite;
            if (presenteFormulario.CodPresenteSelecionados.Count() > 0)
            {
                List<Models.Presente> presentes = new List<Models.Presente>();
                foreach (var item in presenteFormulario.CodPresenteSelecionados)
                {
                    Models.Presente presente = _context.Presentes.Find(item);
                    presentes.Add(presente);
                }
                reciboPresente.Presentes = presentes;
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                var url = await _mercadoPagoService.CriarPreferenciaAsync(reciboPresente, baseUrl);

                return Redirect(url);
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NotificarPagamento()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(json);

            if (data.type == "payment")
            {
                string pagamentoId = data.data.id;

                var client = new PaymentClient();
                var pagamento = await client.GetAsync(long.Parse(pagamentoId));

                if (pagamento.Status == "approved" || pagamento.Status == "pending")
                {
                    // Pegue o ExternalReference que você passou antes
                    var reciboId = pagamento.ExternalReference;

                    // Recupere o objeto ReciboPresente
                    if (_memoryCache.TryGetValue(reciboId, out ReciboPresente recibo))
                    {
                        // Salvar no banco
                        _context.RecibosPresentes.Add(recibo);
                        await _context.SaveChangesAsync();
                        _memoryCache.Remove(reciboId); // Remover o recibo do cache após salvar
                    }
                }
            }

            return Ok();
        }

        // GET: ReciboPresentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reciboPresente = await _context.RecibosPresentes.FindAsync(id);
            if (reciboPresente == null)
            {
                return NotFound();
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "email", reciboPresente.codUsuario);
            return View(reciboPresente);
        }
        // GET: ReciboPresentes/PresentesStatus/1
        public async Task<IActionResult> PresentesStatus(int id,string reciboId)
        {
            if (_memoryCache.TryGetValue(reciboId, out ReciboPresente recibo))
            {
                ViewBag.local = "Presente Status";
                if (recibo!=null)
                {
                    if (id==1)
                    {
                        recibo.status = "Aprovado";
                        ViewBag.MSG0 = "1";
                        ViewBag.MSG = "Seu pagamento foi concluído com sucesso!";
                        ViewBag.MSG1 = "Estamos enviando agora mesmo os presentes.";

                        var usuario = await _context.Usuario.FindAsync(recibo.codUsuario);
                        if (usuario!=null)
                        {
                            if (recibo.convite.notificar == 1)
                            {
                                _pageGeneratorService.EnviarNotPresente(usuario.email, usuario.nome, recibo);
                            } else if (recibo.convite.notificar==2) 
                            {
                                _pageGeneratorService.EnviarNotPresenteWhats(usuario.token, usuario.telefone, usuario.nome, recibo);
                            }
                        }

                    }
                    else if(id==2)
                    {
                        recibo.status = "Pendente";
                        ViewBag.MSG0 = "2";
                        ViewBag.MSG = "Seu pagamento está em andamento!";
                        ViewBag.MSG1 = "Assim que concluído enviamos os presentes.";
                    }
                    else
                    {
                        ViewBag.MSG = "Que pena mas houve algum problema em seu pagamento!";
                        return View();
                    }
                    if (recibo.Presentes != null)
                    {
                        foreach (var presente in recibo.Presentes)
                        {
                            _context.Attach(presente);
                        }
                    }
                    // Salvar no banco
                    _context.RecibosPresentes.Add(recibo);
                    await _context.SaveChangesAsync();
                    _memoryCache.Remove(reciboId); // Remover o recibo do cache após salvar
                }
            }
            return View();
        }

        // POST: ReciboPresentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codReciboPresente,nomeCliente,cpfCliente,emailCliente,telefoneCliente,codUsuario,codConvite")] ReciboPresente reciboPresente)
        {
            if (id != reciboPresente.codReciboPresente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reciboPresente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReciboPresenteExists(reciboPresente.codReciboPresente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "email", reciboPresente.codUsuario);
            return View(reciboPresente);
        }

        // GET: ReciboPresentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reciboPresente = await _context.RecibosPresentes
                .Include(r => r.usuario)
                .FirstOrDefaultAsync(m => m.codReciboPresente == id);
            if (reciboPresente == null)
            {
                return NotFound();
            }

            return View(reciboPresente);
        }

        // POST: ReciboPresentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reciboPresente = await _context.RecibosPresentes.FindAsync(id);
            if (reciboPresente != null)
            {
                _context.RecibosPresentes.Remove(reciboPresente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReciboPresenteExists(int id)
        {
            return _context.RecibosPresentes.Any(e => e.codReciboPresente == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using X.PagedList.Extensions;
using System.Runtime.InteropServices;
using BixWeb.Services;
using BixWeb.ViewModel;
using static iTextSharp.text.pdf.AcroFields;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.StyledXmlParser.Jsoup.Nodes;
using MercadoPago.Resource.User;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using MercadoPago.Client.Payment;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using BixWeb.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace BixWeb.Controllers
{
    public class IngressosController : Controller
    {
        private readonly DbPrint _context;
		private readonly IPageGeneratorService _pageGeneratorService;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly IMemoryCache _memoryCache;

        public IngressosController(DbPrint context, IMemoryCache memoryCache, IPageGeneratorService pageGeneratorService, MercadoPagoService mercadoPagoService)
		{
			_context = context;
			_pageGeneratorService = pageGeneratorService;
            _mercadoPagoService = mercadoPagoService;
            _memoryCache = memoryCache;
        }

		// GET: Ingressos
		public IActionResult Index(int id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;


                var ingresso = _context.Ingressos.Where(s => s.Lote.codEvento == id).Include(s => s.Lote).Include(s=>s.ReciboIngresso).AsQueryable();
                ViewBag.Venda = ingresso.Where(s=>s.tipoVendaIngresso=="Não vendido!").Count();
                
                if (!ingresso.Any())
                {
                    ViewData["ingre"] = "Nenhuma ingresso encontrado!";
                    return View();
                }


                if (!String.IsNullOrEmpty(searchString))
                {
                    ingresso = ingresso.Where(s => s.ticketIngresso.Contains(searchString)
                                            || s.NomeIngresso.Contains(searchString)
                                            || s.EmailIngresso.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ingresso = ingresso.OrderByDescending(s => s.ticketIngresso);
                        ingresso = ingresso.OrderByDescending(s => s.Lote.numLote);
                        break;
                    default:  // Name ascending 
                        ingresso = ingresso.OrderBy(s => s.ticketIngresso);
                        ingresso = ingresso.OrderBy(s => s.Lote.numLote);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewBag.codEvento = id;
                return View(ingresso.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexIngressos: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // GET: Ingressos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresso = await _context.Ingressos
                .Include(i => i.Lote)
                .FirstOrDefaultAsync(m => m.codIngresso == id);
            if (ingresso == null)
            {
                return NotFound();
            }

            return View(ingresso);
        }
        public IActionResult Enviar(int id, string email, string nome, string telefone) 
        {
            var ingresso = _context.Ingressos.Find(id);
            if (ingresso== null || email=="")
            {
				return RedirectToAction("Index", new { formSubmitted = false });
            }
            ingresso.Lote = _context.Lotes.Where(s=>s.codLote==ingresso.codLote).Include(s=>s.Evento).FirstOrDefault();
            string diretorio = Directory.GetCurrentDirectory();
            string link = $"{this.Request.Scheme}://{this.Request.Host}";
            if (ingresso.Lote.Evento.codFilial == null)
            {
                diretorio = diretorio + "/wwwroot/Usuarios/" + ingresso.Lote.Evento.codUsuario + "/Eventos/" + ingresso.Lote.codEvento + "/Ingressos/";
                link += "/Ingresso/Validar/" + id + "?usuario=" + ingresso.Lote.Evento.codUsuario;
            }
            else
            {
                diretorio = diretorio + "/Empresas/" + ingresso.Lote.Evento.codFilial + "/Eventos/" + ingresso.Lote.codEvento + "/Ingressos/";
                link += "/Ingresso/Validar/" + id + "?filial=" + ingresso.Lote.Evento.codFilial;
            }
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            if (_pageGeneratorService.GerarIngresso(id,diretorio,link))
            {
                if (_pageGeneratorService.EnviarIngresso(diretorio+ingresso.codIngresso+".jpg",email, nome))
                {
					return RedirectToAction("Index", new { formSubmitted = true });
				}
            }
			return RedirectToAction("Index", new { formSubmitted = false });
		}
        public async Task<IActionResult> EnviarWhats(int id, string email, string nome, string telefone)
        {
            var ingresso = _context.Ingressos.Find(id);
            if (ingresso == null || email == "")
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            ingresso.Lote = _context.Lotes.Where(s => s.codLote == ingresso.codLote).Include(s => s.Evento).FirstOrDefault();
            string diretorio = Directory.GetCurrentDirectory();
            string link = $"{this.Request.Scheme}://{this.Request.Host}";
            if (ingresso.Lote.Evento.codFilial==null)
            {
                diretorio = diretorio + "/wwwroot/Usuarios/" + ingresso.Lote.Evento.codUsuario + "/Eventos/"+ingresso.Lote.codEvento+"/Ingressos/";
                link += "/Ingresso/Validar/" + id + "?usuario=" + ingresso.Lote.Evento.codUsuario;
            }
            else
            {
                diretorio = diretorio + "/Empresas/" + ingresso.Lote.Evento.codFilial + "/Eventos/"+ingresso.Lote.codEvento+"/Ingressos/";
                link += "/Ingresso/Validar/" + id + "?filial=" + ingresso.Lote.Evento.codFilial;
            }
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            if (_pageGeneratorService.GerarIngresso(id, diretorio, link))
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return RedirectToAction("Index", new { formSubmitted = false });
                }
                int CodUsuario = int.Parse(userId);
                var mensagem = new MensagemWhats();

                string usuarioToken = _context.Usuario.Where(s => s.codUsuario == CodUsuario).Select(s => s.token).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(usuarioToken))
                {
                    return RedirectToAction("Index", new { formSubmitted = false });
                }
                mensagem.token = usuarioToken;

                mensagem.message = "Este é seu ingresso para participar do evento: "+ingresso.Lote.Evento.nomeEvento;
                mensagem.message+="\nMais informações sobre o evento em: "+ingresso.Lote.Evento.linkEvento;
                mensagem.number = "55" + telefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                link = $"{this.Request.Scheme}://{this.Request.Host}";
                if (ingresso.Lote.Evento.codFilial==null)
                {
                    mensagem.link = link + "/Usuarios/" + ingresso.Lote.Evento.codUsuario + "/Eventos/"+ingresso.Lote.codEvento+"/Ingressos/" + ingresso.ticketIngresso + ".jpg";
                }
                else
                {
                    mensagem.link = link + "/Empresas/" + ingresso.Lote.Evento.codFilial + "/Eventos/"+ingresso.Lote.codEvento+"/Ingressos/" + ingresso.ticketIngresso + ".jpg";
                }
                if (await _pageGeneratorService.EnviarIngressoWhats(mensagem))
                {
                    return RedirectToAction("Index", new { formSubmitted = true });
                }
            }
            return RedirectToAction("Index", new { formSubmitted = false });
        }
        public IActionResult Venda(int id) 
        {
            
            var ingresso= _context.Ingressos.Where(s=>s.Lote.Evento.codEvento==id && s.tipoVendaIngresso== "Não vendido!")
                .Include(s=>s.Lote).ThenInclude(s=>s.Evento).FirstOrDefault();
            ViewBag.qtdade = _context.Ingressos.Where(s => s.Lote.codLote==ingresso.codLote && s.tipoVendaIngresso == "Não vendido!")
                .Include(s => s.Lote).ThenInclude(s => s.Evento).Count();
            ViewBag.url = "";
            return View(ingresso);
        }
        [HttpPost]
        public async Task<IActionResult> Venda(ReciboIngresso reciboIngresso,int quantidade,int lote, int codEvento)
        {
            bool hasError = false;
            try
            {
                if (string.IsNullOrWhiteSpace(reciboIngresso.emailCliente))
                {
                    ModelState.AddModelError("ReciboIngresso.emailCliente", "Você deve preencher o email para continuar.");
                    hasError = true;
                }
                if (string.IsNullOrWhiteSpace(reciboIngresso.nomeCliente))
                {
                    ModelState.AddModelError("ReciboIngresso.nomeCliente", "Você deve preencher o email para continuar.");
                    hasError = true;
                }
                if (string.IsNullOrWhiteSpace(reciboIngresso.cpfCliente))
                {
                    ModelState.AddModelError("ReciboIngresso.cpfCliente", "Você deve preencher o email para continuar.");
                    hasError = true;
                }
                if (hasError)
                {
                    var ingresso = _context.Ingressos.Where(s => s.Lote.Evento.codEvento == codEvento && s.tipoVendaIngresso == "Não vendido!")
                    .Include(s => s.Lote).ThenInclude(s => s.Evento).FirstOrDefault();
                    ViewBag.url = "";
                    return View(ingresso); // substitua pelo nome da sua View
                }
                else
                {
                    reciboIngresso.ingressos = _context.Ingressos.Where(s => s.Lote.codLote == lote && s.tipoVendaIngresso == "Não vendido!")
                        .Include(s => s.Lote).ThenInclude(s => s.Evento).Take(quantidade).ToList();

                    var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (userId != null)
                    {
                        int codUsuario = int.Parse(userId);
                        reciboIngresso.codUsuario = codUsuario;
                    }
                    foreach (var ingresso in reciboIngresso.ingressos)
                    {
                        if (User.IsInRole("Admin, Funcionario"))
                        {
                            ingresso.tipoVendaIngresso = "Venda Online por Filial";
                        }
                        else
                        {
                            ingresso.tipoVendaIngresso = "Venda Online";
                        }
                    }
                    reciboIngresso.status = "Pendente";
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    string url = await _mercadoPagoService.VendaIngressoAsync(reciboIngresso, baseUrl, codEvento);
                    if (url=="Error")
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                    else
                    {
                        return Redirect(url);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar VendaIngresso: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // GET: Ingressos/
        [Authorize(Roles = "Cliente")]
        public IActionResult RecibosIngresso(int? page) 
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int CodUsuario = int.Parse(userId);
                if (CodUsuario>0)
                {
                    var recibos = _context.ReciboIngressos.Where(s => s.codUsuario == CodUsuario).Include(s=>s.ingressos).ThenInclude(s=>s.Lote).ThenInclude(s=>s.Evento).AsQueryable();
                    recibos = recibos.OrderBy(s => s.dataCriacao);
                    
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(recibos.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }

            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar RecibosIngresso: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public IActionResult ReciboIngresso(int id)
        {
            try
            {
                var ReciboIngresso = _context.ReciboIngressos.Where(s => s.codReciboIngresso == id).Include(s => s.ingressos).ThenInclude(s => s.Lote).ThenInclude(s => s.Evento).FirstOrDefault();
                if (ReciboIngresso == null) 
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada, Nenhum Recibo Encontrado";
                    return View("Error");
                } 
                else 
                {
                    return View(ReciboIngresso);
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Reciboigresso: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public async Task<IActionResult> IngressoStatus(int id, string reciboId) 
        {
            ViewBag.local = "Ingresso Status";
            if (_memoryCache.TryGetValue(reciboId, out ReciboIngresso recibo))
            {
                if (recibo != null)
                {
                    if (id == 1)
                    {
                        recibo.status = "Aprovado";
                        ViewBag.MSG0 = "1";
                        ViewBag.MSG = "Seu pagamento foi concluído com sucesso!";
                        ViewBag.MSG1 = "Enviamos o recibo com ingresso para seu email Cadastrado.";

                    }
                    else if (id == 2)
                    {
                        recibo.status = "Pendente";
                        ViewBag.MSG0 = "2";
                        ViewBag.MSG = "Seu pagamento está em andamento!";
                        ViewBag.MSG1 = "Assim que concluído enviamos os ingressos.";
                    }
                    else
                    {
                        ViewBag.MSG = "Que pena mas houve algum problema em seu pagamento!";
                        return View();
                    }
                    if (recibo.ingressos != null)
                    {
                        foreach (var presente in recibo.ingressos)
                        {
                            _context.Attach(presente);
                        }
                    }
                    // Salvar no banco
                    _context.ReciboIngressos.Add(recibo);
                    await _context.SaveChangesAsync();
                    _memoryCache.Remove(reciboId); // Remover o recibo do cache após salvar
                }
            }
            return View();
        }
        public IActionResult CadastrarIngresso(int id)
        {
            ReciboIngresso recibos = _context.ReciboIngressos.Where(s => s.codReciboIngresso == id).Include(s => s.ingressos).FirstOrDefault();
            if (recibos==null)
            {
                ViewData["Error"] = "Ops! Houve um erro não encontramos nenhum ingresso";
                return View("Error");
            }
            return View(recibos);
        }
        [HttpPost]
        public IActionResult CadastrarIngresso(int id, string[] name, string[] email, string[] ticket, string[] cpf)
        {
            //var recibos = _context.ReciboIngressos.Where(s => s.codReciboIngresso == id).Include(s => s.ingressos).FirstOrDefault();
            bool isErro = false;
            for (int i = 0; i < ticket.Count(); i++)
            {
                if (!string.IsNullOrEmpty(email[i])  && !string.IsNullOrEmpty(email[i]) && !string.IsNullOrEmpty(cpf[i]) && !string.IsNullOrEmpty(ticket[i]))
                {
                    var ingresso = _context.Ingressos.Where(s => s.ticketIngresso == ticket[i]).FirstOrDefault();
                    if (ingresso != null)
                    {
                        ingresso.EmailIngresso = email[i];
                        ingresso.NomeIngresso = name[i];
                        _context.Ingressos.Update(ingresso);
                        _context.SaveChanges();
                        string diretorio = Directory.GetCurrentDirectory();
                        string link = $"{this.Request.Scheme}://{this.Request.Host}";

                        if (ingresso.Lote.Evento.codFilial==null)
                        {
                            diretorio = diretorio + "/wwwroot/Usuarios/" + ingresso.Lote.Evento.codUsuario + "/Eventos/" + ingresso.Lote.Evento.codEvento + "/Ingressos/";
                            link += "/Ingresso/Validar/" + ingresso.codIngresso + "?usuario=" + ingresso.Lote.Evento.codUsuario;
                        }
                        else
                        {
                            diretorio = diretorio + "/Empresas/" + ingresso.Lote.Evento.codFilial + "Eventos/"+ingresso.Lote.codEvento+"/Ingressos/";
                            link += "/Ingresso/Validar/" + ingresso.codIngresso + "?filial=" + ingresso.Lote.Evento.codFilial;
                        }

                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }

                        if (_pageGeneratorService.GerarIngresso(ingresso.codIngresso, diretorio, link))
                        {
                            if (!_pageGeneratorService.EnviarIngresso(diretorio + ingresso.codIngresso + ".jpg", ingresso.EmailIngresso, ingresso.NomeIngresso))
                            {
                                isErro = true;
                            }
                        }
                    }
                }
            }
            if (isErro)
            {
                ViewData["Error"] = "Ops! Houve um erro ao enviar os ingressos solicitados";
                return View("Error");
            }
            else
            {
                return RedirectToAction("CadastrarIngresso", new { id = id });
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
                    if (_memoryCache.TryGetValue(reciboId, out ReciboIngresso recibo))
                    {
                        if (recibo !=null)
                        {
                            // Salvar no banco
                            _context.ReciboIngressos.Add(recibo);
                            await _context.SaveChangesAsync();
                            _memoryCache.Remove(reciboId); // Remover o recibo do cache após salvar
                        }
                    }
                }
            }

            return Ok();
        }
        public IActionResult Create()
        {
            ViewData["codLote"] = new SelectList(_context.Lotes, "codLote", "codLote");
            return View();
        }

        // POST: Ingressos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codIngresso,ticketIngresso,NomeIngresso,EmailIngresso,tipoVendaIngresso,ativoIngresso,codLote")] Ingresso ingresso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingresso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["codLote"] = new SelectList(_context.Lotes, "codLote", "codLote", ingresso.codLote);
            return View(ingresso);
        }

        // GET: Ingressos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresso = await _context.Ingressos.FindAsync(id);
            if (ingresso == null)
            {
                return NotFound();
            }
            ViewData["codLote"] = new SelectList(_context.Lotes, "codLote", "codLote", ingresso.codLote);
            return View(ingresso);
        }

        // POST: Ingressos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codIngresso,ticketIngresso,NomeIngresso,EmailIngresso,tipoVendaIngresso,ativoIngresso,codLote")] Ingresso ingresso)
        {
            if (id != ingresso.codIngresso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingresso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngressoExists(ingresso.codIngresso))
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
            ViewData["codLote"] = new SelectList(_context.Lotes, "codLote", "codLote", ingresso.codLote);
            return View(ingresso);
        }

        // GET: Ingressos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresso = await _context.Ingressos
                .Include(i => i.Lote)
                .FirstOrDefaultAsync(m => m.codIngresso == id);
            if (ingresso == null)
            {
                return NotFound();
            }

            return View(ingresso);
        }

        // POST: Ingressos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingresso = await _context.Ingressos.FindAsync(id);
            if (ingresso != null)
            {
                _context.Ingressos.Remove(ingresso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngressoExists(int id)
        {
            return _context.Ingressos.Any(e => e.codIngresso == id);
        }
    }
}

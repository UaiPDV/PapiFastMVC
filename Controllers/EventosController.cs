using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using X.PagedList.Extensions;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BixWeb.Services;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using BixWeb.ViewModel;
using BixWeb.Migrations;
using Microsoft.Extensions.Caching.Memory;
using iText.Layout.Element;
using System.Net.Mail;
using System.Net;

namespace BixWeb.Controllers
{
    public class EventosController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;
        private readonly IPageGeneratorService1 _pageGeneratorService1;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly IMemoryCache _memoryCache;

        public EventosController(DbPrint context, IMemoryCache memoryCache, IPageGeneratorService pageGeneratorService, IPageGeneratorService1 pageGeneratorService1, MercadoPagoService mercadoPagoService)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
            _pageGeneratorService1 = pageGeneratorService1;
            _mercadoPagoService = mercadoPagoService;
            _memoryCache = memoryCache;
        }

        [Authorize]
        // GET: Eventos
        public IActionResult Lista(string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = System.String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != null)
                {
                    int codUsuario = int.Parse(userId);
                    var eventos = _context.Eventos.Where(s => s.UsuarioFilial.codUsuario == codUsuario || s.codUsuario==codUsuario);
                    if (eventos.Any())
                    {
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            eventos = eventos.Where(s => s.nomeEvento.Contains(searchString)
                                                       || s.descricaoEvento.Contains(searchString));

                        }

                        switch (sortOrder)
                        {
                            case "name_desc":
                                eventos = eventos.OrderByDescending(s => s.nomeEvento);
                                break;
                            default:  // Name ascending 
                                eventos = eventos.OrderBy(s => s.nomeEvento);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(eventos.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["Error"] = "Nenhum evento encontrado!";
                        return View();
                    }

                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexEventos: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = System.String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;
                var eventos = _context.Eventos.Where(s => s.statusEvento == true && s.eventoPrivado==false).Include(s => s.Lotes).ThenInclude(s => s.Ingressos).AsEnumerable();
             
                if (eventos.Any())
                {
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        eventos = eventos.Where(s => s.nomeEvento.Contains(searchString)
                                                   || s.descricaoEvento.Contains(searchString));

                    }

                    switch (sortOrder)
                    {
                        case "name_desc":
                            eventos = eventos.OrderByDescending(s => s.dataCriacaoEvento);
                            break;
                        default:  // Name ascending 
                            eventos = eventos.OrderBy(s => s.dataCriacaoEvento);
                            break;
                    }

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(eventos.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    ViewData["Error"] = "Nenhum evento encontrado!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexEventos: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public PartialViewResult DetailsPartial()
        {
            return PartialView(_context.Eventos.OrderByDescending(e => e.dataCriacaoEvento).Take(4).ToList());
        }
        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(string idOrSlug)
        {
            if (idOrSlug == null)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }

            if (int.TryParse(idOrSlug, out int id))
            {
                // Busca o evento pelo ID
                var evento = await _context.Eventos
                 .Include(e => e.Endereco)
                 .Include(e => e.UsuarioFilial)
                 .Include(e => e.Lotes).ThenInclude(s => s.Ingressos)
                 .FirstOrDefaultAsync(m => m.codEvento == id);

                if (evento == null)
                {
                    return View("Error");
                }

                var verificar = VerificarLotesDisponiveis(evento.Lotes.ToList());
                if (verificar != null)
                {
                    ViewBag.loteAtual = verificar.LoteAtual;
                    ViewBag.IngressoDisponivel = verificar.IngressosDisponiveisAtual;
                    ViewBag.ProximoLote = verificar.QuantidadeTotalProximoLote;
                    ViewBag.dataProximoLote = verificar.DataProximoLote;
                }

                return View(evento);
            }
            else
            {
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                baseUrl += "/Eventos/Details/" + idOrSlug;
                // Busca o evento pelo slug (string)
                var evento = await _context.Eventos.Where(e => e.linkEvento == baseUrl).Include(e => e.Endereco)
                 .Include(e => e.UsuarioFilial)
                 .Include(e => e.Lotes).ThenInclude(s => s.Ingressos)
                 .FirstOrDefaultAsync();

                if (evento == null)
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }

                var verificar = VerificarLotesDisponiveis(evento.Lotes.ToList());
                if (verificar != null) 
                {
                    ViewBag.loteAtual=verificar.LoteAtual;
                    ViewBag.IngressoDisponivel = verificar.IngressosDisponiveisAtual;
                    ViewBag.ProximoLote= verificar.QuantidadeTotalProximoLote;
                    ViewBag.dataProximoLote = verificar.DataProximoLote;
                }
                return View(evento);
            }

        }
        public LoteDisponivelInfo VerificarLotesDisponiveis(List<Lote> lotes)
        {
            var dataAtual = DateTime.Now;
            var resultado = new LoteDisponivelInfo();

            // Ordenar todos os lotes por data de venda
            var lotesOrdenados = lotes.OrderBy(l => l.dataVendaLote).ToList();

            // 1. Verificar lotes atuais (com data <= hoje)
            var lotesAtuais = lotesOrdenados.Where(l => l.dataVendaLote.Date <= dataAtual.Date).ToList();

            foreach (var lote in lotesAtuais)
            {
                if (lote.Ingressos == null) continue;

                var ingressosDisponiveis = lote.Ingressos
                    .Count(i => i.tipoVendaIngresso == "Não vendido!");

                if (ingressosDisponiveis > 0)
                {
                    resultado.LoteAtual = lote.numLote;
                    resultado.IngressosDisponiveisAtual = ingressosDisponiveis;
                    break;
                }
            }

            // 2. Verificar próximo lote (mesmo que tenha lote atual disponível)
            var proximoLote = lotesOrdenados
                .FirstOrDefault(l => l.dataVendaLote.Date > dataAtual.Date);

            if (proximoLote != null)
            {
                resultado.ProximoLote = proximoLote;
                resultado.DataProximoLote = proximoLote.dataVendaLote;
                resultado.QuantidadeTotalProximoLote = proximoLote.qtdIngLote; // Quantidade total do lote
            }

            return resultado;
        }
        public string Endereco(int id)
        {
            Filial filial = _context.Filiais.Where(s => s.codFilial == id).Include(s => s.Endereco).FirstOrDefault();
            
            string endereco = "<div class=\"col-md-2\">\r\n" +
                "                            <label class=\"form-label required\" for=\"Endereco_cep\">CEP:</label>\r\n" +
                "                            <input type=\"text\" class=\"form-control mb-2\" data-val=\"true\" data-val-required=\"O CEP é obrigatório.\" id=\"Endereco_cep\" name=\"Endereco.cep\" value=\" " + filial.Endereco.cep + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.cep\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-2\">\r\n" +
                "                            <label class=\"form-label required\" for=\"Endereco_estado\">Estado:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"text\" data-val=\"true\" data-val-length=\"O campo estado deve ser preenchido corretamente.\" data-val-length-max=\"150\" data-val-length-min=\"4\" id=\"Endereco_estado\" maxlength=\"150\" name=\"Endereco.estado\" value=\"" + filial.Endereco.estado + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.estado\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-4\">\r\n" +
                "                            <label class=\"form-label required\" for=\"Endereco_cidade\">Cidade:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"text\" data-val=\"true\" data-val-length=\"O campo cidade deve ser preenchido corretamente.\" data-val-length-max=\"150\" data-val-length-min=\"4\" id=\"Endereco_cidade\" maxlength=\"150\" name=\"Endereco.cidade\" value=\"" + filial.Endereco.cidade + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.cidade\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-4\">\r\n" +
                "                             <label class=\"form-label required\" for=\"Endereco_bairro\">Bairro:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"text\" data-val=\"true\" data-val-length=\"O campo bairro deve ser preenchido corretamente.\" data-val-length-max=\"150\" data-val-length-min=\"4\" id=\"Endereco_bairro\" maxlength=\"150\" name=\"Endereco.bairro\" value=\"" + filial.Endereco.bairro + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.bairro\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-5\">\r\n" +
                "                            <label class=\"form-label required\" for=\"Endereco_logradouro\">Rua/Avenida:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"text\" data-val=\"true\" data-val-length=\"O campo logradouro deve ser preenchido corretamente.\" data-val-length-max=\"150\" data-val-length-min=\"4\" id=\"Endereco_logradouro\" maxlength=\"150\" name=\"Endereco.logradouro\" value=\"" + filial.Endereco.logradouro + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.logradouro\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-2\">\r\n" +
                "                            <label class=\"form-label required\" for=\"Endereco_numeroendereco\">Numeral:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"number\" data-val=\"true\" data-val-required=\"Número do endereço deve ser preenchido.\" id=\"Endereco_numeroendereco\" name=\"Endereco.numeroendereco\" value=\"" + filial.Endereco.numeroendereco + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.numeroendereco\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n" +
                "                        <div class=\"col-md-5\">\r\n" +
                "                            <label class=\"form-label\" for=\"Endereco_refenciaEndereco\">Ponto de referencia:</label>\r\n" +
                "                            <input class=\"form-control mb-2\" type=\"text\" id=\"Endereco_refenciaEndereco\" name=\"Endereco.refenciaEndereco\" value=\"" + filial.Endereco.refenciaEndereco + "\">\r\n" +
                "                            <span class=\"fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback text-danger field-validation-valid\" data-valmsg-for=\"Endereco.refenciaEndereco\" data-valmsg-replace=\"true\"></span>\r\n" +
                "                        </div>\r\n";
            return endereco;
        }
        public ActionResult Cardapio(int id)
        {
            var evento = _context.Eventos.Where(s => s.codEvento == id).FirstOrDefault();
            ViewBag.codEvento = id;
            if (evento != null)
            {
                var preco = _context.Precos.Where(s => s.codFilial == evento.codFilial).Include(s => s.Produto).ThenInclude(s => s.Categoria);
                    return View(preco);
            }
            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
            return View("Error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cardapio(int id, int tipoCardapio, List<ProdutoCampanha> ProdutoCampanha) 
        {
            if (id !=0)
            {
                if (tipoCardapio>0)
                {
                    try
                    {
                        var evento = _context.Eventos.Where(s => s.codEvento == id).Include(s => s.UsuarioFilial).ThenInclude(s => s.Filial).FirstOrDefault();
                        ProdutoCampanha = ProdutoCampanha.Where(s => s.codProduto != 0).ToList();
                        if (ProdutoCampanha.Any() && evento!=null)
                        {
                            foreach (var item in ProdutoCampanha)
                            {
                                item.codEvento = id;
                                var Produto = _context.ProdutosCampanha.Where(s => s.codProduto == item.codProduto).FirstOrDefault();
                                if (Produto==null)
                                {
                                    _context.Add(item);
                                    _context.SaveChanges();
                                }
                            }
                            string diretorio = Directory.GetCurrentDirectory();


                            diretorio = diretorio + "/Empresas/" + evento.codFilial.ToString() + "/Eventos/"+evento.codEvento.ToString()+"/Cardapio";
                            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                            baseUrl += "/Empresas/" + evento.codFilial.ToString() + "/Eventos/" + evento.codEvento.ToString()+"/Cardapio/Index.html";
                            evento.linkCardapio = baseUrl;
                            evento.tipoCardapio = tipoCardapio;

                            switch (tipoCardapio)
                            {
                                case 1:
                                    string origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio");
                                    if (!Directory.Exists(diretorio))
                                    {
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    else
                                    {
                                        Directory.Delete(diretorio, true);
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    if (_pageGeneratorService.GenerateHtml1(diretorio, id, 0))
                                    {
                                        ViewData["Sucesso"] = "Cardapio criado com sucesso";
                                        ViewData["Link"] = evento.linkCardapio;
                                    }
                                    else
                                    {
                                        ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                    }

                                    break;
                                case 2:
                                    origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio2");
                                    if (!Directory.Exists(diretorio))
                                    {
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    else
                                    {
                                        Directory.Delete(diretorio, true);
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    if (_pageGeneratorService.GenerateHtml2(diretorio, id, 0))
                                    {
                                        ViewData["Sucesso"] = "Cardapio criado com sucesso";
                                        ViewData["Link"] = evento.linkCardapio;
                                    }
                                    else
                                    {
                                        ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                    }
                                    break;
                                case 6:
                                    origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio6");
                                    if (!Directory.Exists(diretorio))
                                    {
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    else
                                    {
                                        Directory.Delete(diretorio, true);
                                        Directory.CreateDirectory(diretorio);
                                        _pageGeneratorService.copyarquivo(origem, diretorio);
                                    }
                                    if (_pageGeneratorService.GenerateHtml6(diretorio, id, 0))
                                    {
                                        ViewData["Sucesso"] = "Cardapio criado com sucesso";
                                        ViewData["Link"] = evento.linkCardapio;
                                    }
                                    else
                                    {
                                        ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                    }
                                    break;
                            }
                            
                            _context.Update(evento);
                            _context.SaveChanges();

                            ViewData["produto"] = "Produtos já selecionados";
                            ViewBag.tipoCardapio = tipoCardapio;
                            ViewBag.codEvento = id;
                            return View();
                        }
                        else
                        {
                            evento = _context.Eventos.Where(s => s.codEvento == id).FirstOrDefault();
                            ViewBag.codEvento = id;
                            ViewBag.Msg = "Você deve selecionar os produtos!";
                            ViewBag.tipoCardapio = tipoCardapio;
                            if (evento != null)
                            {
                                var preco = _context.Precos.Where(s => s.codFilial == evento.codFilial).Include(s => s.Produto).ThenInclude(s => s.Categoria);
                                return View(preco);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorViewModel.LogError($"Erro ao chamar CardapioEventos: {ex}");
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
            return View("Error");
        }
        public ActionResult Produtos(int id) 
        {
            if (id>0)
            {
                try
                {
                    Evento evento = _context.Eventos.Where(s => s.codEvento == id).Include(s => s.ProdutosEvento).ThenInclude(s=>s.Produto).ThenInclude(s=>s.Categoria).FirstOrDefault();
                    var precos = _context.Precos.Where(s => s.codFilial == evento.codFilial).Include(s=>s.Produto).ThenInclude(s=>s.Categoria);
                    List<ProdutoCampanha> produtoCampanhas = new List<ProdutoCampanha>();
                    foreach (var item in precos)
                    {
                        ProdutoCampanha produtoCampanha = new ProdutoCampanha();
                        produtoCampanha.valor=item.valor;
                        produtoCampanha.codProduto = item.codProduto;
                        produtoCampanha.codProdCamp = 0;
                        produtoCampanha.Produto=item.Produto;
                        produtoCampanha.codEvento=evento.codEvento;
                        if (evento.ProdutosEvento.Any(s=>s.codProduto==item.codProduto))
                        {
                            produtoCampanha = evento.ProdutosEvento.Where(s => s.codProduto == item.codProduto).FirstOrDefault();
                        }
                        produtoCampanhas.Add(produtoCampanha);
                    }
                    ViewBag.tipoCardapio = evento.tipoCardapio;
                    ViewBag.codEvento = id;
                    return View(produtoCampanhas);
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar ProdutosEventos: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
            return View("Error");
        }
        public ActionResult BuscarPresente(int id)
        {
            if (id > 0)
            {
                ViewBag.codConvite = id;
                
                ListaPresente listaPresente = _context.ListaPresentes.Where(s => s.codConvite == id).FirstOrDefault();
                if (listaPresente!=null)
                {
                    return RedirectToAction("Presentes", new { codConvite = id });
                }
                return View();
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PresentesB(int codConvite, string descricao, bool descricaoEven, bool convitedesc, string nascimento,string genero, string precoMin, string precoMax)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var convite = _context.Convites.Where(s => s.codConvite == codConvite).Include(s => s.Evento).Include(s=>s.Convidados).FirstOrDefault();
            ViewBag.codConvite = codConvite;
            string texto = "";
            if (descricaoEven)
            {
                texto = "Descriçao do evento "+convite.Evento.descricaoEvento+".";
            }
            if (convitedesc)
            {
                texto += "Texto convite " + convite.msgConvite + ".";
            }
            int quantidade = 0;
            if (convite.Convidados.Count()<5)
            {
                quantidade = 5;
            }
            else
            {
                quantidade = convite.Convidados.Count();
            }
            texto += "Descrição do anfitrião nasceu na data de" + nascimento + ", seu genêro é " + genero + ". Outras informações da pessoa " + descricao + ". A quantidade de presentes deve ser exatamente:" + convite.Convidados.Count() + "O valor dos presentes deve estár obrigatoriamente entre " + precoMin + " e " + precoMax;
            var presentes =await _pageGeneratorService1.ObterRecomendacoes(texto);

            ListaPresente listaPresente = new ListaPresente();
            listaPresente.codConvite = codConvite;
            listaPresente.dataLista = DateTime.Now;
            listaPresente.codUsuario = int.Parse(userId);
            
            _context.Add(listaPresente);
            _context.SaveChanges();

            string diretorio = Directory.GetCurrentDirectory();
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            if (convite.codFilial==null || convite.Evento==null || convite.codFilial==0 || convite.UsuarioFilial==null)
            {
                diretorio = diretorio + "/wwwroot/Usuarios/"+convite.codCriador+"/Eventos/" +convite.codEvento  + "/ImagensPresentes/";
                baseUrl += "/Usuarios/" + convite.codCriador + "/Eventos/"+convite.codEvento+"/ImagensPresentes/";
            }
            else
            {
                diretorio = diretorio + "/Empresas/" + convite.Evento.codFilial + "/Eventos/"+convite.codEvento+"/ImagensPresentes/";
                baseUrl += "/Empresas/" + convite.Evento.codFilial + "/Eventos/"+convite.codEvento+"/ImagensPresentes/";
            }

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            foreach (var item in presentes)
            {

                Models.Presente presente = new Models.Presente();
                presente.codListaPresente = listaPresente.codListaPres;
                presente.Descricao = item.Descricao;
                presente.Nome = item.Nome;
                presente.Preco = item.Preco;
                _context.Add(presente);
                _context.SaveChanges();

                if (string.IsNullOrWhiteSpace(item.Imagem) || item.Imagem=="Erro")
                {
                    var baseUrlO = $"{this.Request.Scheme}://{this.Request.Host}";
                    presente.Imagem = baseUrlO + "/imagens/ProdutoSIMG.png";
                }
                else
                {
                    string path = diretorio +  presente.codPresente + ".jpg";
                    await BaixarEGuardarImagem(item.Imagem, path);
                    presente.Imagem = baseUrl +  presente.codPresente + ".jpg";
                }
                _context.Update(presente);
                _context.SaveChanges();
            }
            
            if (presentes.Any())
            {
                return RedirectToAction("Presentes", new { codConvite = codConvite });
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        static async Task BaixarEGuardarImagem(string imageUrl, string savePath)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
                await System.IO.File.WriteAllBytesAsync(savePath, imageBytes);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CriarPreferencia(int[] codPresente, int codConvite, string? email,string? nome,string? telefone, string? cpf)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            ReciboPresente reciboPresente = new ReciboPresente(){
                cpfCliente = cpf,
                emailCliente = email,
                nomeCliente = nome,
                telefoneCliente = telefone,
                status = "Pendente",
            };
            if (string.IsNullOrWhiteSpace(userId))
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cpf))
                {
                    ViewBag.msg = "Você deve preencher o nome, email e telefone para continuar.";
                    return RedirectToAction("Presentes", new { codConvite = codConvite });
                }
            }
            else
            {
                reciboPresente.codUsuario = int.Parse(userId);
                var usuario = _context.Usuario.Where(s => s.codUsuario == reciboPresente.codUsuario).FirstOrDefault();
                reciboPresente.usuario = usuario;
            }
            reciboPresente.codConvite = codConvite;
            if (codPresente.Length > 0)
            {
                List<Models.Presente> presentes = new List<Models.Presente>();
                foreach (var item in codPresente)
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
                return RedirectToAction("Presentes", new { codConvite = codConvite });
            }
                
        }
        public string TestePreferencia(ReciboPresente recibo, string link)
        {
            string reciboId = Guid.NewGuid().ToString();
            _memoryCache.Set(reciboId, recibo, TimeSpan.FromHours(1));
            link += "/ReciboPresentes/PresentesStatus/1?reciboId=" + reciboId;
            return link;
        }
        public ActionResult Presentes(int codConvite)
        {
            
            var listaPresente = _context.ListaPresentes.Where(s => s.codConvite ==codConvite).FirstOrDefault();
            
            if (listaPresente!=null)
            {
                var presentes = _context.Presentes.Where(s => s.codListaPresente == listaPresente.codListaPres).ToList();
                presentes = presentes.OrderBy(s => s.Nome).ToList();
                if (presentes.Any())
                {
                    var viewModel = new PresenteFormularioViewModel
                    {
                        Presentes = presentes.ToPagedList(1, 15),
                        codConvite = codConvite,
                    };
                    return View(viewModel);
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public ActionResult GerarToken(PresenteFormularioViewModel FormularioView) 
        {
            bool hasError = false;

            if (FormularioView.CodPresenteSelecionados == null || !FormularioView.CodPresenteSelecionados.Any())
            {
                ViewBag.msg = "Você não selecionou nenhum presente!";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(FormularioView.Email))
            {
                ViewBag.msg1 = "Você deve preencher o email para continuar.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(FormularioView.Nome))
            {
                ViewBag.msg2 = "Você deve preencher o nome para continuar.";
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(FormularioView.CPF))
            {
                ViewBag.msg3 = "Você deve preencher o CPF para continuar.";
                hasError = true;
            }

            FormularioView.Presentes = _context.Presentes
                .Where(s => s.listaPresente.codConvite==FormularioView.codConvite)
                .ToPagedList(1, 15);
            if (hasError)
            {
                return View("Presentes",FormularioView); // substitua pelo nome da sua View
            }

            Random random = new Random();
            string token = "";

            for (int i = 0; i < 6; i++)
            {
                token += random.Next(0, 10).ToString(); // Gera um número aleatório entre 0 e 9
            }
            if (_pageGeneratorService.EnviarToken(FormularioView.Email, FormularioView.Nome, token))
            {
                _memoryCache.Set(FormularioView.Email, token, TimeSpan.FromHours(1));
                _memoryCache.Set(token, FormularioView, TimeSpan.FromHours(1));
                return RedirectToAction("ValidarToken", "ReciboPresentes", new { email = FormularioView.Email });
            }
            else
            {
                ViewBag.msg = "Ops! Houve um erro ao enviar o token para o email informado.";
                return View("Error");
            }
        }
        [HttpPost]
        public bool VerificarValor(string valor)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
            interpolatedStringHandler.AppendFormatted(Request.Scheme);
            interpolatedStringHandler.AppendLiteral("://");
            interpolatedStringHandler.AppendFormatted(Request.Host);
            string link = interpolatedStringHandler.ToStringAndClear() + "/Eventos/Details/" + valor;
            IQueryable<Evento> source = _context.Eventos.Where(s => s.linkEvento == link);
            return source != null && source.Count() > 0;
        }
        // GET: Eventos/Create
        public IActionResult Create()
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                int CodUsuario = int.Parse(userId);
                var filiais = _context.UsuarioFiliais
                    .Where(uf => uf.codUsuario == CodUsuario)
                    .Select(uf => uf.Filial)
                    .ToList();

                ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");

                return View();
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // POST: Eventos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Evento evento, IFormFile banner, IFormFile thumbnail)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                try
                {
                    evento.dataCriacaoEvento = DateTime.Now;
                    evento.codUsuario = int.Parse(userId);
                    evento.statusEvento = true;

                    string str1 = string.Join("", (evento.nomeEvento.Split(' ')).Where((s => !string.IsNullOrEmpty(s))).Select((s => s[0]))) + "EM";
                    if (evento.codFilial==null)
                    {
                        str1 += evento.codUsuario.ToString();
                    }
                    else 
                    {
                        str1 += evento.codFilial.ToString();
                    }
                    foreach (var lote in evento.Lotes)
                    {
                        evento.qtdTotalIngEvento = lote.qtdIngLote + evento.qtdTotalIngEvento;
                        lote.Ingressos = new List<Ingresso>();
                        for (int i = 0; i < lote.qtdIngLote; i++)
                        {
                            Ingresso ingresso = new Ingresso();
                            ingresso.tipoVendaIngresso = "Não vendido!";
                            ingresso.ativoIngresso = true;
                            string[] strArray = { str1, "L", null, null, null };
                            strArray[2] = lote.numLote.ToString();
                            strArray[3] = "SQ";
                            strArray[4] = i.ToString();
                            ingresso.ticketIngresso = string.Concat(strArray);
                            lote.Ingressos.Add(ingresso);
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        evento.linkEvento = baseUrl + "/Eventos/Details/" + evento.linkEvento;

                        _context.Add(evento);
                        await _context.SaveChangesAsync();

                        string diretorio = Directory.GetCurrentDirectory();

                        if (evento.codFilial==null)
                        {
                            diretorio = diretorio + "/wwwroot/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento";
                            if (banner != null)
                            {
                                if (!Directory.Exists(diretorio))
                                {
                                    Directory.CreateDirectory(diretorio);
                                }
                                var fileName = Path.GetFileName(banner.FileName);
                                string name = diretorio + "/" + banner.FileName;
                                using (var stream = new FileStream(name, FileMode.Create))
                                {
                                    banner.CopyTo(stream);
                                }
                                evento.bannerEvento = baseUrl + "/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                            }
                            if (thumbnail != null)
                            {
                                if (!Directory.Exists(diretorio))
                                {
                                    Directory.CreateDirectory(diretorio);
                                }
                                var fileName = Path.GetFileName(thumbnail.FileName);
                                string name = diretorio + "/" + thumbnail.FileName;
                                using (var stream = new FileStream(name, FileMode.Create))
                                {
                                    thumbnail.CopyTo(stream);
                                }
                                evento.thumbnailEvento = baseUrl + "/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                            }
                        }
                        else
                        {
                            diretorio = diretorio + "/Empresas/" + evento.codFilial + "/Eventos/"+ evento.codEvento+"/ImagensEvento";
                            if (banner != null)
                            {
                                if (!Directory.Exists(diretorio))
                                {
                                    Directory.CreateDirectory(diretorio);
                                }
                                var fileName = Path.GetFileName(banner.FileName);
                                string name = diretorio + "/" + banner.FileName;
                                using (var stream = new FileStream(name, FileMode.Create))
                                {
                                    banner.CopyTo(stream);
                                }
                                evento.bannerEvento = baseUrl + "/Empresas/" + evento.codFilial + "/Eventos/"+evento.codEvento+"/ImagensEvento/" + fileName;
                            }
                            if (thumbnail != null)
                            {
                                if (!Directory.Exists(diretorio))
                                {
                                    Directory.CreateDirectory(diretorio);
                                }
                                var fileName = Path.GetFileName(thumbnail.FileName);
                                string name = diretorio + "/" + thumbnail.FileName;
                                using (var stream = new FileStream(name, FileMode.Create))
                                {
                                    thumbnail.CopyTo(stream);
                                }
                                evento.thumbnailEvento = baseUrl + "/Empresas/" + evento.codFilial + "/Eventos/"+evento.codEvento+"/ImagensEvento/" + fileName;
                            }
                        }
                        _context.Update(evento);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Lista));
                    }
                    else
                    {
                        int CodUsuario = int.Parse(userId);
                        var filiais = _context.UsuarioFiliais
                            .Where(uf => uf.codUsuario == CodUsuario)
                            .Select(uf => uf.Filial)
                            .ToList();

                        ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");
                        return View(evento);
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar CreateEventos: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            
        }
        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var evento = await _context.Eventos.Where(s => s.codEvento == id).Include(s => s.Endereco).Include(s => s.Lotes).FirstOrDefaultAsync();
                if (evento == null)
                {
                    return NotFound();
                }
                var filial = _context.Filiais.Where(uf => uf.codFilial == evento.codFilial);
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                baseUrl = baseUrl + "/Eventos/Details/";
                evento.linkEvento = evento.linkEvento.Replace(baseUrl,"");
                ViewData["codFilial"] = new SelectList(filial, "codFilial", "nome", evento.codFilial);
                return View(evento);
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar EditEventos: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // POST: Eventos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evento evento, IFormFile? banner, IFormFile? thumbnail)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                
                try
                {
                    evento.codUsuario = int.Parse(userId);
                    evento.statusEvento = true;
                    string str1 = string.Join("", (evento.nomeEvento.Split(' ')).Where((s => !string.IsNullOrEmpty(s))).Select((s => s[0]))) + "EM" + evento.codFilial.ToString();

                    if (evento.eventoGratuito==true)
                    {
                        foreach (var lote in evento.Lotes)
                        {
                            lote.Ingressos = new List<Ingresso>();
                            for (int i = evento.qtdTotalIngEvento; i < lote.qtdIngLote; i++)
                            {
                                Ingresso ingresso = new Ingresso();
                                ingresso.tipoVendaIngresso = "Não vendido!";
                                ingresso.ativoIngresso = true;
                                string[] strArray = { str1, "L", null, null, null };
                                strArray[2] = lote.numLote.ToString();
                                strArray[3] = "SQ";
                                strArray[4] = i.ToString();
                                ingresso.ticketIngresso = string.Concat(strArray);
                                lote.Ingressos.Add(ingresso);
                            }
                            evento.qtdTotalIngEvento = lote.qtdIngLote;
                        }
                    }
                    else
                    {
                        foreach (var lote in evento.Lotes)
                        {
                            if (lote.codLote==0)
                            {
                                evento.qtdTotalIngEvento = lote.qtdIngLote + evento.qtdTotalIngEvento;
                                lote.Ingressos = new List<Ingresso>();
                                for (int i = 0; i < lote.qtdIngLote; i++)
                                {
                                    Ingresso ingresso = new Ingresso();
                                    ingresso.tipoVendaIngresso = "Não vendido!";
                                    ingresso.ativoIngresso = true;
                                    string[] strArray = { str1, "L", null, null, null };
                                    strArray[2] = lote.numLote.ToString();
                                    strArray[3] = "SQ";
                                    strArray[4] = i.ToString();
                                    ingresso.ticketIngresso = string.Concat(strArray);
                                    lote.Ingressos.Add(ingresso);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar CreateEventos: {ex}");
                }

                if (ModelState.IsValid)
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    string diretorio = Directory.GetCurrentDirectory();

                    if (evento.codFilial == null)
                    {
                        diretorio = diretorio + "/wwwroot/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento";
                        if (banner != null)
                        {
                            if (!Directory.Exists(diretorio))
                            {
                                Directory.CreateDirectory(diretorio);
                            }
                            var fileName = Path.GetFileName(banner.FileName);
                            string name = diretorio + "/" + banner.FileName;
                            using (var stream = new FileStream(name, FileMode.Create))
                            {
                                banner.CopyTo(stream);
                            }
                            evento.bannerEvento = baseUrl + "/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                        }
                        if (thumbnail != null)
                        {
                            if (!Directory.Exists(diretorio))
                            {
                                Directory.CreateDirectory(diretorio);
                            }
                            var fileName = Path.GetFileName(thumbnail.FileName);
                            string name = diretorio + "/" + thumbnail.FileName;
                            using (var stream = new FileStream(name, FileMode.Create))
                            {
                                thumbnail.CopyTo(stream);
                            }
                            evento.thumbnailEvento = baseUrl + "/Usuarios/" + evento.codUsuario + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                        }
                    }
                    else
                    {
                        diretorio = diretorio + "/Empresas/" + evento.codFilial + "/Eventos/" + evento.codEvento + "/ImagensEvento";
                        if (banner != null)
                        {
                            if (!Directory.Exists(diretorio))
                            {
                                Directory.CreateDirectory(diretorio);
                            }
                            var fileName = Path.GetFileName(banner.FileName);
                            string name = diretorio + "/" + banner.FileName;
                            using (var stream = new FileStream(name, FileMode.Create))
                            {
                                banner.CopyTo(stream);
                            }
                            evento.bannerEvento = baseUrl + "/Empresas/" + evento.codFilial + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                        }
                        if (thumbnail != null)
                        {
                            if (!Directory.Exists(diretorio))
                            {
                                Directory.CreateDirectory(diretorio);
                            }
                            var fileName = Path.GetFileName(thumbnail.FileName);
                            string name = diretorio + "/" + thumbnail.FileName;
                            using (var stream = new FileStream(name, FileMode.Create))
                            {
                                thumbnail.CopyTo(stream);
                            }
                            evento.thumbnailEvento = baseUrl + "/Empresas/" + evento.codFilial + "/Eventos/" + evento.codEvento + "/ImagensEvento/" + fileName;
                        }
                    }

                    evento.linkEvento = baseUrl + "/Eventos/Details/" + evento.linkEvento;
                    try
                    {
                        _context.Update(evento);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Lista));
                    }
                    catch (Exception ex)
                    {
                        ErrorViewModel.LogError($"Erro ao chamar CreateEventos: {ex}");
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                }
                else
                {
                    int CodUsuario = int.Parse(userId);
                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();

                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");
                    return View(evento);
                }

            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Endereco)
                .Include(e => e.UsuarioFilial)
                .FirstOrDefaultAsync(m => m.codEvento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }
        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<bool> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.Where(s=>s.codEvento==id).Include(s=>s.Lotes).ThenInclude(s=>s.Ingressos).FirstOrDefaultAsync();
            
            if (evento != null)
            {
                try
                {
                    var ingressos = _context.Ingressos.Where(s => s.tipoVendaIngresso != "Não vendido!" && s.Lote.codEvento == id);
                    if (ingressos.Any())
                    {
                        evento.statusEvento = false;
                        _context.Update(evento);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    _context.Eventos.Remove(evento);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar DeleteEventos: {ex}");
                    return false;
                }
            }
            return false;
        }
        public bool Desativar(int id) 
        {
            var filial = _context.Filiais.Where(s => s.codFilial == id).FirstOrDefault();
            try
            {
                if (filial != null)
                {
                    filial.ativo = false;

                    var campanhas = _context.Campanhas.Where(s => s.codFilial == filial.codFilial);
                    if (campanhas.Any())
                    {
                        foreach (var item in campanhas)
                        {
                            item.campanhaAtiva = false;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                    }
                    var Cupons = _context.Cupons.Where(s => s.codFilial == filial.codFilial);
                    if (Cupons.Any())
                    {
                        foreach (var cupom in Cupons)
                        {
                            cupom.statusCupom = false;
                            _context.Update(cupom);
                            _context.SaveChanges();
                        }
                    }
                    var vouchers = _context.Vouchers.Where(s => s.codFilial == filial.codFilial);
                    if (!vouchers.Any())
                    {
                        foreach (var voucher in vouchers)
                        {
                            voucher.ativo = false;
                            _context.Update(voucher);
                            _context.SaveChanges();
                        }
                    }
                    var usuarioFilial = _context.UsuarioFiliais.Where(s => s.codFilial == filial.codFilial);
                    if (usuarioFilial.Any())
                    {
                        foreach (var item in usuarioFilial)
                        {
                            item.ativo = false;
                            _context.Update(item);
                            _context.SaveChanges();
                        }
                    }
                    _context.Update(filial);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.codEvento == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using System.Security.Claims;
using X.PagedList.Extensions;
using BixWeb.Services;
using System.Reflection;
using BixWeb.ViewModel;

namespace BixWeb.Controllers
{
    public class ConvitesController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;

        public ConvitesController(DbPrint context, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
        }

        // GET: Convites
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

                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != null)
                {
                    int codUsuario = int.Parse(userId);
                    var convites = _context.Convites  // Supondo que a navegação está configurada
                        .Include(c => c.UsuarioFilial.Filial).Include(s=>s.Usuario).Include(s=>s.Convidados).Include(s=>s.Evento)   // Se tiver uma entidade Filial relacionada
                        .Where(c => _context.UsuarioFiliais
                                        .Any(uf => uf.codUsuario == codUsuario &&
                                                   uf.codFilial == c.codFilial) ||
                                    c.codCriador == codUsuario);

                    if (convites.Any())
                    {
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            convites = convites.Where(s => s.Evento.nomeEvento.Contains(searchString)
                                                       || s.Usuario.nome.Contains(searchString));

                        }

                        switch (sortOrder)
                        {
                            case "name_desc":
                                convites = convites.OrderByDescending(s => s.dataCriacao);
                                break;
                            default:  // Name ascending 
                                convites = convites.OrderBy(s => s.dataCriacao);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(convites.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["Convites"] = "Nenhum convite encontrado!";
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
                ErrorViewModel.LogError($"Erro ao chamar IndexConvites: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
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
					var convites = _context.Convites.Where(s=>s.codCliente==codUsuario).Include(s => s.Usuario).Include(s => s.Ingressos).Include(s => s.Evento).Include(s=>s.ListaPresente).AsQueryable();

					if (convites.Any())
					{
						if (!String.IsNullOrEmpty(searchString))
						{
							convites = convites.Where(s => s.Evento.nomeEvento.Contains(searchString)
													   || s.Usuario.nome.Contains(searchString));

						}

						switch (sortOrder)
						{
							case "name_desc":
								convites = convites.OrderByDescending(s => s.dataCriacao);
								break;
							default:  // Name ascending 
								convites = convites.OrderBy(s => s.dataCriacao);
								break;
						}

						int pageSize = 10;
						int pageNumber = (page ?? 1);
						return View(convites.ToPagedList(pageNumber, pageSize));
					}
					else
					{
						ViewData["Convites"] = "Nenhum convite encontrado!";
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
				ErrorViewModel.LogError($"Erro ao chamar IndexConvites: {ex}");
				ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
				return View("Error");
			}
		}
		public JsonResult ListaEventos(int id)
        {
            return Json(_context.Eventos.Where(s => s.codFilial == id).Select(s => new
            {
                codCampanha = s.codEvento,
                nomeCampanha = s.nomeEvento
            }).ToList());
        }
        public JsonResult QtdIngressos(int id)
        {
            var evento = _context.Eventos.Where(s => s.codEvento == id).Include(s => s.Lotes).ThenInclude(s => s.Ingressos.Where(s => s.tipoVendaIngresso == "Não vendido!")).FirstOrDefault();
            if (evento != null)
            {
                int ingressos=0;
                foreach (var item in evento.Lotes)
                {
                    ingressos += item.Ingressos.Count();
                }
                return Json(new { ingressos });
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult VeryCPF(string cpf)
        {
            var cliente = _context.Usuario.Where(s => s.cpf == cpf).FirstOrDefault();
            if (cliente != null)
            {
                return Json(new
                {
                    success = true,
                    nome = cliente.nome,
                    email = cliente.email,
                    telefone = cliente.telefone,
                    codCliente= cliente.codUsuario
                });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        // GET: Convites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var convite = await _context.Convites
                .Include(c => c.Evento).Include(s=>s.Convidados).Include(s=>s.Usuario)
                .FirstOrDefaultAsync(m => m.codConvite == id);
            if (convite == null)
            {
                return NotFound();
            }

            return View(convite);
        }
        public IActionResult Adicionar() 
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                int CodUsuario = int.Parse(userId);
                var eventos = _context.Eventos
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .ToList();
                ViewBag.Eventos = new SelectList(eventos, "codEvento", "nomeEvento");
                return View();
            }
            else
            {

                return View("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(Convite convite, IFormFile? banner, int qtdIngressos) 
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (userId != null)
                    {
                        int CodUsuario = int.Parse(userId);
                        string diretorio = Directory.GetCurrentDirectory();
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        if (banner != null)
                        {
                            diretorio = diretorio + "/wwwroot/Usuarios/" + CodUsuario + "/Eventos/"+convite.codEvento+"/ImagensEvento/Convites/";
                            if (!Directory.Exists(diretorio))
                            {
                                Directory.CreateDirectory(diretorio);
                            }
                            var fileName = Path.GetFileName(banner.FileName);
                            string name = diretorio + banner.FileName;
                            using (var stream = new FileStream(name, FileMode.Create))
                            {
                                banner.CopyTo(stream);
                            }
                            convite.imagemConvite = baseUrl + "/Usuarios/" + CodUsuario + "/Eventos/"+convite.codEvento+"/ImagensEvento/Convites/" + fileName;
                        }
                        convite.dataCriacao = DateTime.Now;
                        convite.codCriador = CodUsuario;
                        convite.codCliente = CodUsuario; // Assumindo que o criador é o cliente
                        
                        convite.ativo = true;
                        _context.Add(convite);
                        await _context.SaveChangesAsync();

                        var ingressos = _context.Ingressos.Where(s => s.Lote.codEvento == convite.codEvento && s.tipoVendaIngresso == "Não vendido!").Take(qtdIngressos);
                        foreach (var ingresso in ingressos)
                        {
                            // Atualizando o ingresso
                            ingresso.tipoVendaIngresso = "Reservado no site";
                            ingresso.codConvite = convite.codConvite;
                            ingresso.Convite = convite;
                            _context.Update(ingresso);  // Marca o ingresso para ser atualizado

                            // Criando o convidado
                            Convidado convidado = new Convidado
                            {
                                confirmacaoConvite = "Aguardando",
                                vistoConvite = false,
                                codConvite = convite.codConvite,
                                codIngresso = ingresso.codIngresso
                            };
                            _context.Add(convidado);  // Marca o convidado para ser adicionado
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Lista));
                    }
                    else
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar AdicionarConvites: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            return View();
        }
        // GET: Convites/Create
        public IActionResult Create()
        {
            try
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
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ViewCreateConvites: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // POST: Convites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codConvite,msgConvite,facebook,instagram,imagemConvite,codFilial,codEvento,dataCriacao,Usuario")] Convite convite, int qtdIngressos)
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != null)
                {
                    int CodUsuario = int.Parse(userId);
                    if (ModelState.IsValid)
                    {
                        var cliente = _context.Usuario.Where(s => s.cpf==convite.Usuario.cpf
                                  || s.email == convite.Usuario.email || s.telefone == convite.Usuario.telefone).FirstOrDefault();

                        if (cliente!=null) {
                            convite.Usuario = cliente;
                            convite.codCliente = cliente.codUsuario;
                        }

                        convite.dataCriacao= DateTime.Now;
                        convite.codCriador = CodUsuario;
                        convite.ativo=true;

                        _context.Add(convite);
                        await _context.SaveChangesAsync();


                        var ingressos = _context.Ingressos.Where(s => s.Lote.codEvento == convite.codEvento && s.tipoVendaIngresso == "Não vendido!").Take(qtdIngressos);
                        foreach (var ingresso in ingressos)
                        {
                            // Atualizando o ingresso
                            ingresso.tipoVendaIngresso = "Reservado no site";
                            ingresso.codConvite = convite.codConvite;
                            ingresso.Convite = convite;
                            _context.Update(ingresso);  // Marca o ingresso para ser atualizado

                            // Criando o convidado
                            Convidado convidado = new Convidado
                            {
                                confirmacaoConvite = "Aguardando",
                                vistoConvite = false,
                                codConvite = convite.codConvite,
                                codIngresso = ingresso.codIngresso
                            };
                            _context.Add(convidado);  // Marca o convidado para ser adicionado
                        }

                        // Chama SaveChangesAsync uma única vez, após terminar a iteração
                        await _context.SaveChangesAsync();
                        var usuarioFilial = _context.UsuarioFiliais.Where(s => s.codFilial == convite.codFilial && s.codUsuario == convite.codCliente).FirstOrDefault();
                        if (usuarioFilial==null)
                        {
                            usuarioFilial=new UsuarioFilial();
							usuarioFilial.codUsuario = convite.codCliente;
							usuarioFilial.codFilial = (int)convite.codFilial;
							usuarioFilial.ativo = false;
							usuarioFilial.tipoUsuario = "Cliente";
                            string token= Guid.NewGuid().ToString();
                            usuarioFilial.token = $"{this.Request.Scheme}://{this.Request.Host}"+"/Login/Ativar/"+token;
							usuarioFilial.dataCadastro=DateTime.Now;

							if (_pageGeneratorService.AtivarContaCliente(convite.Usuario,usuarioFilial))
                            {
                                usuarioFilial.token=token;
                                _context.Add(usuarioFilial);
                                await _context.SaveChangesAsync();
								return RedirectToAction(nameof(Index));
							}
                            else
                            {
                                ViewData["Error"] = "Ops! Houve um erro ao enviar o email";
                                return View("Error");
                            }
                        }
                        else
                        {
                            var evento = _context.Eventos.Where(s => s.codEvento == convite.codEvento).Include(s => s.Endereco).FirstOrDefault();
                            string link= $"{this.Request.Scheme}://{this.Request.Host}" + "/Login/";
                            if (evento != null)
                            {
                                if (_pageGeneratorService.ComprovanteConvites(convite.Usuario, link, evento))
                                {
									return RedirectToAction(nameof(Index));
								}
                                else
                                {

									ViewData["Error"] = "Ops! Houve um erro ao enviar o email";
									return View("Error");
								}
                            }
                            else
                            {
								ViewData["Error"] = "Ops! Houve um erro ao enviar o email";
								return View("Error");
							}
                        }
                    }

                    
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
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar CreateConvites: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        public IActionResult Convidado(int id)
        {
            Convidado convidado = new Convidado();
            convidado.Convite = _context.Convites.Where(s => s.codConvite == id).Include(s=>s.Usuario).Include(s => s.Evento).ThenInclude(s=>s.Endereco).FirstOrDefault();
            convidado.codConvite = id;
            return View(convidado);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convidado( Convidado convidado)
        {
            if (!string.IsNullOrWhiteSpace(convidado.emailConvidado) &&
                !string.IsNullOrWhiteSpace(convidado.telefoneConvidado) &&
                !string.IsNullOrWhiteSpace(convidado.nomeConvidado))
            {
                convidado.vistoConvite = true;
                convidado.confirmacaoConvite = "Confirmado";

                _context.Add(convidado);
                await _context.SaveChangesAsync();
                return RedirectToAction("Confirmar", new { id = 0, resposta = true});
            }
            else
            {

                ViewBag.Erro = "Você não preencheu os campos corretamente!";
                return View();
            }

        }
        // GET: Convites/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            ViewBag.linkConvite = baseUrl + "/Convites/Convidado/" + id;
            var convite = await _context.Convites.Where(s=>s.codConvite==id).Include(s=>s.Evento).Include(s=>s.Convidados).FirstOrDefaultAsync();
            if (convite == null)
            {
                return NotFound();
            }

            return View(convite);
        }
		// POST: Convites/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Convite convite, IFormFile? banner)
        {
            if (id != convite.codConvite)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {   
                    if (banner != null)
                    {
                        string diretorio = Directory.GetCurrentDirectory();
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        if (convite.codFilial == null)
                        {
                            diretorio = diretorio + "/wwwroot/Usuarios/" + convite.codCriador + "/Eventos/" + convite.codEvento + "/ImagensEvento/Convites/";
                            baseUrl += "/Usuarios/" + convite.codCriador + "/Eventos/" + convite.codEvento + "/ImagensEvento/Convites/";
                        }
                        else
                        {
                            diretorio = diretorio + "/Empresas/" + convite.codFilial + "/Eventos/"+convite.codEvento+"/ImagensEvento/Convites/";
                            baseUrl += "/Empresas/" + convite.codFilial + "/Eventos/" + convite.codEvento + "/ImagensEvento/Convites/";
                        }
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(banner.FileName);
                        string name = diretorio + banner.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            banner.CopyTo(stream);
                        }
                        convite.imagemConvite = baseUrl  + fileName;
                    }

                    _context.Update(convite);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar EditConvites: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                return RedirectToAction(nameof(Lista));
            }
            return View(convite);
        }
        public async Task<IActionResult> EnviarConvite(int id, int? sessao) 
        {
            var convidados=await _context.Convidados.Where(s=>s.codConvite==id).ToListAsync();
            if (convidados.Any()) 
            {
                convidados = convidados.OrderBy(s=>s.nomeConvidado).ToList();
                ViewBag.sessao = sessao;
                return View(convidados.ToPagedList());
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EnviarConvite(List<Convidado> Convidados, string? sessao)
        {
            if (Convidados.Any())
            {
                foreach (var convidado in Convidados)
                {
                    if (convidado.confirmacaoConvite == "Aguardando")
                    {
                        if (!string.IsNullOrWhiteSpace(sessao))
                        {
                            if (!string.IsNullOrWhiteSpace(convidado.telefoneConvidado))
                            {
                                convidado.telefoneConvidado= convidado.telefoneConvidado.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                                if (!string.IsNullOrWhiteSpace(convidado.telefoneConvidado))
                                {
                                    Mensagem mensagem = new Mensagem();

                                    var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                                    if (userId != null)
                                    {
                                        int codUsuario = int.Parse(userId);
                                        string token = _context.Usuario.Where(s => s.codUsuario == codUsuario).Select(s => s.token).FirstOrDefault();
                                        if (!string.IsNullOrWhiteSpace(token))
                                        {
                                            mensagem.token = token;
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
                                    string diretorio = Directory.GetCurrentDirectory();
                                    convidado.Convite = _context.Convites.Where(s => s.codConvite == convidado.codConvite).FirstOrDefault();
                                    convidado.Ingresso = _context.Ingressos.Where(s => s.codIngresso == convidado.codIngresso).Include(s => s.Lote).ThenInclude(s => s.Evento).FirstOrDefault();
                                    mensagem.numero = "55" + convidado.telefoneConvidado.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                                    diretorio = Directory.GetCurrentDirectory();
                                    string link = $"{this.Request.Scheme}://{this.Request.Host}";

                                    mensagem.urlImagem = link;
                                    if (convidado.Ingresso.Lote.Evento.codFilial != null)
                                    {
                                        link += "/Ingresso/Validar/" + convidado.Ingresso.codIngresso + "?usuario=" + convidado.Ingresso.Lote.Evento.codFilial;
                                        diretorio = diretorio + "/Empresas/" + convidado.Ingresso.Lote.Evento.codFilial + "/Eventos/" + convidado.Ingresso.Lote.codEvento + "/Ingressos/";
                                        mensagem.urlImagem += "/Empresas/" + convidado.Ingresso.Lote.Evento.codFilial + "/Eventos/Ingressos/" + convidado.Ingresso.Lote.Evento.codEvento + "/" + convidado.Ingresso.ticketIngresso + ".jpg";
                                    }
                                    else
                                    {
                                        link += "/Ingresso/Validar/" + convidado.Ingresso.codIngresso + "?filial=" + convidado.Ingresso.Lote.Evento.codFilial;
                                        diretorio = diretorio + "/wwwroot/Usuarios/" + convidado.Convite.codCriador + "/Eventos/" + convidado.Convite.codEvento + "/Ingressos/";
                                        mensagem.urlImagem += "/Usuarios/" + convidado.Ingresso.Lote.Evento.codFilial + "/Eventos/" + convidado.Convite.codEvento + "/Ingressos/" + convidado.Ingresso.ticketIngresso + ".jpg";

                                    }
                                    if (!Directory.Exists(diretorio))
                                    {
                                        Directory.CreateDirectory(diretorio);
                                    }
                                    link = $"{this.Request.Scheme}://{this.Request.Host}";
                                    if (_pageGeneratorService.GerarIngresso(convidado.Ingresso.codIngresso, diretorio, link))
                                    {
                                        mensagem.textoConvite = "Este é seu ingresso para o evento!\n" + convidado.Convite.msgConvite + "\nVeja mais informações em: " + convidado.Convite.Evento.linkEvento;
                                        string linksim = $"" + link + "/Convites/ConfirmarPresenca/" + convidado.codConvidado + "?resposta=true";
                                        string linknao = $"" + link + "/Convites/ConfirmarPresenca/" + convidado.codConvidado + "?resposta=false";
                                        mensagem.textoConfirmacao = "Confirmação de Presença:\n\n[Sim, estarei lá] " + linksim + " \n[Não posso ir] " + linknao + " ";

                                        if (!await _pageGeneratorService.EnviarConviteWhats(mensagem))
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
                                    _context.Update(convidado);
                                    await _context.SaveChangesAsync();
                                    ViewBag.msg = "Convites enviados com sucesso!";
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(convidado.emailConvidado))
                            {
                                convidado.Convite = _context.Convites.Where(s => s.codConvite == convidado.codConvite).FirstOrDefault();
                                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                bool enviar = _pageGeneratorService.EnviarConvite(convidado, baseUrl);
                                
                                if (!enviar)
                                {
                                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                                    return View("Error");
                                }
                                _context.Update(convidado);
                                await _context.SaveChangesAsync();
                                ViewBag.msg = "Convites enviados com sucesso!";
                            }
                        }
                    }
                }
                return View(Convidados.ToPagedList());
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public IActionResult VisualisarEmail(int id) 
        {
            var convidado=_context.Convidados.Find(id);
            if (convidado != null) 
            {
                convidado.vistoConvite = true;

                _context.Update(convidado);
                _context.SaveChanges();
            }
            return File(Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"), "image/gif");
        }
        public IActionResult Confirmar(int id, bool resposta) 
        {
            var convidado = _context.Convidados.Find(id);

            try
            {
                if (convidado != null)
                {
                    if (resposta)
                    {
                        convidado.confirmacaoConvite = "Confirmado";
                    }
                    else
                    {
                        convidado.confirmacaoConvite = "Recusado";
                    }
                    convidado.Convite = _context.Convites.Where(s => s.codConvite == convidado.codConvite).Include(s => s.Usuario).FirstOrDefault();
                    if (convidado.Convite != null)
                    {
                        if (convidado.Convite.notificar==1)
                        {
                            _pageGeneratorService.EnviarNotPresenca(convidado);
                        }
                        else if (convidado.Convite.notificar == 2)
                        {
                            _pageGeneratorService.EnviarNotPresencaWhats(convidado);
                        }
                    }
                    ViewBag.msg = "Obrigado " + convidado.nomeConvidado + " por responder ao convite! Registramos sua resposta com sucesso!";
                    _context.Update(convidado);
                    _context.SaveChanges();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ConfirmarConvites: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
            return View("Error");
        }
        public bool Desativar(int id) 
        {
            var convite = _context.Convites.Find(id);
            if (convite != null)
            {
                try
                {
                    convite.Ingressos = _context.Ingressos.Where(s => s.codConvite == id).ToList();
                    foreach (var item in convite.Ingressos)
                    {
                        item.codConvite = null;
                        item.tipoVendaIngresso = "Não Vendido!";
                    }
                    convite.ativo = false;
                    _context.Update(convite);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar DesativarConvites: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return false;
                }
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return false;
            }
        }

        // GET: Convites/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var convite = await _context.Convites
        //        .Include(c => c.Evento)
        //        .FirstOrDefaultAsync(m => m.codConvite == id);
        //    if (convite == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(convite);
        //}

        //// POST: Convites/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var convite = await _context.Convites.FindAsync(id);
        //    if (convite != null)
        //    {
        //        _context.Convites.Remove(convite);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ConviteExists(int id)
        {
            return _context.Convites.Any(e => e.codConvite == id);
        }
    }
}

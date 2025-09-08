using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using X.PagedList.Extensions;
using BixWeb.Services;
using System.Drawing;
using System.IO.Compression;
using System.Net;
using BixWeb.ViewModel;
using System.Text.Json;
using System.Text;
using Microsoft.Identity.Client;
using System.Drawing.Imaging;
using Azure;
using System.Globalization;

namespace BixWeb.Controllers
{
    public class CampanhasController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;
        private readonly IPageGeneratorService1 _pageGeneratorService1;
        public CampanhasController(DbPrint context, IPageGeneratorService pageGeneratorService, IPageGeneratorService1 pageGeneratorService1)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
            _pageGeneratorService1 = pageGeneratorService1;
        }

        // GET: Campanhas
        [Authorize(Roles = "Gerente, Funcionario")]
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
                var campanhas = from s in _context.Campanhas
                             select s;

                if (userId != null) 
                {
                    int CodUsuario = int.Parse(userId);
                    campanhas = _context.Campanhas  // Supondo que a navegação está configurada
                        .Include(c => c.Usuariofilial.Filial)   // Se tiver uma entidade Filial relacionada
                        .Where(c => _context.UsuarioFiliais
                                        .Any(uf => uf.codUsuario == CodUsuario &&
                                                   uf.codFilial == c.codFilial) ||
                                    c.codUsuario == CodUsuario);

                    if (campanhas.Any()) 
                    {
                        if (!System.String.IsNullOrEmpty(searchString))
                        {
                            campanhas = campanhas.Where(s => s.nomeCampanha.Contains(searchString)
                                                      || s.Usuariofilial.Filial.nome.Contains(searchString));
                        }
                        switch (sortOrder)
                        {
                            case "name_desc":
                                campanhas = campanhas.OrderByDescending(s => s.nomeCampanha);
                                break;
                            default:  // Name ascending 
                                campanhas = campanhas.OrderBy(s => s.nomeCampanha);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(campanhas.ToPagedList(pageNumber, pageSize));
                    } else 
                    {
                        ViewData["Campanha"] = "Nenhuma campanha encontrada!";
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
                ErrorViewModel.LogError($"Erro ao chamar IndexCampanhas: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public IActionResult ViewProduto(int? id) 
        {
            if (id!=null)
            {
                try
                {
                    var precos =_context.Precos.Where(s => s.codFilial == id && s.Produto.Categoria.ativo == true).Include(s => s.Produto).Include(s => s.Produto.Categoria).ToList();
                    precos = precos.OrderBy(s => s.Produto.nomeProduto).ToList();
                    if (precos.Any())
                    {
                        return PartialView(precos);
                    }
                    else
                    {
                        ViewData["produto"] = "Não foi encontrado nenhum produto!";
                        return PartialView();
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar ViewProduto: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            else
            {
                return View();
            }
        }
        // GET: Campanhas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campanha = await _context.Campanhas
                .Include(c => c.Usuariofilial)
                .FirstOrDefaultAsync(m => m.codCampanha == id);
            if (campanha == null)
            {
                return NotFound();
            }

            return View(campanha);
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        // GET: Campanhas/Create
        public IActionResult Create()
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId!=null)
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
                ErrorViewModel.LogError($"Erro ao chamar ViewCreateCampanhas: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        // POST: Campanhas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codCampanha,nomeCampanha,dataPostCampanha,campanhaAtiva,matriculaFuncionario,linkCardapio,dataCardapio,tipoCardapio,typeServise,codFilial,codUsuario")] Campanha campanha, List<ProdutoCampanha> ProdutoCampanha)
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int CodUsuario;
                if (userId != null)
                {
                    CodUsuario = int.Parse(userId);
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();
                var matricula= _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario && uf.codFilial==campanha.codFilial)
                        .Select(uf => uf.token).FirstOrDefault();

                if (ProdutoCampanha!=null)
                {
                    ProdutoCampanha = ProdutoCampanha.Where(s=>s.codProduto!=0).ToList();
                    if (ProdutoCampanha.Any())
                    {
                        if (ModelState.IsValid)
                        {
                            if (campanha.campanhaAtiva==true) 
                            {
                                var veryCampanha = _context.Campanhas.Where(s => s.codFilial == campanha.codFilial && s.campanhaAtiva==true).FirstOrDefault();
                                if (veryCampanha != null)
                                {
                                    veryCampanha.campanhaAtiva = false;
                                    veryCampanha.linkCardapio = "";
                                    veryCampanha.tipoCardapio = 0;
                                    string diretorio = Directory.GetCurrentDirectory();

                                    diretorio = diretorio + "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/";
                                    if (Directory.Exists(diretorio)) 
                                    {
                                        Directory.Delete(diretorio, true );
                                    }
                                    _context.Update(veryCampanha);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            campanha.codUsuario = CodUsuario;
                            campanha.dataPostCampanha= DateTime.Now;
                            campanha.produtosCampanha = ProdutoCampanha;
                            campanha.matriculaFuncionario= matricula != null ? matricula : string.Empty; ;
                            _context.Add(campanha);
                            await _context.SaveChangesAsync();
                            if (campanha.campanhaAtiva)
                            {
                                return RedirectToAction("Cardapio", new { id = campanha.codCampanha });
                            }
                            else
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            
                        }
                    }
                    ViewData["Produto"] = "Nenhum Produto Selecionado!";
                }
                

                ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");
                return View(campanha);

            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar postCreateCampanha: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Cardapio(int id)
        {
            return View();
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        [HttpPost]
        public IActionResult Cardapio(int id, int tipoCardapio)
        {
            try
            {
                var campanha = _context.Campanhas.Where(c => c.codCampanha == id).Include(c => c.produtosCampanha)
                    .ThenInclude(p => p.Produto).ThenInclude(prod => prod.Categoria).FirstOrDefault();
                
                if (tipoCardapio > 0) 
                {
                    if (campanha != null)
                    {
                        string diretorio = Directory.GetCurrentDirectory();

                        diretorio = diretorio + "/Empresas/" + campanha.codFilial.ToString()+"/Cardapio/";
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        baseUrl += "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/Index.html";
                        campanha.linkCardapio= baseUrl;
                        campanha.tipoCardapio = tipoCardapio;
                        
                        switch (tipoCardapio)
                        {
                            case 1:
                                string origem = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "ModeloCardapio", "Cardapio");
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
                                if (_pageGeneratorService.GenerateHtml1(diretorio, campanha.codCampanha, 1))
                                {
                                    ViewData["Sucesso"] = "Cardapio criado com sucesso";
                                    ViewData["Link"] = campanha.linkCardapio;
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
                                if (_pageGeneratorService.GenerateHtml2(diretorio, campanha.codCampanha,1))
                                {
                                    ViewData["Sucesso"] = "Cardapio criado com sucesso";
                                    ViewData["Link"] = campanha.linkCardapio;
                                }
                                else
                                {
                                    ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                }
                                break;
							case 3:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio3");
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
								if (_pageGeneratorService.GenerateHtml3(diretorio, campanha))
								{
									ViewData["Sucesso"] = "Cardapio criado com sucesso";
									ViewData["Link"] = campanha.linkCardapio;
								}
								else
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
								}
								break;
							case 4:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio4");
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
								if (_pageGeneratorService.GenerateHtml4(diretorio, campanha))
								{
									ViewData["Sucesso"] = "Cardapio criado com sucesso";
									ViewData["Link"] = campanha.linkCardapio;
								}
								else
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
								}
								break;
							case 5:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio5");
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
								if (_pageGeneratorService.GenerateHtml5(diretorio, campanha))
								{
									ViewData["Sucesso"] = "Cardapio criado com sucesso";
									ViewData["Link"] = campanha.linkCardapio;
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
								if (_pageGeneratorService.GenerateHtml6(diretorio, campanha.codCampanha, 1))
								{
									ViewData["Sucesso"] = "Cardapio criado com sucesso";
									ViewData["Link"] = campanha.linkCardapio;
								}
								else
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
								}
								break;
						}

                        _context.Update(campanha);
                        _context.SaveChanges();
                        return View();
                    }
                    else
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                }
                else if (tipoCardapio==0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar CardapioCampanhas: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // GET: Campanhas/Edit/5
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            var campanha = await _context.Campanhas.FindAsync(id);
            if (campanha == null)
            {
                return NotFound();
            }
            var filial = _context.Filiais.Where(s=>s.codFilial==campanha.codFilial);
            ViewData["codFilial"] = new SelectList(filial, "codFilial", "nome", campanha.codFilial);
            return View(campanha);
        }
		[Authorize(Roles = "Gerente, Funcionario")]
		public IActionResult CardapioImpresso(int id, int formato) 
        {
            ViewBag.Formato = formato;
            ViewBag.Id = id;
            return View();
        }
		[Authorize(Roles = "Gerente, Funcionario")]
		public IActionResult ProdutosCardapio(int id, int formato, int tipo) 
        {
			ViewBag.Id = id;
			ViewBag.Tipo = tipo;
			ViewBag.Formato= formato;
			try
			{
				var produtos = _context.ProdutosCampanha.Where(s => s.codCampanha == id).Include(s => s.Produto).ThenInclude(s => s.Categoria).OrderBy(s => s.Produto.nomeProduto).ThenBy(s => s.Produto.Categoria.nome);
                
				if (produtos.Any())
				{
					return View(produtos);
				}
				else
				{
					ViewData["produto"] = "Não foi encontrado nenhum produto!";
					return View();
				}
			}
			catch (Exception ex)
			{
				ErrorViewModel.LogError($"Erro ao chamar Produtos em CampanhasController: {ex}");
				ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
				return View("Error");
			}
		}
        [HttpPost]
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult ProdutosCardapio(int id, int formato, int tipo, List<ProdutoCampanha> ProdutoCampanha, List<IFormFile> Anuncios)
        {
            ViewBag.Id = id;
            ViewBag.Tipo = tipo;
            ViewBag.Formato = formato;
            ProdutoCampanha = ProdutoCampanha.Where(s => s.codProduto != 0).ToList();
            
            if (ProdutoCampanha.Any())
            {
                try
                {
                    foreach (var produto in ProdutoCampanha)
                    {
                        produto.Produto = _context.Produtos.Where(s => s.codProduto == produto.codProduto).Include(s => s.Categoria).OrderBy(s=>s.nomeProduto).FirstOrDefault();
                    }
                    var campanha = _context.Campanhas.Where(s => s.codCampanha == id).Include(s => s.Usuariofilial).ThenInclude(s => s.Filial).ThenInclude(s=>s.Endereco).FirstOrDefault();
                    if (campanha == null)
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                    else
                    {
                        string diretorio = Directory.GetCurrentDirectory();
                        diretorio = diretorio + "/Empresas/" + campanha.codFilial;
                        if (!Directory.Exists(diretorio + "/Imagens"))
                        {
                            Directory.CreateDirectory(diretorio + "/Imagens");
                        }
                        if (formato == 1)
                        {
                            string path1 = _pageGeneratorService.GerarPDF(campanha,ProdutoCampanha,Anuncios,tipo,diretorio);
                            string contentType = "application/pdf";
                            string fileName = Path.GetFileName(path1);
                            return File(System.IO.File.ReadAllBytes(path1), contentType, fileName);
                            
                        }
                        else
                        {
                            string path1 = _pageGeneratorService.GerarIMG(campanha, ProdutoCampanha, Anuncios, tipo, diretorio);
                            string contentType = "application/zip";
                            string fileName = Path.GetFileName(path1);
                            return File(System.IO.File.ReadAllBytes(path1), contentType, fileName);                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar ProdutosCardapio em CampanhasController: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            else
            {
                try
                {
                    var produtos = _context.ProdutosCampanha.Where(s => s.codCampanha == id).Include(s => s.Produto).ThenInclude(s => s.Categoria).OrderBy(s => s.Produto.nomeProduto).ThenBy(s => s.Produto.Categoria.nome);

                    if (produtos.Any())
                    {
                        ViewData["produto"] = "Você deve selecionar os produtos!";
                        return View(produtos);
                    }
                    else
                    {
                        ViewData["produto"] = "Não foi encontrado nenhum produto!";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar ProdutosCardapio em CampanhasController: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
        }
        // POST: Campanhas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Edit(int id, [Bind("codCampanha,nomeCampanha,dataPostCampanha,campanhaAtiva,matriculaFuncionario,linkCardapio,dataCardapio,tipoCardapio,typeServise,codFilial,codUsuario")] Campanha campanha)
        {
            if (id != campanha.codCampanha)
            {
                return NotFound();
            }
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int CodUsuario;
            if (userId != null)
            {
                CodUsuario = int.Parse(userId);
            }
            else
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }

            campanha.dataPostCampanha=DateTime.Now;
            campanha.codUsuario=CodUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    if (campanha.campanhaAtiva == true && campanha.tipoCardapio > 0)
                    {
                        string diretorio = Directory.GetCurrentDirectory();

                        diretorio = diretorio + "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/Campanha/";
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        baseUrl += "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/Campanha/Index.html";
                        campanha.linkCardapio = baseUrl;

                        switch (campanha.tipoCardapio)
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
                                if (!_pageGeneratorService.GenerateHtml1(diretorio, campanha.codCampanha, 1))
                                {
                                    ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                    return View("Error");
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
                                if (!_pageGeneratorService.GenerateHtml2(diretorio, campanha.codCampanha, 1))
                                {
                                    ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
                                    return View("Error");
                                }
                                break;
							case 3:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio3");
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
								if (!_pageGeneratorService.GenerateHtml3(diretorio, campanha))
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
									return View("Error");
								}
								break;
							case 4:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio4");
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
								if (!_pageGeneratorService.GenerateHtml4(diretorio, campanha))
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
									return View("Error");
								}
								break;
							case 5:
								origem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModeloCardapio", "Cardapio5");
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
								if (!_pageGeneratorService.GenerateHtml5(diretorio, campanha))
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
									return View("Error");
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
								if (!_pageGeneratorService.GenerateHtml6(diretorio, campanha.codCampanha, 1))
								{
									ViewData["Erro"] = "Algum erro inesperado, não foi possível criar o cardápio.";
									return View("Error");
								}
								break;
						}
                    }
                    else
                    {
                        string diretorio = Directory.GetCurrentDirectory();
                        diretorio = diretorio + "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/Campanha/";
                        if (Directory.Exists(diretorio))
                        {
                            Directory.Delete(diretorio,true);
                        }
                        campanha.linkCardapio = "";
                        campanha.tipoCardapio = 0;
                    }
                    _context.Update(campanha);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar EditCampanhas: {ex}");
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                return RedirectToAction(nameof(Index));
            }
            var filial = _context.Filiais.Where(s => s.codFilial == campanha.codFilial);
            ViewData["codFilial"] = new SelectList(filial, "codFilial", "nome", campanha.codFilial);

            return View(campanha);
        }
        // GET: Campanhas/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var campanha = await _context.Campanhas
        //        .Include(c => c.Usuariofilial)
        //        .FirstOrDefaultAsync(m => m.codCampanha == id);
        //    if (campanha == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(campanha);
        //}

        // POST: Campanhas/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<bool> DeleteConfirmed(int id)
        {
            var campanha = await _context.Campanhas.Where(s=>s.codCampanha==id).Include(s=>s.produtosCampanha).FirstOrDefaultAsync();
            if (campanha == null)
            {
                return false;
            }
            try
            {
                if (campanha.tipoCardapio > 0)
                {
                    string diretorio = Directory.GetCurrentDirectory();
                    diretorio = diretorio + "/Empresas/" + campanha.codFilial.ToString() + "/Cardapio/";
                    if (Directory.Exists(diretorio))
                    {
                        Directory.Delete(diretorio, true);
                    }
                }
                _context.Campanhas.Remove(campanha);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar DeleteCampanhas: {ex}");
                return false;
            }
        }
        // GET: Campanhas/Edit/5
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Anuncios(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Produtos(int id, int? erro) 
        {
            ViewBag.Id = id;
            try
            {
                var produtos = _context.ProdutosCampanha.Where(s => s.codCampanha == id).Include(s => s.Produto).ThenInclude(s => s.Categoria).OrderBy(s => s.Produto.nomeProduto).ThenBy(s => s.Produto.Categoria.nome);

                if (produtos.Any())
                {
                    if (erro==1)
                    {
                        ViewData["Error"] = "Ops! Você deve selecionar ao menos um produto!";
                    }
                    else if (erro==2) 
                    {
                        ViewData["Error"] = "Ops! Você não tem nenhum cardápio cadastrado nessa campanha!";
                    }
                    return View(produtos);
                }
                else
                {
                    ViewData["produto"] = "Não foi encontrado nenhum produto!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Produtos em CampanhasController: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public async Task<IActionResult> GerarAnuncioIA(int id, List<ProdutoCampanha> ProdutoCampanha, int Resolucao) 
        {
            ProdutoCampanha = ProdutoCampanha.Where(s => s.codProduto != 0).ToList();

            if (ProdutoCampanha.Any())
            {
                var campanha = _context.Campanhas.Where(s => s.codCampanha == id).Include(s => s.Usuariofilial).ThenInclude(s => s.Filial).FirstOrDefault();
                if (campanha == null)
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                string diretorio = Directory.GetCurrentDirectory();
                diretorio = diretorio + "/Empresas/" + campanha.codFilial;

                if (!Directory.Exists(diretorio + "/Imagens"))
                {
                    Directory.CreateDirectory(diretorio + "/Imagens");
                }
                foreach (var produto in ProdutoCampanha)
                {
                    produto.Produto = _context.Produtos.Where(s => s.codProduto == produto.codProduto).FirstOrDefault();
                    string path = diretorio + "/Imagens/" + produto.Produto.nomeProduto + ".jpg";
                    var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.valor);
                    string prompt =await _pageGeneratorService1.ObterPromptImagem(produto.Produto.nomeProduto,produto.Produto.descricaoDetalhada,valorFormatado);
                    string imageUrl = await _pageGeneratorService1.GerarImagens(prompt);
                    if (imageUrl!=null && imageUrl!="")
                    {
                        BaixarEGuardarImagem(imageUrl, path, campanha.Usuariofilial.Filial.logoHome);
                    }
                    else
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                }
                if (System.IO.File.Exists(diretorio + "/ImagensProdutos.zip"))
                {
                    System.IO.File.Delete(diretorio + "/ImagensProdutos.zip");
                    ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/ImagensProdutos.zip");
                }
                else
                {
                    ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/ImagensProdutos.zip");
                }
                Directory.Delete(diretorio + "/Imagens", true);
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                var pathToTheFile = Path.Combine(baseUrl, "Empresas", campanha.codFilial.ToString(), "ImagensProdutos.zip"); // exemplo de caminho
                var mimeType = "application/zip"; // Tipo MIME para .txt. Altere conforme o tipo de arquivo.

                // Obter o nome do arquivo para ser exibido no download:
                var fileDownloadName = Path.GetFileName(pathToTheFile);

                var fileBytes = System.IO.File.ReadAllBytes(diretorio + "/ImagensProdutos.zip");

                return File(fileBytes, mimeType, fileDownloadName);
            }
                return RedirectToAction("Produtos", new { id = id, tipo = 1, erro = 1 });
        }
        static async Task BaixarEGuardarImagem(string imageUrl, string savePath, string logoURL)
        {
            if (logoURL!=null)
            {

                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap image = new Bitmap(stream);

                WebClient client1 = new WebClient();
                Stream stream1 = client1.OpenRead(logoURL);
                Bitmap logo = new Bitmap(stream1);

                using (Bitmap logoRedimensionada = RedimensionarLogo(logo, 175))
                using (Graphics canvas = Graphics.FromImage(image))
                {
                    // Define a posição da logo (canto inferior direito)
                    int x = image.Width - logoRedimensionada.Width;
                    int y = image.Height - logoRedimensionada.Height;

                    // Sobrepõe a logo na imagem principal
                    canvas.DrawImage(logoRedimensionada, x, y, logoRedimensionada.Width, logoRedimensionada.Height);

                    // Salva a imagem editada
                    image.Save(savePath, ImageFormat.Jpeg);
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
                    await System.IO.File.WriteAllBytesAsync(savePath, imageBytes);
                }
            }
           
        }
        static Bitmap RedimensionarLogo(Bitmap logo, int alturaMaxima)
        {
            if (logo.Height <= alturaMaxima)
            {
                return new Bitmap(logo); // Retorna a logo original se já estiver dentro do limite
            }

            // Calcula nova largura mantendo a proporção
            double proporcao = (double)alturaMaxima / logo.Height;
            int novaLargura = (int)(logo.Width * proporcao);
            int novaAltura = alturaMaxima;

            // Cria nova imagem redimensionada
            Bitmap novaLogo = new Bitmap(novaLargura, novaAltura);
            using (Graphics g = Graphics.FromImage(novaLogo))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(logo, 0, 0, novaLargura, novaAltura);
            }

            return novaLogo;
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GerarAnuncios(int id, int tipo, List<ProdutoCampanha> ProdutoCampanha, int Resolucao)
        {
            ProdutoCampanha = ProdutoCampanha.Where(s => s.codProduto != 0).ToList();
            if (ProdutoCampanha.Any())
            {
                var campanha = _context.Campanhas.Where(s=>s.codCampanha==id).Include(s=>s.Usuariofilial).ThenInclude(s=>s.Filial).FirstOrDefault();
                if (campanha == null) {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                string diretorio = Directory.GetCurrentDirectory();
                diretorio = diretorio + "/Empresas/" + campanha.codFilial;
                if (!Directory.Exists(diretorio + "/Imagens"))
                {
                    Directory.CreateDirectory(diretorio + "/Imagens");
                }
                List<string> listaImagens = new List<string>();
                foreach (var produto in ProdutoCampanha)
                {
                    Anuncio anuncio = new Anuncio();
                    produto.Produto = _context.Produtos.Where(s => s.codProduto == produto.codProduto).FirstOrDefault();
                    switch (Resolucao)
                    {
                        case 720:

                            anuncio.largura = 1280;
                            anuncio.altura = 720;
                            anuncio.fontValor = 55;
                            anuncio.fontProd = 47;

                            if (produto.Produto.nomeProduto.Length > 30 && produto.Produto.nomeProduto.Length < 38)
                            {
                                anuncio.fontProd = 41;
                            }
                            if (produto.Produto.nomeProduto.Length >= 38)
                            {
                                anuncio.fontProd = 38;
                            }
                            break;
                        case 1080:

                            anuncio.largura = 1920;
                            anuncio.altura = 1080;
                            anuncio.fontValor = 86;
                            anuncio.fontProd = 76;

                            if (produto.Produto.nomeProduto.Length > 30 && produto.Produto.nomeProduto.Length < 38)
                            {
                                anuncio.fontProd = 60;
                            }
                            if (produto.Produto.nomeProduto.Length >= 38)
                            {
                                anuncio.fontProd = 50;
                            }
                            break;
                        case 2040:

                            anuncio.largura = 2560;
                            anuncio.altura = 1440;
                            anuncio.fontValor = 108;
                            anuncio.fontProd = 96;
                            if (produto.Produto.nomeProduto.Length > 30 && produto.Produto.nomeProduto.Length < 38)
                            {
                                anuncio.fontProd = 86;
                            }
                            if (produto.Produto.nomeProduto.Length >= 38)
                            {
                                anuncio.fontProd = 70;
                            }
                            break;
                    }
                    anuncio.caminho = diretorio + "/Imagens/" + produto.Produto.nomeProduto + ".jpg";

                    if (produto.Produto.imagem != "")
                    {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead(produto.Produto.imagem);
                        anuncio.produto = new Bitmap(stream);
                    }
                    if (campanha.Usuariofilial.Filial.logoPDV != "")
                    {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead(campanha.Usuariofilial.Filial.logoPDV);
                        anuncio.logo = new Bitmap(stream);
                    }
                    switch (tipo)
                    {
                        case 1:
                            _pageGeneratorService.Layout1(anuncio, produto);
                            break;
                        case 2:
                            int fontAce = 20;

                            if (campanha.tipoCardapio == 0)
                            {
                                ViewBag.erro = "Você precisa ter um cardápio HTML ativo!";
                                return RedirectToAction("Produtos", new { id = id, tipo = tipo, erro = 2 });
                            }
                            _pageGeneratorService.Layout2(anuncio, fontAce, produto, campanha.linkCardapio);
                            break;
                        case 3:
                            _pageGeneratorService.Layout3(anuncio, produto);
                            break;
                        case 4:
                            _pageGeneratorService.Layout4(anuncio, produto);
                            break;
                    }
                }
                if (listaImagens != null)
                {
                    foreach (var item in listaImagens)
                    {
                        System.IO.File.Delete(item);
                    }
                }
                if (System.IO.File.Exists(diretorio + "/ImagensProdutos.zip"))
                {
                    System.IO.File.Delete(diretorio + "/ImagensProdutos.zip");
                    ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/ImagensProdutos.zip");
                }
                else
                {
                    ZipFile.CreateFromDirectory(diretorio + "/Imagens", diretorio + "/ImagensProdutos.zip");
                }
                Directory.Delete(diretorio + "/Imagens", true);

                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                var pathToTheFile = Path.Combine(baseUrl, "Empresas", campanha.codFilial.ToString(), "ImagensProdutos.zip"); // exemplo de caminho
                var mimeType = "application/zip"; // Tipo MIME para .txt. Altere conforme o tipo de arquivo.

                // Obter o nome do arquivo para ser exibido no download:
                var fileDownloadName = Path.GetFileName(pathToTheFile);

                var fileBytes = System.IO.File.ReadAllBytes(diretorio + "/ImagensProdutos.zip");

                return File(fileBytes, mimeType, fileDownloadName);
            }
            else
            {
                return RedirectToAction("Produtos", new { id = id , tipo=tipo, erro=1});
            }
        }
        private bool CampanhaExists(int id)
        {
            return _context.Campanhas.Any(e => e.codCampanha == id);
        }

        
    }
}

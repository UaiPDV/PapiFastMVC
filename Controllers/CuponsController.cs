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
using System.Linq.Expressions;
using X.PagedList.Extensions;
using BixWeb.Services;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BixWeb.Controllers
{
    public class CuponsController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;
        public CuponsController(DbPrint context, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
        }

        // GET: Cupons
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

				if (userId != null)
				{
					int CodUsuario = int.Parse(userId);
					var cupons = _context.Cupons  // Supondo que a navegação está configurada
						.Include(c => c.UsuarioFilial.Filial).Include(s=>s.usuario)   // Se tiver uma entidade Filial relacionada
						.Where(c => _context.UsuarioFiliais
										.Any(uf => uf.codUsuario == CodUsuario &&
												   uf.codFilial == c.codFilial) ||
									c.codCriador == CodUsuario);

					if (cupons.Any())
					{
						foreach (var c in cupons)
						{
							if (c.produtos != null && c.produtos != "")
							{
								int qtd = c.produtos.Count(c => c == ',');

								c.produtos = qtd.ToString();
							}

						}
						if (!System.String.IsNullOrEmpty(searchString))
						{
							cupons = cupons.Where(s => s.tokenCupom.Contains(searchString)
													  || s.usuario.nome.Contains(searchString));
						}
						switch (sortOrder)
						{
							case "name_desc":
								cupons = cupons.OrderByDescending(s => s.validadeCupom);
								break;
							default:  // Name ascending 
								cupons = cupons.OrderBy(s => s.validadeCupom);
								break;
						}

						int pageSize = 10;
						int pageNumber = (page ?? 1);
						return View(cupons.ToPagedList(pageNumber, pageSize));
					}
					else
					{
						ViewData["cupom"] = "Nenhuma cupom cadastrado!";
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
		[Authorize(Roles = "Cliente")]
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
					int CodUsuario = int.Parse(userId);
					
                    var cupons = _context.Cupons.Where(s => s.codcliente == CodUsuario).Include(s => s.usuario).AsEnumerable();
					if (cupons.Any())
					{
						foreach (var c in cupons)
						{
							if (c.produtos != null && c.produtos != "")
							{
								int qtd = c.produtos.Count(c => c == ',');

								c.produtos = qtd.ToString();
							}

						}
						if (!System.String.IsNullOrEmpty(searchString))
						{
							cupons = cupons.Where(s => s.tokenCupom.Contains(searchString)
													  || s.usuario.nome.Contains(searchString));
						}
						switch (sortOrder)
						{
							case "name_desc":
								cupons = cupons.OrderByDescending(s => s.validadeCupom);
								break;
							default:  // Name ascending 
								cupons = cupons.OrderBy(s => s.validadeCupom);
								break;
						}

						int pageSize = 10;
						int pageNumber = (page ?? 1);
						return View(cupons.ToPagedList(pageNumber, pageSize));
					}
					else
					{
						ViewData["cupom"] = "Nenhuma Cupom encontrado!";
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
		// GET: Cupons/Details/5
		[Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cupom = await _context.Cupons
                .Include(c => c.Campanha)
                .Include(c => c.UsuarioFilial)
                .Include(c => c.usuario)
                .FirstOrDefaultAsync(m => m.codCupom == id);
            if (cupom == null)
            {
                return NotFound();
            }

            return View(cupom);
        }

        // GET: Cupons/Create
        [Authorize(Roles = "Gerente, Funcionario")]
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
                    if (filiais.Count() <= 1)
                    {
                        foreach (var filia in filiais)
                        {
                            ViewData["codCampanha"] = new SelectList(_context.Campanhas.Where(s=>s.codFilial==filia.codFilial), "codFilial", "nome");
                        }
                    }
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
                ErrorViewModel.LogError($"Erro ao chamar ViewCreateCupons: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public JsonResult ListaCampanhas(int id)
        {
            return Json(_context.Campanhas.Where(s => s.codFilial == id).Select(s => new
            {
                codCampanha = s.codCampanha,
                nomeCampanha = s.nomeCampanha
            }).ToList());
        }
        public IActionResult ViewProduto(int? codCampanha, int codfilial )
        {
            if (codCampanha== null)
            {
                try
                {
                    
                    var precos = _context.Precos.Where(s => s.codFilial == codfilial && s.Produto.Categoria.ativo == true).Include(s => s.Produto).Include(s => s.Produto.Categoria).ToList();
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
                var produtosCampa = _context.ProdutosCampanha.Where(s => s.codCampanha == codCampanha && s.Produto.Categoria.ativo == true).Include(s => s.Produto).Include(s => s.Produto.Categoria).ToList();
                produtosCampa = produtosCampa.OrderBy(s => s.Produto.nomeProduto).ToList();
                if (produtosCampa.Any())
                {
                    return PartialView("ProdutoCampanha",produtosCampa);
                }
                else
                {
                    ViewData["produto"] = "Não foi encontrado nenhum produto!";
                    return PartialView();
                }
            }
        }
        public IActionResult EnviarCupom(int id, string email, string nome)
        {
			var cupon = _context.Cupons.Where(s => s.codCupom == id).FirstOrDefault();
            string diretorio = Directory.GetCurrentDirectory();
            if (cupon!=null)
            {
                diretorio = diretorio + "/Empresas/" + cupon.codFilial+"/Cupons/";
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                string link= $"{this.Request.Scheme}://{this.Request.Host}";
                link += "/Cupons/Validar/"+id+"?filial="+cupon.codFilial;
                if (_pageGeneratorService.GerarCupon(id, diretorio, link))
                {
                    if (_pageGeneratorService.EnviarCupom(diretorio + cupon.tokenCupom + "-" + cupon.codCupom.ToString() + ".png", email, nome))
                    {
                        var cliente = _context.Usuario.Where(s => s.email == email).FirstOrDefault();
                        if (cliente != null)
                        {
                            cupon.codcliente = cliente.codUsuario;
                            cupon.usuario = cliente;
                            _context.Update(cupon);
                            _context.SaveChangesAsync();
                        }
                        return RedirectToAction("Index", new { formSubmitted = true });
					}
                }
			}
			return RedirectToAction("Index", new { formSubmitted = false });
		}
        public async Task<IActionResult> EnviarCupomWhats(int id, string telefone, string nome)
        {
            var cupon = _context.Cupons.Where(s => s.codCupom == id).FirstOrDefault();
            string diretorio = Directory.GetCurrentDirectory();
            if (cupon != null)
            {
                diretorio = diretorio + "/Empresas/" + cupon.codFilial + "/Cupons/";
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                string link = $"{this.Request.Scheme}://{this.Request.Host}";
                link += "/Cupons/Validar/" + id + "?filial=" + cupon.codFilial;
                var filial = _context.Filiais.Find(cupon.codFilial);
                var urlimagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/Empresas/" + cupon.codFilial + "/Cupons/" + cupon.tokenCupom + ".png";
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string token;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    int CodUsuario = int.Parse(userId);
                    token= _context.Usuario.Where(s=>s.codUsuario==CodUsuario).Select(s => s.token).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return RedirectToAction("Index", new { formSubmitted = false });
                    }
                }
                else
                {
                    return RedirectToAction("Index", new { formSubmitted = false });
                }
                if (_pageGeneratorService.GerarCupon(id, diretorio, link))
                {
                    var produtos = cupon.produtos.Replace(",", "\n");
                    var validade = cupon.validadeCupom.ToString("dd/MM/yyyy HH:mm");
                    if (await _pageGeneratorService.EnviarCupomWhats(urlimagem, telefone, nome, filial.nome, token, produtos, validade))
                    {
                        var cliente = _context.Usuario.Where(s => s.telefone == telefone).FirstOrDefault();
                        if (cliente != null)
                        {
                            cupon.codcliente = cliente.codUsuario;
                            cupon.usuario = cliente;
                            _context.Update(cupon);
                            await _context.SaveChangesAsync();
                        }
                        return RedirectToAction("Index", new { formSubmitted = true });
                    }
                }
            }
            return RedirectToAction("Index", new { formSubmitted = false });
        }
        // POST: Cupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Create([Bind("codCupom,codCriador,codFilial,descontoCupom,validadeCupom,statusCupom,usadoCupom,tokenCupom,codCampanha,produtos,codcliente")] Cupom cupom, List<ProdutoCampanha> ProdutoCampanha, int qtdCupons)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                try
                {
                    int CodUsuario = int.Parse(userId);
                    ProdutoCampanha = ProdutoCampanha.Where(s=>s.codProduto!=0).ToList();

                    if (ProdutoCampanha.Any())
                    {
                        if (qtdCupons == 0) 
                        {
                            ViewData["qtdCupons"] = "A quantidade de ser maior que 0!";
                        } 
                        else
                        {
                            cupom.codCriador = CodUsuario;
                            cupom.statusCupom = true;
                            foreach (var item in ProdutoCampanha) 
                            {
                                cupom.produtos+=item.codProduto.ToString()+",";
                            }
                            if (ModelState.IsValid)
                            {
                                for (int i =0; i < qtdCupons; i++)
                                {
                                    cupom.codCupom = 0;
                                    _context.Add(cupom);
                                    await _context.SaveChangesAsync();
                                }
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }
                    else
                    {
                        ViewData["produto"] = "Nenhum produto Selecionado!";
                    }

                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();
                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome");
                    ViewData["codCampanha"] = new SelectList(_context.Campanhas.Where(s=>s.codFilial==cupom.codFilial), "codCampanha", "nomeCampanha", cupom.codCampanha);
                    
                    return View(cupom);
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar ViewCreateCampanhas: {ex}");
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

        // GET: Cupons/Edit/5
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cupom = await _context.Cupons.FindAsync(id);
            if (cupom == null)
            {
                return NotFound();
            }
            ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", cupom.codCampanha);
            ViewData["codCriador"] = new SelectList(_context.UsuarioFiliais, "codUsuario", "codUsuario", cupom.codCriador);
            ViewData["codcliente"] = new SelectList(_context.Usuario, "codUsuario", "email", cupom.codcliente);
            return View(cupom);
        }

        // POST: Cupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Edit(int id, [Bind("codCupom,codCriador,codFilial,descontoCupom,validadeCupom,statusCupom,usadoCupom,tokenCupom,codCampanha,produtos,codcliente")] Cupom cupom)
        {
            if (id != cupom.codCupom)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cupom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CupomExists(cupom.codCupom))
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
            ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", cupom.codCampanha);
            ViewData["codCriador"] = new SelectList(_context.UsuarioFiliais, "codUsuario", "codUsuario", cupom.codCriador);
            ViewData["codcliente"] = new SelectList(_context.Usuario, "codUsuario", "email", cupom.codcliente);
            return View(cupom);
        }

        // GET: Cupons/Delete/5
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cupom = await _context.Cupons
                .Include(c => c.Campanha)
                .Include(c => c.UsuarioFilial)
                .Include(c => c.usuario)
                .FirstOrDefaultAsync(m => m.codCupom == id);
            if (cupom == null)
            {
                return NotFound();
            }

            return View(cupom);
        }

        // POST: Cupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cupom = await _context.Cupons.FindAsync(id);
            if (cupom != null)
            {
                _context.Cupons.Remove(cupom);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CupomExists(int id)
        {
            return _context.Cupons.Any(e => e.codCupom == id);
        }
    }
}

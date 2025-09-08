using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BixWeb.Controllers
{
    public class ProdutosCampanhaController : Controller
    {
        private readonly DbPrint _context;

        public ProdutosCampanhaController(DbPrint context)
        {
            _context = context;
        }

        // GET: ProdutosCampanha
        [Authorize(Roles = "Gerente, Funcionario")]
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

                var produtosCampanha = _context.ProdutosCampanha.Where(s=>s.codCampanha==id).Include(s => s.Produto).ThenInclude(s => s.Categoria).AsQueryable();
                
                ViewData["codCampanha"]=id;

                if (produtosCampanha.Count() <= 0)
                {
                    ViewData["prodCamp"] = "Nenhum Produto encontrado!";
                    return View();
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    produtosCampanha = produtosCampanha.Where(s => s.Produto.descricaoDetalhada.Contains(searchString)
                                            || (s.valor).ToString().Contains(searchString)
                                            || s.Produto.Categoria.nome.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        produtosCampanha = produtosCampanha.OrderByDescending(s => s.Produto.nomeProduto);
                        break;
                    default:  // Name ascending 
                        produtosCampanha = produtosCampanha.OrderBy(s => s.Produto.nomeProduto);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(produtosCampanha.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexProdutoCampanha: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult GetProdutos(int id)
        {
            var subCategories = _context.Produtos
                .Where(s => s.codCategoria == id)
                .Select(s => new { s.codProduto, s.nomeProduto }).OrderBy(s=>s.nomeProduto)
                .ToList();

            return Json(subCategories);
        }
        // GET: ProdutosCampanha/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var produtoCampanha = await _context.ProdutosCampanha
        //        .Include(p => p.Campanha)
        //        .Include(p => p.Produto)
        //        .FirstOrDefaultAsync(m => m.codProdCamp == id);
        //    if (produtoCampanha == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(produtoCampanha);
        //}

        // GET: ProdutosCampanha/Create
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Create(int id)
        {
            var campanha =_context.Campanhas.Find(id);
            try
            {
                if (campanha != null)
                {
                    var produtos = _context.Precos.Where(s => s.codFilial == campanha.codFilial).Include(p => p.Produto).ThenInclude(s => s.Categoria);
                    var Categorias = produtos.GroupBy(p => p.Produto.Categoria).ToList();
                    List<Categoria> categoria= new List<Categoria>();
                    foreach (var item in Categorias)
                    {
                        categoria.Add(item.Key);
                    }
                    categoria= categoria.Where(s => s.ativo==true).ToList();
                    ViewBag.codCategoria = new SelectList(categoria.OrderBy(s=>s.nome), "codCategoria", "nome");
                    ViewData["codCampanha"] = id;

                }
                return View();
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar CreateProdutosCampanha: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        // POST: ProdutosCampanha/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Funcionario")]
        public async Task<IActionResult> Create([Bind("codProdCamp,codProduto,valor,codCampanha")] ProdutoCampanha produtoCampanha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtoCampanha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = produtoCampanha.codCampanha });
            }
            var produtos = _context.Precos.Where(s => s.codFilial == produtoCampanha.codCampanha).Include(p => p.Produto).ThenInclude(s => s.Categoria);
            var Categorias = produtos.GroupBy(p => p.Produto.Categoria).ToList();
            List<Categoria> categoria = new List<Categoria>();
            foreach (var item in Categorias)
            {
                categoria.Add(item.Key);
            }
            ViewBag.codCategoria = new SelectList(categoria, "codCategoria", "nome");
            ViewData["codCampanha"] = produtoCampanha.codCampanha;
            return View(produtoCampanha);
        }

        // GET: ProdutosCampanha/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var produtoCampanha = await _context.ProdutosCampanha.FindAsync(id);
        //    if (produtoCampanha == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", produtoCampanha.codCampanha);
        //    ViewData["codProduto"] = new SelectList(_context.Produtos, "codProduto", "codProduto", produtoCampanha.codProduto);
        //    return View(produtoCampanha);
        //}

        //// POST: ProdutosCampanha/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("codProdCamp,codProduto,valor,codCampanha")] ProdutoCampanha produtoCampanha)
        //{
        //    if (id != produtoCampanha.codProdCamp)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(produtoCampanha);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProdutoCampanhaExists(produtoCampanha.codProdCamp))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", produtoCampanha.codCampanha);
        //    ViewData["codProduto"] = new SelectList(_context.Produtos, "codProduto", "codProduto", produtoCampanha.codProduto);
        //    return View(produtoCampanha);
        //}

        //// GET: ProdutosCampanha/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var produtoCampanha = await _context.ProdutosCampanha
        //        .Include(p => p.Campanha)
        //        .Include(p => p.Produto)
        //        .FirstOrDefaultAsync(m => m.codProdCamp == id);
        //    if (produtoCampanha == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(produtoCampanha);
        //}

        // POST: ProdutosCampanha/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<bool> DeleteConfirmed(int id)
        {
            var produtoCampanha = await _context.ProdutosCampanha.FindAsync(id);
            if (produtoCampanha != null)
            {
                _context.ProdutosCampanha.Remove(produtoCampanha);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ProdutoCampanhaExists(int id)
        {
            return _context.ProdutosCampanha.Any(e => e.codProdCamp == id);
        }
    }
}

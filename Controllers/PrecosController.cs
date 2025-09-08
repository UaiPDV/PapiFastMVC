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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Reflection;

namespace BixWeb.Controllers
{
    public class PrecosController : Controller
    {
        private readonly DbPrint _context;

        public PrecosController(DbPrint context)
        {
            _context = context;
        }

        // GET: Precos
        public IActionResult Index(int id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {
                ViewBag.codFilial = id;
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

                var precos = from s in _context.Precos
                             select s;

                precos = precos.Where(s => s.codFilial == id && s.Produto.Categoria.ativo == true).Include(s => s.Produto).Include(s => s.Produto.Categoria);

                if (precos.Count()<= 0)
                {
                    ViewData["preco"] = "Nenhuma produto encontrado!";
                    return View();
                }
                
                
                if (!String.IsNullOrEmpty(searchString))
                {
                    precos = precos.Where(s => s.Produto.descricaoDetalhada.Contains(searchString)
                                            || (s.valor).ToString().Contains(searchString)
                                            || s.Produto.Categoria.nome.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        precos = precos.OrderByDescending(s => s.Produto.nomeProduto);
                        break;
                    default:  // Name ascending 
                        precos = precos.OrderBy(s => s.Produto.nomeProduto);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                
                return View(precos.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexFiliais: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        // GET: Precos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preco = await _context.Precos
                .Include(p => p.Filial)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.codPreco == id);
            if (preco == null)
            {
                return NotFound();
            }

            return View(preco);
        }

        // GET: Precos/Create
        public IActionResult Create(int id)
        {
            ViewBag.codFilial=id;
            ViewData["codCategoria"] = new SelectList(_context.Categorias.Where(s=>s.ativo==true), "codCategoria", "nome");
            return View();
        }

        // POST: Precos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codPreco,valor,codFilial,codProduto, Produto")] Preco preco, IFormFile? imagemProduto)
        {
            var produto = _context.Produtos.OrderByDescending(p => p.codProduto).FirstOrDefault();
            int cod = 1;
            if (produto!=null)
            {
                 cod = produto.codProduto + 1;
            }
            if (ModelState.IsValid)
            {
                if (imagemProduto!=null)
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    string diretorio = Directory.GetCurrentDirectory();

                    diretorio = diretorio + "/Empresas/"+preco.codFilial+"/Imagens/Produtos";

                    if (!Directory.Exists(diretorio))
                    {
                        Directory.CreateDirectory(diretorio);
                    }
                    var fileName = Path.GetFileName(imagemProduto.FileName);
                    string name = diretorio + "/"+cod+"-" + fileName;
                    using (var stream = new FileStream(name, FileMode.Create))
                    {
                        imagemProduto.CopyTo(stream);
                    }
                    preco.Produto.imagem= baseUrl + "/Empresas/"+preco.codFilial+"/Imagens/Produtos/" + +cod + "-" + fileName;
                }
                _context.Add(preco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {id=preco.codFilial });
            }
            ViewBag.codFilial = preco.codFilial;
            ViewData["codCategoria"] = new SelectList(_context.Categorias.Where(s => s.ativo == true), "codCategoria", "nome",preco.Produto.codCategoria);
            return View(preco);
        }

        // GET: Precos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preco = await _context.Precos.FindAsync(id);
            if (preco == null)
            {
                return NotFound();
            }
            preco.Produto = _context.Produtos.Find(preco.codProduto);
          
            ViewData["codCategoria"] = new SelectList(_context.Categorias.Where(s => s.ativo == true), "codCategoria", "nome",preco.Produto.codCategoria); 
            return View(preco);
        }

        // POST: Precos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codPreco,valor,codFilial,codProduto")] Preco preco, IFormFile? imagemProduto)
        {
            if (id != preco.codPreco)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagemProduto != null)
                    {
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        string diretorio = Directory.GetCurrentDirectory();

                        diretorio = diretorio + "/Imagens/Produtos";

                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(imagemProduto.FileName);
                        string name = diretorio + "/" + id + "-" + imagemProduto.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            imagemProduto.CopyTo(stream);
                        }
                        preco.Produto.imagem = baseUrl + "/Imagens/Produtos/" + id + "-" + fileName;
                    }
                    _context.Update(preco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrecoExists(preco.codPreco))
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
            ViewData["codCategoria"] = new SelectList(_context.Categorias.Where(s => s.ativo == true), "codCategoria", "nome", preco.Produto.codCategoria); 
            return View(preco);
        }

        // GET: Precos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preco = await _context.Precos
                .Include(p => p.Filial)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.codPreco == id);
            if (preco == null)
            {
                return NotFound();
            }

            return View(preco);
        }

        // POST: Precos/Delete/5
        [HttpPost, ActionName("Delete")]
        public bool DeleteConfirmed(int id)
        {
            var preco = _context.Precos.Find(id);
            if (preco != null)
            {
                _context.Precos.Remove(preco);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        private bool PrecoExists(int id)
        {
            return _context.Precos.Any(e => e.codPreco == id);
        }
    }
}

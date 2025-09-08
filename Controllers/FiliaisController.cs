using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using X.PagedList.Extensions;
using System.Reflection;

namespace BixWeb.Controllers
{
    public class FiliaisController : Controller
    {
        private readonly DbPrint _context;

        public FiliaisController(DbPrint context)
        {
            _context = context;
        }

        // GET: Filiais
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
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

                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var filiais = new List<Filial>();
                if (userId != null) 
                {
                    int CodUsuario = int.Parse(userId);
                    var usuarioFiliais = _context.UsuarioFiliais.Where(s => s.codUsuario == CodUsuario).Include(s=>s.Filial).Include(s=>s.Filial.precos).ToList();
                    if (usuarioFiliais.Count()>0) 
                    {
                        foreach (var usuarioFilial in usuarioFiliais)
                        {
                            if (usuarioFilial.Filial != null) 
                            {
                                filiais.Add(usuarioFilial.Filial);
                            }
                        }
                    }
                    else
                    {
                        ViewData["filial"] = "Nenhuma filial encontrada!";
                        return View(filiais.AsQueryable().ToPagedList());
                    }
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    string searchLower = searchString.ToLower();
                    filiais = filiais.Where(s => s.nome.ToLower().Contains(searchLower)
                                              || s.cnpj.ToLower().Contains(searchLower)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        filiais = filiais.OrderByDescending(s => s.nome).ToList();
                        break;
                    default:  // Name ascending 
                        filiais = filiais.OrderBy(s => s.nome).ToList();
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(filiais.AsQueryable().ToPagedList(pageNumber,pageSize));
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar IndexFiliais: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        // GET: Filiais/Details/5
        //public async task<iactionresult> details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return notfound();
        //    }

        //    var filial = await _context.filiais
        //        .include(f => f.endereco)
        //        .firstordefaultasync(m => m.codfilial == id);
        //    if (filial == null)
        //    {
        //        return notfound();
        //    }

        //    return view(filial);
        //}

        // GET: Filiais/Create

        [Authorize(Roles = "Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filiais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Create([Bind("codFilial,nome,cnpj,telefone,email,logoHome,logoPDV,codEndereco,Endereco")] Filial filial, IFormFile? Homelogo, IFormFile? logoCardapio)
        {
            if (ModelState.IsValid)
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId==null)
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                int codUsuario = int.Parse(userId);
                
                _context.Add(filial);
                await _context.SaveChangesAsync();

                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                string diretorio = Directory.GetCurrentDirectory();

                diretorio = diretorio + "/Empresas/" + filial.codFilial + "/Logo";
                if (logoCardapio != null)
                {
                    if (!Directory.Exists(diretorio))
                    {
                        Directory.CreateDirectory(diretorio);
                    }
                    var fileName = Path.GetFileName(logoCardapio.FileName);
                    string name = diretorio + "/" + logoCardapio.FileName;
                    using (var stream = new FileStream(name, FileMode.Create))
                    {
                        logoCardapio.CopyTo(stream);
                    }
                    filial.logoPDV = baseUrl + "/Empresas/" + filial.codFilial + "/Logo/" + fileName;
                }
                if (Homelogo != null)
                {
                    if (!Directory.Exists(diretorio))
                    {
                        Directory.CreateDirectory(diretorio);
                    }
                    var fileName = Path.GetFileName(Homelogo.FileName);
                    string name = diretorio + "/" + Homelogo.FileName;
                    using (var stream = new FileStream(name, FileMode.Create))
                    {
                        Homelogo.CopyTo(stream);
                    }
                    filial.logoHome = baseUrl + "/Empresas/" + filial.codFilial + "/Logo/" + fileName;
                }
                UsuarioFilial usuarioFilial = new UsuarioFilial();
                usuarioFilial.codUsuario = codUsuario;

                filial.ativo = true;
                usuarioFilial.codFilial=filial.codFilial;
                usuarioFilial.dataCadastro= DateTime.Now;
                usuarioFilial.pin = 0; // Defina o PIN como 0 ou gere um PIN aleatório
                usuarioFilial.cargoUsuario = "Gerente";
                usuarioFilial.tipoUsuario = "Gerente";

                _context.Add(usuarioFilial);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(filial);
        }

        // GET: Filiais/Edit/5
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filial = await _context.Filiais.FindAsync(id);
            if (filial == null)
            {
                return NotFound();
            }
            filial.Endereco = _context.Enderecos.Where(s => s.codEndereco == filial.codEndereco).FirstOrDefault();
            return View(filial);
        }

        // POST: Filiais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Edit(int id, [Bind("codFilial,nome,cnpj,telefone,email,logoHome,logoPDV,codEndereco,Endereco")] Filial filial, IFormFile? Homelogo, IFormFile? logoCardapio)
        {
            if (id != filial.codFilial)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                    string diretorio = Directory.GetCurrentDirectory();

                    diretorio = diretorio + "/Empresas/" + filial.codFilial + "/Logo";
                    if (logoCardapio!=null)
                    {
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(logoCardapio.FileName);
                        string name = diretorio + "/" + logoCardapio.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            logoCardapio.CopyTo(stream);
                        }
                        filial.logoPDV = baseUrl + "/Empresas/" + filial.codFilial + "/Logo/" + fileName;
                    }
                    if (Homelogo!=null)
                    {
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(Homelogo.FileName);
                        string name = diretorio + "/" + Homelogo.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            Homelogo.CopyTo(stream);
                        }
                        filial.logoHome = baseUrl + "/Empresas/" + filial.codFilial + "/Logo/" + fileName;
                    }
                    filial.ativo = true;
                    
                    _context.Update(filial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilialExists(filial.codFilial))
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
            return View(filial);
        }

        // GET: Filiais/Delete/5
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filial = await _context.Filiais
                .Include(f => f.Endereco)
                .FirstOrDefaultAsync(m => m.codFilial == id);
            if (filial == null)
            {
                return NotFound();
            }

            return View(filial);
        }

        // POST: Filiais/Delete/5
        [Authorize(Roles = "Gerente")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filial = await _context.Filiais.FindAsync(id);
            if (filial != null)
            {
                _context.Filiais.Remove(filial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VeryCNPJ(string cnpj)
        {
            var filial = await _context.Filiais
                .FirstOrDefaultAsync(f => f.cnpj == cnpj);
            return Json(new { success = filial != null });
        }

        [Authorize(Roles = "Admin")]
        private bool FilialExists(int id)
        {
            return _context.Filiais.Any(e => e.codFilial == id);
        }
    }
}

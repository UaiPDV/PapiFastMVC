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
using System.Runtime.InteropServices;

namespace BixWeb.Controllers
{
    public class VoucherController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;
        public VoucherController(DbPrint context, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
        }

        // GET: Voucher
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
                var vouchers = from s in _context.Vouchers
                               select s;

                if (userId != null)
                {
                    int CodUsuario = int.Parse(userId);
                    vouchers = vouchers  // Supondo que a navegação está configurada
                        .Include(c => c.UsuarioFilial.Filial).Include(s => s.Cliente)   // Se tiver uma entidade Filial relacionada
                        .Where(c => _context.UsuarioFiliais
                                        .Any(uf => uf.codUsuario == CodUsuario &&
                                                   uf.codFilial == c.codFilial) ||
                                    c.codCriador == CodUsuario);

                    if (vouchers.Any())
                    {
                        if (!System.String.IsNullOrEmpty(searchString))
                        {
                            vouchers = vouchers.Where(s => s.Cliente.nome.Contains(searchString)
                                                      || s.Cliente.email.Contains(searchString)
                                                      || s.tokenVoucher.Contains(searchString)
                                                      || s.validadeVoucher.ToString().Contains(searchString));
                        }
                        switch (sortOrder)
                        {
                            case "name_desc":
                                vouchers = vouchers.OrderByDescending(s => s.criacaoVoucher);
                                break;
                            default:  // Name ascending 
                                vouchers = vouchers.OrderBy(s => s.criacaoVoucher);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(vouchers.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["Voucher"] = "Nenhum Voucher Cadastrado!";
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
                    var vouchers = _context.Vouchers.Where(s => s.codCliente == CodUsuario).Include(s => s.Cliente).AsEnumerable();

                    if (vouchers.Any())
                    {
                        if (!System.String.IsNullOrEmpty(searchString))
                        {
                            vouchers = vouchers.Where(s => s.Cliente.nome.Contains(searchString)
                                                      || s.Cliente.email.Contains(searchString)
                                                      || s.tokenVoucher.Contains(searchString)
                                                      || s.validadeVoucher.ToString().Contains(searchString));
                        }
                        switch (sortOrder)
                        {
                            case "name_desc":
                                vouchers = vouchers.OrderByDescending(s => s.criacaoVoucher);
                                break;
                            default:  // Name ascending 
                                vouchers = vouchers.OrderBy(s => s.criacaoVoucher);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(vouchers.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["Voucher"] = "Nenhum Voucher Cadastrado!";
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

        // GET: Voucher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .Include(v => v.Campanha)
                .Include(v => v.Cliente)
                .Include(v => v.UsuarioFilial)
                .FirstOrDefaultAsync(m => m.codVoucher == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // GET: Voucher/Create
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

        // POST: Voucher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codVoucher,tokenVoucher,valorOriginal,valorDisponivel,validadeVoucher,criacaoVoucher,Utilizado,codCampanha,codCriador,codFilial,codCliente,Cliente")] Voucher voucher)
        {
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != null)
            {
                try
                {
                    int CodUsuario = int.Parse(userId);
                    voucher.valorDisponivel = voucher.valorOriginal;
                    voucher.criacaoVoucher = DateTime.Now;
                    voucher.codCriador = CodUsuario;

                    var cliente = from s in _context.Usuario
                                  select s;

                    cliente = cliente.Where(s => s.cpf.Contains(voucher.Cliente.cpf)
                                  || s.email.Contains(voucher.Cliente.email));

                    if (!cliente.Any())
                    {
                        if (string.IsNullOrWhiteSpace(voucher.Cliente.cpf) && voucher.Cliente.email == null)
                        {
                            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                            return View("Error");
                        }
                        Usuario usuario = new()
                        {
                            nome = voucher.Cliente.nome,
                            email = voucher.Cliente.email,
                            telefone = voucher.Cliente.telefone,
                            cpf = voucher.Cliente.cpf,
                            password = voucher.Cliente.cpf.Substring(0, 4)
                        };
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();
                        voucher.codCliente = usuario.codUsuario;

                    }
                    else
                    {
                        foreach (var item in cliente)
                        {
                            voucher.codCliente = item.codUsuario;
                            voucher.Cliente = null;
                        }
                    }
                    var ultimoRegistro = _context.Vouchers.OrderByDescending(e => e.codVoucher).FirstOrDefault();
                    if (ultimoRegistro == null)
                    {
                        voucher.tokenVoucher = voucher.codCriador + "SM-1V" + (int)voucher.valorOriginal;
                    }
                    else
                    {
                        voucher.tokenVoucher = voucher.codCriador + "SM-" + ultimoRegistro.codVoucher + 1 + "V" + (int)voucher.valorOriginal;
                    }
                    if (ModelState.IsValid)
                    {
                        _context.Add(voucher);
                        await _context.SaveChangesAsync();

                        var link = $"{this.Request.Scheme}://{this.Request.Host}" + "/Voucher/Validar/" + voucher.codVoucher;
                        string diretorio = Directory.GetCurrentDirectory();
                        diretorio += "/Empresas/" + voucher.codFilial + "/Voucher/";
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        _pageGeneratorService.GerarVoucher(voucher.codVoucher, diretorio, link);
                        _pageGeneratorService.EnviarVoucher(diretorio, voucher.Cliente.email, voucher.Cliente.nome);
                        return RedirectToAction(nameof(Index));
                    }
                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();

                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome", voucher.codFilial);
                    return View(voucher);
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao chamar CreateVouchers: {ex}");
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

        // GET: Voucher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", voucher.codCampanha);
            ViewData["codCliente"] = new SelectList(_context.Usuario, "codUsuario", "email", voucher.codCliente);
            ViewData["codCriador"] = new SelectList(_context.UsuarioFiliais, "codUsuario", "codUsuario", voucher.codCriador);
            return View(voucher);
        }
        // POST: Voucher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codVoucher,tokenVoucher,valorOriginal,valorDisponivel,validadeVoucher,criacaoVoucher,Utilizado,codCampanha,codCriador,codFilial,codCliente")] Voucher voucher)
        {
            if (id != voucher.codVoucher)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.codVoucher))
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
            ViewData["codCampanha"] = new SelectList(_context.Campanhas, "codCampanha", "nomeCampanha", voucher.codCampanha);
            ViewData["codCliente"] = new SelectList(_context.Usuario, "codUsuario", "email", voucher.codCliente);
            ViewData["codCriador"] = new SelectList(_context.UsuarioFiliais, "codUsuario", "codUsuario", voucher.codCriador);
            return View(voucher);
        }
        public IActionResult Enviar(int id)
        {
            var voucher = _context.Vouchers.Find(id);
            if (voucher == null)
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            voucher.Cliente = _context.Usuario.Find(voucher.codCliente);
            if (voucher.Cliente == null)
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            if (voucher.Cliente.email == null || voucher.Cliente.email == "")
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }

            var link = $"{this.Request.Scheme}://{this.Request.Host}" + "/Voucher/Validar/" + voucher.codVoucher;
            string diretorio = Directory.GetCurrentDirectory();
            diretorio += "/Empresas/" + voucher.codFilial + "/Voucher/";
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            if (_pageGeneratorService.GerarVoucher(id, diretorio, link))
            {
                if (_pageGeneratorService.EnviarVoucher(diretorio + voucher.tokenVoucher + ".png", voucher.Cliente.email, voucher.Cliente.nome))
                {
                    return RedirectToAction("Index", new { formSubmitted = true });
                }
            }
            return RedirectToAction("Index", new { formSubmitted = false });
        }
        public async Task<IActionResult> EnviarWhats(int id)
        {
            var voucher = _context.Vouchers.Find(id);
            if (voucher == null)
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            voucher.Cliente = _context.Usuario.Find(voucher.codCliente);
            if (voucher.Cliente == null)
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            if (voucher.Cliente.telefone == null)
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }

            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            string token;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                int codUsuario = int.Parse(userId);
                token = _context.Usuario.Where(s => s.codUsuario == codUsuario).Select(u => u.token).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToAction("Index", new { formSubmitted = false });
                }
            }
            else
            {
                return RedirectToAction("Index", new { formSubmitted = false });
            }
            var link = $"{this.Request.Scheme}://{this.Request.Host}" + "/Voucher/Validar/" + voucher.codVoucher;
            var urlimagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/Empresas/" + voucher.codFilial + "/Voucher/" + voucher.tokenVoucher + ".png";
            var filial = _context.Filiais.Find(voucher.codFilial);
            string diretorio = Directory.GetCurrentDirectory();
            diretorio += "/Empresas/" + voucher.codFilial + "/Voucher/";
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            if (_pageGeneratorService.GerarVoucher(id, diretorio, link))
            {
                if (string.IsNullOrWhiteSpace(voucher.Cliente.telefone))
                {
                    return RedirectToAction("Index", new { formSubmitted = false });
                }
                string number = voucher.Cliente.telefone;
                voucher.Cliente.telefone = "55" + number.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (await _pageGeneratorService.EnviarVoucherWhats(urlimagem, voucher.Cliente.telefone, voucher.Cliente.nome, filial.nome, token))
                {
                    return RedirectToAction("Index", new { formSubmitted = true });
                }
            }
            return RedirectToAction("Index", new { formSubmitted = false });
        }
        // GET: Voucher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .Include(v => v.Campanha)
                .Include(v => v.Cliente)
                .Include(v => v.UsuarioFilial)
                .FirstOrDefaultAsync(m => m.codVoucher == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }
        // POST: Voucher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VoucherExists(int id)
        {
            return _context.Vouchers.Any(e => e.codVoucher == id);
        }
    }
}

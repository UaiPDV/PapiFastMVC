using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using X.PagedList.Extensions;
using NuGet.Common;
using ZXing.Aztec.Internal;
using Microsoft.AspNetCore.Identity;
using BixWeb.Services;

namespace BixWeb.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbPrint _context;
        private readonly IPageGeneratorService _pageGeneratorService;

        public UsuariosController(DbPrint context, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _pageGeneratorService = pageGeneratorService;
        }

        // GET: Usuarios

        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult Clientes(string sortOrder, string currentFilter, string searchString, int? page)
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
                var clientes = from s in _context.Usuario
                               select s;

                if (userId != null)
                {
                    int CodUsuario = int.Parse(userId);
                    clientes = _context.Usuario
                    .Include(u => u.filiais)            // Carrega as informações de UsuarioFilial
                    .ThenInclude(uf => uf.Filial)              // Carrega as informações de Filial relacionadas ao UsuarioFilial
                    .Where(u => u.filiais
                        .Any(uf => uf.Filial.Usuarios
                            .Any(uf2 => uf2.codUsuario == CodUsuario) && uf.tipoUsuario == "Cliente"));                            // Remove duplicatas (caso um usuário esteja em mais de uma filial)
                                                                                                                                   // Remove duplicatas (caso um usuário esteja em mais de uma filial)

                    if (clientes.Any())
                    {
                        if (!System.String.IsNullOrEmpty(searchString))
                        {
                            clientes = clientes.Where(s =>
                                s.nome.Contains(searchString) ||
                                s.email.Contains(searchString) ||
                                s.telefone.Contains(searchString) ||
                                s.cpf.Contains(searchString) ||
                                s.filiais.Any(f => f.Filial.nome.Contains(searchString) || f.Filial.codFilial.ToString().Contains(searchString))
                            );
                        }
                        switch (sortOrder)
                        {
                            case "name_desc":
                                clientes = clientes.OrderByDescending(s => s.nome);
                                break;
                            default:  // Name ascending 
                                clientes = clientes.OrderBy(s => s.nome);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(clientes.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["Cliente"] = "Nenhum cliente Cadastrado!";
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
                ErrorViewModel.LogError($"Erro ao chamar ClientesUsuarios: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        public IActionResult Funcionarios(string sortOrder, string currentFilter, string searchString, int? page)
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
                var funcionarios = from s in _context.Usuario
                                   select s;

                if (userId != null)
                {
                    int CodUsuario = int.Parse(userId);
                    funcionarios = _context.Usuario
                    .Include(u => u.filiais)            // Carrega as informações de UsuarioFilial
                    .ThenInclude(uf => uf.Filial)              // Carrega as informações de Filial relacionadas ao UsuarioFilial
                    .Where(u => u.filiais
                        .Any(uf => uf.Filial.Usuarios
                            .Any(uf2 => uf2.codUsuario == CodUsuario) && uf.tipoUsuario == "Gerente" || uf.tipoUsuario == "Funcionario"));                            // Remove duplicatas (caso um usuário esteja em mais de uma filial)

                    if (funcionarios.Any())
                    {
                        if (!System.String.IsNullOrEmpty(searchString))
                        {
                            funcionarios = funcionarios.Where(s =>
                                s.nome.Contains(searchString) ||
                                s.email.Contains(searchString) ||
                                s.telefone.Contains(searchString) ||
                                s.cpf.Contains(searchString) ||
                                s.filiais.Any(f => f.Filial.nome.Contains(searchString) || f.Filial.codFilial.ToString().Contains(searchString))
                            );
                        }
                        switch (sortOrder)
                        {
                            case "name_desc":
                                funcionarios = funcionarios.OrderByDescending(s => s.nome);
                                break;
                            default:  // Name ascending 
                                funcionarios = funcionarios.OrderBy(s => s.nome);
                                break;
                        }

                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        return View(funcionarios.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        ViewData["funcionario"] = "Nenhum funcionário Cadastrado!";
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
                ErrorViewModel.LogError($"Erro ao chamar FuncionariosUsuarios: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult NewCliente()
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
                ErrorViewModel.LogError($"Erro ao chamar NewClintes: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [Authorize(Roles = "Gerente, Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewCliente(Usuario usuario, IFormFile? avatar, int codFilial)
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != null)
                {
                    if (!String.IsNullOrEmpty(usuario.cpf) && !String.IsNullOrEmpty(usuario.email))
                    {
                        var cpf = _context.UsuarioFiliais.Where(s => s.Usuario.cpf == usuario.cpf && s.Usuario.email == usuario.email && s.codFilial==codFilial).Any();
                        if (cpf)
                        {
                            ViewBag.email = "Usuario já cadastrado neste comércio!";
                        }
                        else
                        {
                            var usuariocad = _context.Usuario.Where(s => s.cpf == usuario.cpf || s.email==usuario.email).FirstOrDefault();
                            if (usuariocad != null)
                            {
                                if (usuariocad.email == usuario.email  && usuariocad.cpf==usuario.cpf)
                                {
                                    UsuarioFilial usuarioFilial = new UsuarioFilial();
                                    usuarioFilial.dataCadastro = DateTime.Now;
                                    usuarioFilial.ativo = true;
                                    usuarioFilial.tipoUsuario = "Cliente";
                                    usuarioFilial.codFilial = codFilial;
                                    usuarioFilial.Usuario = usuariocad;
                                    usuarioFilial.codUsuario = usuariocad.codUsuario;

                                    if (avatar != null)
                                    {
                                        string diretorio = Directory.GetCurrentDirectory();
                                        diretorio = diretorio + "/wwwroot/Usuarios/" + usuario.codUsuario;
                                        if (!Directory.Exists(diretorio))
                                        {
                                            Directory.CreateDirectory(diretorio);
                                        }
                                        var fileName = Path.GetFileName(avatar.FileName);
                                        string name = diretorio + "/" + avatar.FileName;
                                        using (var stream = new FileStream(name, FileMode.Create))
                                        {
                                            avatar.CopyTo(stream);
                                        }
                                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                        usuariocad.imagem = baseUrl + "/Usuarios/" + usuario.codUsuario + "/" + fileName;
                                    }
                                    else
                                    {
                                        usuariocad.imagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/imagens/Usuario.png";
                                    }

                                    _context.Add(usuarioFilial);
                                    _context.SaveChanges();
                                    return RedirectToAction(nameof(Clientes));
                                }
                                else
                                {
                                    if (usuariocad.email == usuario.email)
                                    {
                                        ViewBag.email = "E-mail já cadastrado!";
                                    }
                                    if (usuariocad.cpf == usuario.cpf)
                                    {
                                        ViewBag.cpf = "CPF já cadastrado!";
                                    }
                                }
                                
                            }
                            else
                            {
                                if (ModelState.IsValid)
                                {
                                    UsuarioFilial usuarioFilial = new UsuarioFilial();
                                    usuarioFilial.dataCadastro = DateTime.Now;
                                    usuarioFilial.ativo = true;
                                    usuarioFilial.tipoUsuario = "Cliente";
                                    usuarioFilial.codFilial = codFilial;

                                    string token = Guid.NewGuid().ToString();
                                    usuarioFilial.token = $"{this.Request.Scheme}://{this.Request.Host}" + "/Login/Ativar/" + token;
                                    usuarioFilial.dataCadastro = DateTime.Now;

                                    usuario.imagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/imagens/Usuario.png";

                                    usuarioFilial.Usuario = usuario;
                                    
                                    if (_pageGeneratorService.AtivarContaCliente(usuarioFilial.Usuario, usuarioFilial))
                                    {
                                        usuarioFilial.token = token;
                                        _context.Add(usuarioFilial);
                                        _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        ViewData["Error"] = "Ops! Houve um erro ao gerar o token de ativação. Tente novamente mais tarde.";
                                        return View("Error");
                                    }

                                    if (avatar != null)
                                    {
                                        string diretorio = Directory.GetCurrentDirectory();
                                        diretorio = diretorio + "/wwwroot/Usuarios/" + usuario.codUsuario;
                                        if (!Directory.Exists(diretorio))
                                        {
                                            Directory.CreateDirectory(diretorio);
                                        }
                                        var fileName = Path.GetFileName(avatar.FileName);
                                        string name = diretorio + "/" + avatar.FileName;
                                        using (var stream = new FileStream(name, FileMode.Create))
                                        {
                                            avatar.CopyTo(stream);
                                        }

                                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                        usuario.imagem = baseUrl + "/Usuarios/" + usuario.codUsuario + "/" + fileName;

                                        _context.Update(usuario);
                                        _context.SaveChanges();
                                    }

                                    return RedirectToAction(nameof(Clientes));
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(usuario.email))
                    {
                        ViewBag.email = "Você deve preencher o campo E-mail!";
                    }
                    if (String.IsNullOrEmpty(usuario.cpf))
                    {
                        ViewBag.cpf = "Você deve preencher o campo CPF!";
                    }


                    int CodUsuario = int.Parse(userId);
                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();

                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome", usuario.codUsuario);

                    return View(usuario);
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar NewClintes: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }

        [Authorize(Roles = "Gerente")]
        public IActionResult NewFuncionario()
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
                ErrorViewModel.LogError($"Erro ao chamar NewClintes: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        [Authorize(Roles = "Gerente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewFuncionario(Usuario usuario, IFormFile? avatar, int codFilial, string tipoUsuario, string token)
        {
            try
            {
                var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (userId != null)
                {
                    int CodUsuario = int.Parse(userId);
                    if (!String.IsNullOrEmpty(usuario.cpf) && !String.IsNullOrEmpty(usuario.email))
                    {
                        var cpf = _context.UsuarioFiliais.Where(s => s.Usuario.cpf == usuario.cpf && s.Usuario.email == usuario.email && s.codFilial == codFilial).Any();
                        if (cpf)
                        {
                            ViewBag.email = "Usuário já cadastrado neste comércio!";
                        }
                        else
                        {
                            var usuariocad = _context.Usuario.Where(s => s.cpf == usuario.cpf && s.email == usuario.email).FirstOrDefault();
                            if (usuariocad != null)
                            {
                                if (usuariocad.email == usuario.email && usuariocad.cpf == usuario.cpf)
                                {
                                    UsuarioFilial usuarioFilial = new UsuarioFilial();
                                    usuarioFilial.dataCadastro = DateTime.Now;
                                    usuarioFilial.ativo = true;
                                    usuarioFilial.tipoUsuario = tipoUsuario;
                                    usuarioFilial.token = token;
                                    usuarioFilial.codFilial = codFilial;
                                    usuarioFilial.codUsuario = usuariocad.codUsuario;
                                    usuarioFilial.Usuario = usuariocad;

                                    if (avatar != null)
                                    {
                                        string diretorio = Directory.GetCurrentDirectory();
                                        diretorio = diretorio + "/wwwroot/Usuarios/" + usuario.codUsuario;
                                        if (!Directory.Exists(diretorio))
                                        {
                                            Directory.CreateDirectory(diretorio);
                                        }
                                        var fileName = Path.GetFileName(avatar.FileName);
                                        string name = diretorio + "/" + avatar.FileName;
                                        using (var stream = new FileStream(name, FileMode.Create))
                                        {
                                            avatar.CopyTo(stream);
                                        }
                                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                        usuariocad.imagem = baseUrl + "/Usuarios/" + usuario.codUsuario + "/" + fileName;
                                    }
                                    else
                                    {
                                        usuariocad.imagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/imagens/Usuario.png";
                                    }

                                    _context.Add(usuarioFilial);
                                    _context.SaveChanges();
                                    return RedirectToAction(nameof(Clientes));
                                }
                                else
                                {
                                    if (usuariocad.email == usuario.email)
                                    {
                                        ViewBag.email = "E-mail já cadastrado!";
                                    }
                                    if (usuariocad.cpf == usuario.cpf)
                                    {
                                        ViewBag.cpf = "CPF já cadastrado!";
                                    }
                                }
                            }
                            else
                            {
                                if (ModelState.IsValid)
                                {
                                    UsuarioFilial usuarioFilial = new UsuarioFilial();
                                    usuarioFilial.dataCadastro = DateTime.Now;
                                    usuarioFilial.ativo = true;
                                    usuarioFilial.tipoUsuario = tipoUsuario;
                                    usuarioFilial.token = token;
                                    usuarioFilial.codFilial = codFilial;
                                    usuarioFilial.Usuario = usuario;
                                    if (String.IsNullOrEmpty(token))
                                    {
                                        token = Guid.NewGuid().ToString();
                                    }
                                    usuarioFilial.token = $"{this.Request.Scheme}://{this.Request.Host}" + "/Login/Ativar/" + token;
                                    usuarioFilial.dataCadastro = DateTime.Now;

                                    usuario.imagem = $"{this.Request.Scheme}://{this.Request.Host}" + "/imagens/Usuario.png";

                                    usuarioFilial.Usuario = usuario;

                                    if (_pageGeneratorService.AtivarContaCliente(usuarioFilial.Usuario, usuarioFilial))
                                    {
                                        usuarioFilial.token = token;
                                        _context.Add(usuarioFilial);
                                        _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        ViewData["Error"] = "Ops! Houve um erro ao gerar o token de ativação. Tente novamente mais tarde.";
                                        return View("Error");
                                    }

                                    if (avatar != null)
                                    {
                                        string diretorio = Directory.GetCurrentDirectory();
                                        diretorio = diretorio + "/wwwroot/Usuarios/" + usuario.codUsuario;
                                        if (!Directory.Exists(diretorio))
                                        {
                                            Directory.CreateDirectory(diretorio);
                                        }
                                        var fileName = Path.GetFileName(avatar.FileName);
                                        string name = diretorio + "/" + avatar.FileName;
                                        using (var stream = new FileStream(name, FileMode.Create))
                                        {
                                            avatar.CopyTo(stream);
                                        }

                                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                        usuario.imagem = baseUrl + "/Usuarios/" + usuario.codUsuario + "/" + fileName;
                                        _context.Update(CodUsuario);
                                        _context.SaveChanges();
                                    }
                                    return RedirectToAction(nameof(Funcionarios));
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(usuario.email))
                    {
                        ViewBag.email = "Você deve preencher o campo E-mail!";
                    }
                    if (String.IsNullOrEmpty(usuario.cpf))
                    {
                        ViewBag.cpf = "Você deve preencher o campo CPF!";
                    }
                    var filiais = _context.UsuarioFiliais
                        .Where(uf => uf.codUsuario == CodUsuario)
                        .Select(uf => uf.Filial)
                        .ToList();

                    ViewData["codFilial"] = new SelectList(filiais, "codFilial", "nome", codFilial);

                    ViewBag.token = token;
                    return View(usuario);
                }
                else
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar NewClintes: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
        }
        // GET: Usuarios/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.codUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codUsuario,email,nome,telefone,cpf,password,imagem")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codUsuario,email,nome,telefone,cpf,password,imagem")] Usuario usuario, IFormFile? ImgUsu)
        {
            if (id != usuario.codUsuario)
            {
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImgUsu != null)
                    {
                        string diretorio = Directory.GetCurrentDirectory();
                        diretorio = diretorio + "/wwwroot/Usuarios/" + id;
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(ImgUsu.FileName);
                        string name = diretorio + "/" + ImgUsu.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            ImgUsu.CopyTo(stream);
                        }
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        if (usuario.imagem != null)
                        {
                            usuario.imagem = usuario.imagem.Replace(baseUrl, "");
                            string deleteimg = Directory.GetCurrentDirectory() + "/wwwroot/" + usuario.imagem;
                            if (System.IO.File.Exists(deleteimg))
                            {
                                System.IO.File.Delete(deleteimg);
                            }
                        }
                        usuario.imagem = baseUrl + "/Usuarios/" + id.ToString() + "/" + fileName;
                    }
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    if (usuario.imagem == null)
                    {
                        usuario.imagem = "/imagens/Usuario.png";
                    }
                    if (usuario.nome == null)
                    {
                        usuario.nome = "Sem nome";
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.nome),
                        new Claim(ClaimTypes.NameIdentifier, usuario.codUsuario.ToString()),
                        new Claim(ClaimTypes.Thumbprint, usuario.imagem),
                        new Claim(ClaimTypes.Email, usuario.email)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal);
                }
                catch (Exception ex)
                {
                    if (!UsuarioExists(usuario.codUsuario))
                    {
                        ErrorViewModel.LogError($"Erro ao chamar IndexFiliais: {ex}");
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return NotFound();
                    }
                }
                return RedirectToAction("edit", new { id = id });
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.codUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
        [Authorize(Roles = "Admin")]
        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> VerificarCpfEmail(string cpf, string email)
        {
            bool corresponde = false;
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.email == email);
            if (usuario!=null)
            {
                if (usuario.cpf==cpf)
                {
                    corresponde = true;
                }
            }
            else
            {
                usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.cpf == cpf);
                if (usuario != null)
                {
                    if (usuario.email == email)
                    {
                        corresponde = true;
                    }
                }
                else
                {
                    corresponde = true;
                }
            }
            return Json(new { success = corresponde });
        }
        [HttpGet]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            bool corresponde = await _context.Usuario.AnyAsync(u => u.email == email);
            return Json(new { success = corresponde });    
        }
        public bool Desativar(int id)
        {
            var Usuario = _context.Usuario.Where(s => s.codUsuario == id).Include(s => s.filiais).FirstOrDefault();
            var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
            {
                return false;
            }
            int CodFuncionario = int.Parse(userId);
            var filiais = _context.UsuarioFiliais.Where(s => s.codUsuario == CodFuncionario);
            try
            {
                if (Usuario != null)
                {
                    foreach (var item in filiais)
                    {
                        UsuarioFilial usuarioFilial = Usuario.filiais.Where(s => s.codFilial == item.codFilial).FirstOrDefault();
                        if (usuarioFilial != null)
                        {
                            usuarioFilial.ativo = false;
                            _context.Update(usuarioFilial);
                            _context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar DesativarFiliais: {ex}");
            }
            return false;
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.codUsuario == id);
        }
    }
}

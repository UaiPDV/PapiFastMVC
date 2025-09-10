using BixWeb.Models;
using BixWeb.Services;
using BixWeb.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Security.Claims;

namespace BixWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbPrint _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IPageGeneratorService _pageGeneratorService;
        public LoginController(DbPrint context, IMemoryCache memoryCache, IPageGeneratorService pageGeneratorService)
        {
            _context = context;
            _memoryCache = memoryCache;
            _pageGeneratorService = pageGeneratorService;
        }

        [AllowAnonymous]
        public IActionResult Index(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(Login Login, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    // Busca apenas pelo email
                    Usuario? usuario = _context.Usuario
                        .Where(s => s.email == Login.Email)
                        .Include(s => s.filiais)
                        .Include(s => s.loginAPI)
                        .FirstOrDefault();

                    if (usuario == null)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não encontrado!");
                        return View(Login);
                    }
                    if (usuario.ativo == false)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não foi ativado!");
                        ViewBag.msg = "ativar";
                        return View(Login);
                    }

                    // Verifica a senha com hash
                    var hasher = new PasswordHasher<Usuario>();
                    var resultado = hasher.VerifyHashedPassword(usuario, usuario.password, Login.Senha);

                    if (resultado != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Senha incorreta!");
                        return View(Login);
                    }

                    // Define tipo e imagem
                    string tipo = "";
                    string imagem = "/imagens/Usuario.png";

                    if (string.IsNullOrEmpty(usuario.nome))
                        usuario.nome = "Sem nome";

                    if (usuario.loginAPI != null)
                    {
                        tipo = "Gerente";
                    }
                    else if (usuario.filiais != null && usuario.filiais.Any())
                    {
                        foreach (var item in usuario.filiais)
                        {
                            if (!string.IsNullOrEmpty(item.tipoUsuario))
                                tipo = item.tipoUsuario;
                        }
                    }
                    else
                    {
                        tipo = "Cliente";
                    }

                    if (!string.IsNullOrEmpty(usuario.imagem))
                        imagem = usuario.imagem;

                    // Cria os Claims
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.nome),
                new Claim(ClaimTypes.NameIdentifier, usuario.codUsuario.ToString()),
                new Claim(ClaimTypes.Role, tipo),
                new Claim(ClaimTypes.Thumbprint, imagem),
                new Claim(ClaimTypes.Email, usuario.email)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7) // mesmo valor que no AddCookie
                    });


                    return LocalRedirect(returnUrl ?? "/");
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro no login: {ex}");
                    ModelState.AddModelError(string.Empty, "Desculpe! Ocorreu um erro.");
                    return View("Index", Login);
                }
            }

            ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
            return View("Index", Login);
        }
        [AllowAnonymous]
        public IActionResult Ativar(string? id)
        {

            if (id != null)
            {
                var usuario = _context.UsuarioFiliais.Where(s => s.token == id).Include(s => s.Usuario).FirstOrDefault();
                if (usuario != null)
                {
                    ViewBag.codFilial = usuario.codFilial;
                    ViewBag.codUsuario = usuario.codUsuario;
                    return View();
                }
                else
                {
                    ViewData["Error"] = "Ops! Link expirado!";
                    return View("Error");
                }
            }
            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
            return View("Error");
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Ativar(Ativar ativar)
        {
            try
            {
                if (ativar.senha == null || ativar.confirmarSenha == null)
                {
                    ViewBag.senha = ativar.senha;
                    ViewBag.confirmarsenha = ativar.confirmarSenha;
                    ViewBag.erro = "Você deve preencher todos o campos!";
                    return View();
                }
                if (ativar.codUsuario <= 0 || ativar.codFilial <= 0)
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                if (ativar.senha != ativar.confirmarSenha)
                {
                    ViewBag.senha = ativar.senha;
                    ViewBag.confirmarsenha = ativar.confirmarSenha;
                    ViewBag.erro = "O campo senha e confirmar senha devem ser iguais!";
                    return View();
                }
                var usuarioFilial = _context.UsuarioFiliais.Where(s => s.codUsuario == ativar.codUsuario && s.codFilial == ativar.codFilial).Include(s => s.Usuario).FirstOrDefault();
                if (usuarioFilial != null)
                {
                    if (ModelState.IsValid)
                    {

                        usuarioFilial.token = "";
                        var hasher = new PasswordHasher<Usuario>();
                        var senha = ativar.senha;
                        usuarioFilial.Usuario.password = hasher.HashPassword(usuarioFilial.Usuario, senha);

                        _context.Update(usuarioFilial);
                        _context.SaveChanges();
                        ViewBag.sucesso = "A senha foi alterada com sucesso!";
                        return View();
                    }
                }
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ativarConta: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario, IFormFile? avatar, string confSenha)
        {
            if (usuario.password != confSenha)
            {
                ViewBag.msg = "As senhas não conferem!";
                return View(usuario);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var hasher = new PasswordHasher<Usuario>();
                    var senha = usuario.password;
                    usuario.password = hasher.HashPassword(usuario, senha);
                    usuario.ativo = false;
                    var usuarioCad = _context.Usuario.Where(s => s.email == usuario.email).FirstOrDefault();
                    if (usuarioCad != null)
                    {
                        ViewBag.msg1 = "Email já cadastrado!";
                        return View(usuario);
                    }
                    usuarioCad = _context.Usuario.Where(s => s.cpf == usuario.cpf).FirstOrDefault();
                    if (usuarioCad != null)
                    {
                        ViewBag.msg2 = "CPF já cadastrado!";
                        return View(usuario);
                    }
                    usuario.ativo = false;
                    _context.Usuario.Add(usuario);
                    await _context.SaveChangesAsync();
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    if (avatar != null)
                    {
                        string diretorio = Directory.GetCurrentDirectory();

                        diretorio = diretorio + "/wwwroot/Usuarios/" + usuario.codUsuario + "/";
                        if (!Directory.Exists(diretorio))
                        {
                            Directory.CreateDirectory(diretorio);
                        }
                        var fileName = Path.GetFileName(avatar.FileName);
                        string name = diretorio + avatar.FileName;
                        using (var stream = new FileStream(name, FileMode.Create))
                        {
                            avatar.CopyTo(stream);
                        }

                        usuario.imagem = baseUrl + "/Usuarios/" + usuario.codUsuario + "/" + fileName; ;
                    }
                    else
                    {
                        usuario.imagem = baseUrl + "/imagens/Usuario.png";
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    Random random = new Random();
                    string token = "";
                    for (int i = 0; i < 6; i++)
                    {
                        token += random.Next(0, 10).ToString(); // Gera um número aleatório entre 0 e 9
                    }
                    if (_pageGeneratorService.EnviarToken(usuario.email, usuario.nome, token))
                    {
                        _memoryCache.Set(usuario.email, token, TimeSpan.FromHours(1));
                        return RedirectToAction("Validation", "Login", new { email = usuario.email });
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ErrorViewModel.LogError($"Erro ao criar usuário: {ex}");
                    ModelState.AddModelError(string.Empty, "Desculpe! Ocorreu um erro ao criar o usuário.");
                    return View("Error");
                }
            }
            return View(usuario);
        }

        public IActionResult Validation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Error"] = "Ops! Ocorreu um erro ao validar o e-mail.";
                return View("Error");
            }
            ViewBag.email = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Validation(string email, string token)
        {
            try
            {
                if (email == null)
                {
                    ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                    return View("Error");
                }
                if (_memoryCache.TryGetValue(email, out string tokenCache))
                {
                    if (token == tokenCache)
                    {
                        Usuario usuario = await _context.Usuario.Where(s => s.email == email && s.ativo == false).FirstOrDefaultAsync();
                        if (usuario == null)
                        {
                            ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                            return View("Error");
                        }
                        usuario.ativo = true;
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                        _memoryCache.Remove(token); // Remove o token após a validação
                        return RedirectToAction(nameof(Index));
                    }
                }
                ViewBag.ErrorMessage = "Token inválido ou expirado. Por favor, solicite um novo token.";
                ViewBag.Email = email;
                return View();
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar ValidarToken: {ex}");
                ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                return View("Error");
            }

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}

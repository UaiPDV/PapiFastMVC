using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using X.PagedList.Extensions;

namespace BixWeb.Controllers
{
    public class PresentesController : Controller
    {
        private readonly DbPrint _context;

        public PresentesController(DbPrint context)
        {
            _context = context;
        }

        // GET: Presentes
        public IActionResult Index(int id,string sortOrder, string currentFilter, string searchString, int? page)
        {
            var presentes =from s in _context.Presentes where s.listaPresente.codConvite == id
                           select s ;
            ViewBag.codConvite = id;

            if (presentes==null)
            {
                ViewData["presente"] = "Ops! Nenhum presente encontrado!";
                
                return View();
            }
            if (!System.String.IsNullOrEmpty(searchString))
            {
                presentes= presentes.Where(s =>
                    s.Nome.Contains(searchString) ||
                    s.Descricao.Contains(searchString)
                );
            }
            switch (sortOrder)
            {
                case "name_desc":
                    presentes = presentes.OrderByDescending(s => s.Preco);
                    break;
                default:  // Name ascending 
                    presentes = presentes.OrderBy(s => s.Preco);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(presentes.ToPagedList(pageNumber, pageSize));
        }

        // GET: Presentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presente = await _context.Presentes
                .Include(p => p.listaPresente)
                .FirstOrDefaultAsync(m => m.codPresente == id);
            if (presente == null)
            {
                return NotFound();
            }

            return View(presente);
        }

        // GET: Presentes/Create
        public IActionResult Create(int id)
        {
            ViewBag.codConvite = id;
            ViewData["codListaPresente"] = new SelectList(_context.ListaPresentes, "codListaPres", "codListaPres");
            return View();
        }

        // POST: Presentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codPresente,Nome,Preco,Descricao,Imagem,codListaPresente,listaPresente")] Presente presente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(presente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["codListaPresente"] = new SelectList(_context.ListaPresentes, "codListaPres", "codListaPres", presente.codListaPresente);
            return View(presente);
        }

        // GET: Presentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presente = await _context.Presentes.FindAsync(id);
            if (presente == null)
            {
                return NotFound();
            }
            ViewData["codListaPresente"] = new SelectList(_context.ListaPresentes, "codListaPres", "codListaPres", presente.codListaPresente);
            return View(presente);
        }

        // POST: Presentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codPresente,Nome,Preco,Descricao,Imagem,codListaPresente")] Presente presente, IFormFile? ImagemPresente)
        {
            if (id != presente.codPresente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var convite = await _context.Convites.Where(s=>s.ListaPresente.codListaPres==presente.codListaPresente).Include(s=>s.Evento).FirstOrDefaultAsync();
                    if (convite == null)
                    {
                        ViewData["Error"] = "Ops! Houve um erro ao carregar a página solicitada";
                        return View("Error");
                    }
                    if (ImagemPresente != null && ImagemPresente.Length > 0)
                    {
                        string diretorio = Directory.GetCurrentDirectory();
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                        // Gerar um nome único para o arquivo
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPresente.FileName)}";
                        if (convite.codFilial!=null && convite.codFilial != 0)
                        {
                            diretorio = diretorio + "/Empresas/" + convite.Evento.codFilial + "/Eventos/" + convite.codEvento + "/ImagensPresentes/";
                            baseUrl += "/Empresas/" + convite.Evento.codFilial + "/Eventos/" + convite.codEvento + "/ImagensPresentes/";
                        }
                        else
                        {
                            diretorio = diretorio + "/wwwroot/Usuarios/" + convite.codCriador + "/Eventos/" + convite.codEvento + "/ImagensPresentes/";
                            baseUrl += "/Usuarios/" + convite.codCriador + "/Eventos/" + convite.codEvento + "/ImagensPresentes/";
                        }
                        
                        using (var stream = new FileStream(diretorio + presente.codPresente + ".jpg", FileMode.Create))
                        {
                            await ImagemPresente.CopyToAsync(stream);
                        }
                        // Atualizar o campo Imagem do presente com o nome do arquivo salvo
                        presente.Imagem = fileName + presente.codPresente + ".jpg";
                    }
                    _context.Update(presente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresenteExists(presente.codPresente))
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
            ViewData["codListaPresente"] = new SelectList(_context.ListaPresentes, "codListaPres", "codListaPres", presente.codListaPresente);
            return View(presente);
        }

        // GET: Presentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presente = await _context.Presentes
                .Include(p => p.listaPresente)
                .FirstOrDefaultAsync(m => m.codPresente == id);
            if (presente == null)
            {
                return NotFound();
            }

            return View(presente);
        }

        // POST: Presentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var presente = await _context.Presentes.FindAsync(id);
            if (presente != null)
            {
                _context.Presentes.Remove(presente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PresenteExists(int id)
        {
            return _context.Presentes.Any(e => e.codPresente == id);
        }
    }
}

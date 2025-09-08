using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;

namespace BixWeb.Controllers
{
    public class ConvidadosController : Controller
    {
        private readonly DbPrint _context;

        public ConvidadosController(DbPrint context)
        {
            _context = context;
        }

        // GET: Convidados
        public async Task<IActionResult> Index()
        {
            var dbPrint = _context.Convidados.Include(c => c.Convite).Include(c => c.Ingresso);
            return View(await dbPrint.ToListAsync());
        }

        // GET: Convidados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var convidado = await _context.Convidados
                .Include(c => c.Convite)
                .Include(c => c.Ingresso)
                .FirstOrDefaultAsync(m => m.codConvidado == id);
            if (convidado == null)
            {
                return NotFound();
            }

            return View(convidado);
        }

        // GET: Convidados/Create
        public IActionResult Create()
        {
            ViewData["codConvite"] = new SelectList(_context.Convites, "codConvite", "codConvite");
            ViewData["codIngresso"] = new SelectList(_context.Ingressos, "codIngresso", "ticketIngresso");
            return View();
        }

        // POST: Convidados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("codConvidado,emailConvidado,telefoneConvidado,vistoConvite,confirmacaoConvite,codIngresso,codConvite")] Convidado convidado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(convidado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["codConvite"] = new SelectList(_context.Convites, "codConvite", "codConvite", convidado.codConvite);
            ViewData["codIngresso"] = new SelectList(_context.Ingressos, "codIngresso", "ticketIngresso", convidado.codIngresso);
            return View(convidado);
        }

        // GET: Convidados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var convidado = await _context.Convidados.FindAsync(id);
            if (convidado == null)
            {
                return NotFound();
            }
            ViewData["codConvite"] = new SelectList(_context.Convites, "codConvite", "codConvite", convidado.codConvite);
            ViewData["codIngresso"] = new SelectList(_context.Ingressos, "codIngresso", "ticketIngresso", convidado.codIngresso);
            return View(convidado);
        }

        // POST: Convidados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("codConvidado,emailConvidado,telefoneConvidado,vistoConvite,confirmacaoConvite,codIngresso,codConvite")] Convidado convidado)
        {
            if (id != convidado.codConvidado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(convidado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConvidadoExists(convidado.codConvidado))
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
            ViewData["codConvite"] = new SelectList(_context.Convites, "codConvite", "codConvite", convidado.codConvite);
            ViewData["codIngresso"] = new SelectList(_context.Ingressos, "codIngresso", "ticketIngresso", convidado.codIngresso);
            return View(convidado);
        }

        // GET: Convidados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var convidado = await _context.Convidados
                .Include(c => c.Convite)
                .Include(c => c.Ingresso)
                .FirstOrDefaultAsync(m => m.codConvidado == id);
            if (convidado == null)
            {
                return NotFound();
            }

            return View(convidado);
        }

        // POST: Convidados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var convidado = await _context.Convidados.FindAsync(id);
            if (convidado != null)
            {
                _context.Convidados.Remove(convidado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConvidadoExists(int id)
        {
            return _context.Convidados.Any(e => e.codConvidado == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OFAMA.Models;
using OFAMA.Data;
using Microsoft.AspNetCore.Authorization;

namespace OFAMA.Controllers
{
    //[Authorize(Roles = "Keyword")]
    public class KeywordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeywordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Keywords
        public async Task<IActionResult> Index()
        {
            return _context.Keyword != null ?
                        View(await _context.Keyword.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Keyword'  is null.");
        }

        /*
        // GET: Keywords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Keyword == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keyword
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyword == null)
            {
                return NotFound();
            }

            return View(keyword);
        }

        */
        
        // GET: Keywords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Keywords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Keyword")] KeywordModel _keyword)
        {
            if (!_context.Keyword.Any())
            {
                if (ModelState.IsValid)
                {
                    DateTime now_date = DateTime.Now;
                    _keyword.Updated_at = now_date;
                    _context.Add(_keyword);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Keyword is already exist");
                //throw new InvalidOperationException($"Keyword is already exist ");
            }
            return View(_keyword);
        }
        
        // GET: Keywords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Keyword == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keyword.FindAsync(id);
            if (keyword == null)
            {
                return NotFound();
            }
            return View(keyword);
        }

        // POST: Keywords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Keyword")] KeywordModel _keyword)
        {
            if (id != _keyword.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var now_date = DateTime.Now;
                    _keyword.Updated_at = now_date;
                    _context.Update(_keyword);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeywordExists(_keyword.Id))
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
            return View(_keyword);
        }
        
        // GET: Keywords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Keyword == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keyword
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyword == null)
            {
                return NotFound();
            }

            return View(keyword);
        }

        // POST: Keywords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Keyword == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Keyword'  is null.");
            }
            var keyword = await _context.Keyword.FindAsync(id);
            if (keyword != null)
            {
                _context.Keyword.Remove(keyword);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        private bool KeywordExists(int id)
        {
          return (_context.Keyword?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

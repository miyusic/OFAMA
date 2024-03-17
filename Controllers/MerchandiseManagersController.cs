using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OFAMA.Data;
using OFAMA.Models;

namespace OFAMA.Controllers
{
    public class MerchandiseManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MerchandiseManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MerchandiseManagers
        public async Task<IActionResult> Index()
        {
              return _context.MerchandiseManager != null ? 
                          View(await _context.MerchandiseManager.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.MerchandiseManager'  is null.");
        }

        // GET: MerchandiseManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MerchandiseManager == null)
            {
                return NotFound();
            }

            var merchandiseManager = await _context.MerchandiseManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchandiseManager == null)
            {
                return NotFound();
            }

            return View(merchandiseManager);
        }

        // GET: MerchandiseManagers/Create
        public IActionResult Create()
        {
            //テーブルから全てのデータを取得するLINQクエリ
            var merchs = _context.Merchandise.Select(m => new { m.Id, m.ItemName });
            ViewBag.Merchs = new SelectList(merchs, "Id", "ItemName");
            //テーブルから種別を取得
            //var kindsQuery = _context.Merchandise.OrderBy(m => m.Kind).Select(m => m.Kind);
            //SelectList kinds = new SelectList((System.Collections.IEnumerable)kindsQuery.Distinct().ToListAsync());
            //return View(kinds);
            return View();
        }

        // POST: MerchandiseManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MerchId,UserId,Amount,Created_at,Updated_at")] MerchandiseManager merchandiseManager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(merchandiseManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(merchandiseManager);
        }

        // GET: MerchandiseManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //テーブルから全てのデータを取得するLINQクエリ
            var merchs = _context.Merchandise.Select(m => new { m.Id, m.ItemName });
            ViewBag.Merchs = new SelectList(merchs, "Id", "ItemName");
            if (id == null || _context.MerchandiseManager == null)
            {
                return NotFound();
            }

            var merchandiseManager = await _context.MerchandiseManager.FindAsync(id);
            if (merchandiseManager == null)
            {
                return NotFound();
            }
            return View(merchandiseManager);
        }

        // POST: MerchandiseManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MerchId,UserId,Amount,Created_at,Updated_at")] MerchandiseManager merchandiseManager)
        {
            if (id != merchandiseManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(merchandiseManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseManagerExists(merchandiseManager.Id))
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
            return View(merchandiseManager);
        }

        // GET: MerchandiseManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MerchandiseManager == null)
            {
                return NotFound();
            }

            var merchandiseManager = await _context.MerchandiseManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchandiseManager == null)
            {
                return NotFound();
            }

            return View(merchandiseManager);
        }

        // POST: MerchandiseManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MerchandiseManager == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MerchandiseManager'  is null.");
            }
            var merchandiseManager = await _context.MerchandiseManager.FindAsync(id);
            if (merchandiseManager != null)
            {
                _context.MerchandiseManager.Remove(merchandiseManager);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchandiseManagerExists(int id)
        {
          return (_context.MerchandiseManager?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

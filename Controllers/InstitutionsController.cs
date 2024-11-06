using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OFAMA.Data;
using OFAMA.Models;

namespace OFAMA.Controllers
{
    public class InstitutionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstitutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Institutions
        //[Authorize(Roles = "Institution_View, Admin_Dev")]
        public async Task<IActionResult> Index()
        {
            var institutions = await _context.Institution.OrderByDescending(x => x.Id).ToListAsync();
            ViewBag.Institutions = new SelectList(institutions, "Name");
            return _context.Institution != null ? 
                          View(institutions) :
                          Problem("Entity set 'ApplicationDbContext.Institution'  is null.");
        }

        // GET: Institutions/Create
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Institutions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                // 同じ名前の機関が存在するか確認
                var institutionExists = await _context.Institution.AnyAsync(i => i.Name == institution.Name);
                if (institutionExists)
                {
                    // 同じ名前の機関が既に存在する場合、ModelStateにエラーを追加
                    ModelState.AddModelError("Name", "指定された名前の機関は既に存在します。");
                }

                institution.Updated_at = DateTime.Now;
                _context.Add(institution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(institution);
        }

        // GET: Institutions/Edit/5
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Institution == null)
            {
                return NotFound();
            }

            var institution = await _context.Institution.FindAsync(id);
            if (institution == null)
            {
                return NotFound();
            }

            institution.Updated_at = DateTime.Now;
            return View(institution);
        }

        // POST: Institutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Institution institution)
        {
            if (id != institution.Id)
            {
                return NotFound();
            }

            // 編集された機関の名前がユニークかどうかを確認
            var institutionExists = await _context.Institution.AnyAsync(i => i.Name == institution.Name && i.Id != institution.Id);
            if (institutionExists)
            {
                // 同じ名前の機関が既に存在する場合、ModelStateにエラーを追加
                ModelState.AddModelError("Name", "指定された名前の機関は既に存在します。");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    institution.Updated_at = DateTime.Now;
                    _context.Update(institution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstitutionExists(institution.Id))
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
            return View(institution);
        }

        // GET: Institutions/Delete/5
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Institution == null)
            {
                return NotFound();
            }

            var institution = await _context.Institution
                .FirstOrDefaultAsync(m => m.Id == id);
            if (institution == null)
            {
                return NotFound();
            }

            return View(institution);
        }

        // POST: Institutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Institution_CED, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Institution == null || _context.Finance == null)
            {
                return Problem("Entity set 'OFAMAContext.Institutions' or 'OFAMAContext.Accounts' is null.");
            }

            // 指定されたInstitutionがAccountテーブルで参照されているかを確認
            var isUsedInAccounts = await _context.Finance.AnyAsync(a => a.InstiId == id); // Institutionをint型にしている場合、ここを修正
            if (isUsedInAccounts)
            {
                // InstitutionがAccountに使用されている場合、ModelStateにエラーメッセージを追加
                ModelState.AddModelError("isUsedInAccounts", "この機関は会計管理で使用されているため、削除できません。");
                // 対象のInstitutionを再取得してDeleteビューを表示
                var institutionToDelete = await _context.Institution.FindAsync(id);
                if (institutionToDelete == null)
                {
                    return NotFound();
                }
                return View("Delete", institutionToDelete); // "Delete"ビューに戻る
            }

            var institution = await _context.Institution.FindAsync(id);
            if (institution != null)
            {
                _context.Institution.Remove(institution);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InstitutionExists(int id)
        {
          return (_context.Institution?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

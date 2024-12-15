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
    public class MerchandisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MerchandisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Merchandises
        [Authorize(Roles = "Merch_View, Admin_Dev")]
        public async Task<IActionResult> Index(string searchString,string merchKind,string merchYear)
        {
            //テーブルから全てのデータを取得するLINQクエリ
            var merchs = _context.Merchandise.Select(m => m);

            //テーブルから制作年度を取得
            var yearsQuery = _context.Merchandise.OrderBy(m => m.Created_yaer.Year).Select(m => m.Created_yaer.Year);

            //テーブルから種別を取得
            var kindsQuery = _context.Merchandise.OrderBy(m => m.Kind).Select(m => m.Kind);

            //商品名検索
            if (!string.IsNullOrEmpty(searchString))
            {
                // タイトルに検索文字列が含まれるデータを抽出するLINQクエリ
                merchs = merchs.Where(s => s.ItemName!.Contains(searchString));
            }

            //種別検索
            if (!string.IsNullOrEmpty(merchKind))
            {
                // タイトルに検索文字列が含まれるデータを抽出するLINQクエリ
                merchs = merchs.Where(s => s.Kind!.Contains(merchKind));
            }

            //制作年度検索
            if (!string.IsNullOrEmpty(merchYear) && int.TryParse(merchYear, out int year))
            {
                // タイトルに検索文字列が含まれるデータを抽出するLINQクエリ
                merchs = merchs.Where(s => s.Created_yaer.Year == int.Parse(merchYear));
            }

            var MerchandiseVM = new MerchandiseViewModel
            {
                Merchandises = await merchs.OrderByDescending(x => x.Created_at).ToListAsync(),
                Kinds = new SelectList(await kindsQuery.Distinct().ToListAsync()),
                Years = new SelectList(await yearsQuery.Distinct().ToListAsync())
            };

            // ToListAsyncメソッドが呼び出されたらクエリが実行され(遅延実行)、その結果をビューに返す
            return _context.Merchandise != null ?
                          View(MerchandiseVM) :
                          Problem("Entity set 'ApplicationDbContext.Equipment'  is null.");
        }

        /*
        // GET: Merchandises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Merchandise == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }
        */

        // GET: Merchandises/Create
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Merchandises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public async Task<IActionResult> Create([Bind("Id,ItemName,Price,Kind,Created_yaer")] Merchandise merchandise)
        {
            if (ModelState.IsValid)
            {

                //名前が一致するItemのレコードを取得
                var merchname = await _context.Merchandise
                    .FirstOrDefaultAsync(m => m.ItemName == merchandise.ItemName);

                if (merchname == null)
                {
                    var now_date = DateTime.Now;
                    merchandise.Updated_at = now_date;
                    merchandise.Created_at = now_date;
                    _context.Add(merchandise);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else//同じ名前のデータは登録できない
                {
                    ModelState.AddModelError("ItemName", "この商品は既に存在しています");
                    return View(merchandise);
                }
            }
            return View(merchandise);
        }

        // GET: Merchandises/Edit/5
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Merchandise == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise == null)
            {
                return NotFound();
            }
            return View(merchandise);
        }

        // POST: Merchandises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Price,Kind,Created_yaer,Created_at")] Merchandise merchandise)
        {
            if (id != merchandise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //名前が一致するItemのレコードを取得
                    var merchname = await _context.Merchandise
                        .AsNoTracking() // トラッキングを無効化
                        .FirstOrDefaultAsync(m => m.ItemName == merchandise.ItemName);

                    if ((merchname!=null) && (merchname.Kind != merchandise.Kind || merchname.Price != merchandise.Price))
                    {
                        merchname = null;
                    }

                    if (merchname == null)
                    {
                        var update_date = DateTime.Now;
                        merchandise.Updated_at = update_date;
                        _context.Update(merchandise);
                        await _context.SaveChangesAsync();
                    }
                    else//同じ名前のデータは登録できない
                    {
                        ModelState.AddModelError("ItemName", "この商品は既に存在しています");
                        return View(merchandise);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseExists(merchandise.Id))
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
            return View(merchandise);
        }

        // GET: Merchandises/Delete/5
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Merchandise == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .FirstOrDefaultAsync(m => m.Id == id);

            //Item名を取得
            //ViewBag.MerchName = merchandise.ItemName;
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // POST: Merchandises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Merch_CED, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Merchandise == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Merchandise'  is null.");
            }
            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise != null)
            {
                // MerchManagerにこの商品を用いている人がいないかを判定
                // 存在しなければ nullを返す
                var use_merchid_data = await _context.MerchandiseManager
                    .FirstOrDefaultAsync(m => m.MerchId == id);
                if (use_merchid_data == null)
                {
                    //削除してOK
                    _context.Merchandise.Remove(merchandise);
                }
                else
                {
                    ModelState.AddModelError("use_merchid_data", "この商品は、他のレコードで使われています");
                    //エラーメッセージを吐く
                    return View(merchandise);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchandiseExists(int id)
        {
          return (_context.Merchandise?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<IdentityUser> _userManager;

        public MerchandiseManagersController(UserManager<IdentityUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MerchandiseManagers
        public async Task<IActionResult> Index
            (
                string searchNameString, 
                string searchMerchString, 
                DateTime? startDate, 
                DateTime? endDate
            )
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            // 辞書の作成
            var merchDictionary = _context.Merchandise.ToDictionary(e => e.Id, e => e.ItemName);
            // ViewBagに辞書を設定
            ViewBag.MerchDictionary = merchDictionary;

            var userDictionary = _userManager.Users.ToDictionary(e => e.Id);
            // ViewBagに辞書を設定
            ViewBag.UserDictionary = userDictionary;


            //全データのリスト
            var merchMngs = _context.MerchandiseManager.Select(m => m);

            //MerchIdのクエリ
            var merchidQuery = _context.MerchandiseManager
                .OrderBy(m => m.MerchId)
                .Select(m => m.MerchId);

            //抽出したMerchIdに対応するItemNameを取ってくる
            var merchNameQuery = _context.Merchandise
                .Join(merchidQuery,
                merch => merch.Id,
                merchid => merchid,
                (merch, merchid) => new
                {
                    MerchId = merch.Id,
                    ItemName = merch.ItemName
                }).Select(x => new { x.MerchId, x.ItemName })
                .OrderBy(x => x.ItemName);

            // ユーザ名検索処理
            if (!string.IsNullOrEmpty(searchNameString))
            {
                // 名前に検索文字列が含まれるデータを抽出する
                merchMngs = merchMngs
                    .Join
                    (
                        _userManager.Users,
                        merchManager => merchManager.UserId,
                        user => user.Id,
                        (merchManager, user) => new
                        {
                            MerchandiseManager = merchManager,
                            UserName = user.UserName
                        }
                    )
                    .Where(joinResult => joinResult.UserName.Contains(searchNameString))
                    .Select(joinResult => joinResult.MerchandiseManager);
            }

            //日付での絞り込み
            if (startDate != null)
            {
                merchMngs = merchMngs.Where(m => m.Created_at >= startDate);
            }

            if (endDate != null)
            {
                merchMngs = merchMngs.Where(m => m.Created_at <= endDate);
            }

            //商品名検索
            if (!string.IsNullOrEmpty(searchMerchString))
            {
                //備品リストから備品Idと備品名を取ってくる
                merchMngs = merchMngs
                    .Join
                    (
                        _context.Merchandise,
                        merchManager => merchManager.MerchId,
                        merch => merch.Id,
                        (merchManager, merch) => new
                        {
                            MerchandiseManager = merchManager,
                            ItemName = merch.ItemName
                        }
                    )
                    .Where(joinResult => joinResult.ItemName.Contains(searchMerchString))
                    .Select(joinResult => joinResult.MerchandiseManager);
            }

            var merchMngVM = new MerchandiseManagerViewModel
            {
                MerchKinds = new SelectList(await merchNameQuery.Distinct().ToListAsync()),
                MerchManagers = await merchMngs.ToListAsync()
            };

            return _context.MerchandiseManager != null ? 
                          View(merchMngVM) :
                          Problem("Entity set 'ApplicationDbContext.MerchandiseManager'  is null.");
        }

        /*
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
        */

        // GET: MerchandiseManagers/Create
        public IActionResult Create()
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var merchs = _context.Merchandise
                .Select(m => new { m.Id, m.ItemName })
                .OrderBy(user => user.ItemName);
            ViewBag.Merchs = new SelectList(merchs, "Id", "ItemName");
            ViewBag.Today = DateTime.Today;
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
        public async Task<IActionResult> Create([Bind("Id,MerchId,UserId,Amount,Created_at")] MerchandiseManager merchandiseManager)
        {
            if (ModelState.IsValid)
            {
                var now_date = DateTime.Now;
                merchandiseManager.Updated_at = now_date;
                //merchandiseManager.Created_at = now_date;
                _context.Add(merchandiseManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(merchandiseManager);
        }

        // GET: MerchandiseManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var merchs = _context.Merchandise
                .Select(m => new { m.Id, m.ItemName })
                .OrderBy(user => user.ItemName);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,MerchId,UserId,Amount,Created_at")] MerchandiseManager merchandiseManager)
        {
            if (id != merchandiseManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var update_date = DateTime.Now;
                    merchandiseManager.Updated_at = update_date;
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

            //ユーザ名を取得
            var username = await _userManager.Users
                .Where(user => user.Id == merchandiseManager.UserId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync();
            ViewBag.UserName = username;

            //Item名を取得
            var merchname = await _context.Merchandise
                .Where(user => user.Id == merchandiseManager.MerchId)
                .Select(m => m.ItemName)
                .FirstOrDefaultAsync();
            ViewBag.MerchName = merchname;

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

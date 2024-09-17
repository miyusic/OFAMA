using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        private readonly Claim _claim;

        public MerchandiseManagersController(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        }

        // GET: MerchandiseManagers
        //[Authorize(Roles = "MerchMng_View, Admin_Dev")]
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
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
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
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
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
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
        public async Task<IActionResult> Edit(int? id)
        {
            //ログインしていない場合
            if (_claim == null)
            {
                return Forbid();
            }

            if (id == null || _context.MerchandiseManager == null)
            {
                return NotFound();
            }

            var merchandiseManager = await _context.MerchandiseManager.FindAsync(id);
            if (merchandiseManager == null)
            {
                return NotFound();
            }

            //ログインIDで制御
            var loginusernameid = _claim.Value;
            if ((loginusernameid != merchandiseManager.UserId) & !(JudgeAdminAccess()))
            {
                return Forbid();
            }

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
            ViewBag.Merchs = new SelectList(merchs, "Id", "ItemName");

            return View(merchandiseManager);
        }

        // POST: MerchandiseManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
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
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
        public async Task<IActionResult> Delete(int? id)
        {
            //ログインしていない場合
            if (_claim == null)
            {
                return Forbid();
            }

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

            //ログインIDで制御
            var loginusernameid = _claim.Value;
            if ((loginusernameid != merchandiseManager.UserId) & !(JudgeAdminAccess()))
            {
                return Forbid();
            }

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

            return View(merchandiseManager);
        }

        // POST: MerchandiseManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
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

        // GET: MerchandiseManagers/Move/5
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
        public async Task<IActionResult> Move(int? id)
        {
            //ログインしていない場合
            if (_claim == null)
            {
                return Forbid();
            }

            if (id == null || _context.MerchandiseManager == null)
            {
                return NotFound();
            }

            var merchManager = await _context.MerchandiseManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchManager == null)
            {
                return NotFound();
            }

            //ログインIDで制御
            var loginusernameid = _claim.Value;
            if ((loginusernameid != merchManager.UserId) & !(JudgeAdminAccess()))
            {
                return Forbid();
            }

            /* 表示データ */
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var equips = _context.Merchandise
                .Select(m => new { m.Id, m.ItemName })
                .OrderBy(user => user.ItemName);
            ViewBag.Eqips = new SelectList(equips, "Id", "ItemName");

            //ユーザ名を取得
            var username = await _userManager.Users
                .Where(user => user.Id == merchManager.UserId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync();
            ViewBag.UserName = username;

            //Item名を取得
            var merchname = await _context.Merchandise
                .Where(user => user.Id == merchManager.MerchId)
                .Select(m => m.ItemName)
                .FirstOrDefaultAsync();
            ViewBag.MerchName = merchname;

            //その他情報をviewBagに格納
            ViewBag.Amount = merchManager.Amount;
            ViewBag.Created_at = merchManager.Created_at;
            ViewBag.Updated_at = merchManager.Updated_at;
            /* ここまで */

            return View();
        }

        // POST: MerchandiseManagers/Move/5
        [HttpPost, ActionName("Move")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "MerchMng_CEMD, Admin_Dev")]
        public async Task<IActionResult> MoveData(int id, [Bind("UserId1,Amount1,UserId2,Amount2,UserId3,Amount3")] ItemManagerMove itemmernagemove)
        {

            var merchManager = await _context.MerchandiseManager.FirstOrDefaultAsync(m => m.Id == id);
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var equips = _context.Merchandise
                .Select(m => new { m.Id, m.ItemName })
                .OrderBy(user => user.ItemName);
            ViewBag.Eqips = new SelectList(equips, "Id", "ItemName");

            if (merchManager != null)
            {
                /* 表示データ */
                //ユーザ名を取得
                var username = await _userManager.Users
                    .Where(user => user.Id == merchManager.UserId)
                    .Select(user => user.UserName)
                    .FirstOrDefaultAsync();
                ViewBag.UserName = username;

                //Item名を取得
                var merchname = await _context.Merchandise
                    .Where(user => user.Id == merchManager.MerchId)
                    .Select(m => m.ItemName)
                    .FirstOrDefaultAsync();
                ViewBag.MerchName = merchname;

                //その他情報をviewBagに格納
                ViewBag.Amount = merchManager.Amount;
                ViewBag.Created_at = merchManager.Created_at;
                ViewBag.Updated_at = merchManager.Updated_at;
                /* ここまで */
            }

            if (ModelState.IsValid)
            {
                if (merchManager != null)
                {
                    if (merchManager.UserId == itemmernagemove.UserId1)
                    {
                        ModelState.AddModelError("UserId1", "元データと同じユーザ名が指定されています");
                        return View(itemmernagemove);
                    }

                    // 数量を保存
                    var amount_before = merchManager.Amount;
                    List<int> amount_list = new List<int> { itemmernagemove.Amount1 };
                    List<string> userid_list = new List<string> { itemmernagemove.UserId1 };

                    // 2つ目のデータがあれば追加
                    if (!string.IsNullOrEmpty(itemmernagemove.UserId2) && itemmernagemove.Amount2 > 0)
                    {
                        if (merchManager.UserId == itemmernagemove.UserId2)
                        {
                            ModelState.AddModelError("UserId2", "元データと同じユーザ名が指定されています");
                            return View(itemmernagemove);
                        }
                        if (itemmernagemove.UserId1 == itemmernagemove.UserId2)
                        {
                            ModelState.AddModelError("UserId1", "同じユーザ名が指定されています");
                            ModelState.AddModelError("UserId2", "同じユーザ名が指定されています");

                            // 3つ目のデータも被りがあれば、エラーメッセージを出す
                            if (!string.IsNullOrEmpty(itemmernagemove.UserId3) && itemmernagemove.Amount3 > 0)
                            {
                                if ((itemmernagemove.UserId1 == itemmernagemove.UserId3)
                                    && (itemmernagemove.UserId2 == itemmernagemove.UserId3))
                                {
                                    ModelState.AddModelError("UserId3", "同じユーザ名が指定されています");
                                }
                            }
                            return View(itemmernagemove);
                        }
                        userid_list.Add(itemmernagemove.UserId2);
                        amount_list.Add((int)itemmernagemove.Amount2);
                    }
                    // 3つ目のデータがあれば追加
                    if (!string.IsNullOrEmpty(itemmernagemove.UserId3) && itemmernagemove.Amount3 > 0)
                    {
                        if (merchManager.UserId == itemmernagemove.UserId3)
                        {
                            ModelState.AddModelError("UserId3", "元データと同じユーザ名が指定されています");
                            return View(itemmernagemove);
                        }

                        if (itemmernagemove.UserId1 == itemmernagemove.UserId3)
                        {
                            ModelState.AddModelError("UserId1", "同じユーザ名が指定されています");
                            ModelState.AddModelError("UserId3", "同じユーザ名が指定されています");
                            if (itemmernagemove.UserId2 == itemmernagemove.UserId3)
                            {
                                ModelState.AddModelError("UserId2", "同じユーザ名が指定されています");
                            }
                            return View(itemmernagemove);
                        }
                        else
                        {
                            if (itemmernagemove.UserId2 == itemmernagemove.UserId3)
                            {
                                ModelState.AddModelError("UserId2", "同じユーザ名が指定されています");
                                ModelState.AddModelError("UserId3", "同じユーザ名が指定されています");
                                return View(itemmernagemove);
                            }
                        }
                        userid_list.Add(itemmernagemove.UserId3);
                        amount_list.Add((int)itemmernagemove.Amount3);
                    }

                    // もし、数量がマイナスになるならエラーメッセージを出す
                    if (amount_before - amount_list.Sum() < 0)
                    {
                        ModelState.AddModelError("Amount1", "合計値が元の数量を超えます");
                        ModelState.AddModelError("Amount2", "合計値が元の数量を超えます");
                        ModelState.AddModelError("Amount3", "合計値が元の数量を超えます");
                        return View(itemmernagemove);
                    }
                    else
                    {
                        // 登録データの作成
                        var now_date = DateTime.Now;

                        // データのコピー + 登録
                        for (int i = 0; i < amount_list.Count; i++)
                        {
                            // データをコピーして新しいインスタンスを作成
                            var newMerchdiseManager = new MerchandiseManager
                            {
                                // Idを除いた全てのプロパティをコピーする
                                // 元のデータとは違うデータはここで定義しない
                                MerchId = merchManager.MerchId,
                                UserId = userid_list[i],
                                Amount = amount_list[i],
                                Created_at = now_date,
                                Updated_at = now_date
                            };
                            // データの追加
                            _context.Add(newMerchdiseManager);

                        }

                        //前のデータをいじる
                        //もし、移動の結果、数量が0になるなら削除
                        if (amount_before - amount_list.Sum() == 0)
                        {
                            if (merchManager != null)
                            {
                                _context.MerchandiseManager.Remove(merchManager);
                            }
                        }
                        else//更新日時と、数量を変更する
                        {
                            merchManager.Amount = amount_before - amount_list.Sum();
                            merchManager.Updated_at = now_date;
                            _context.Update(merchManager);
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(itemmernagemove);
        }

        //管理者のアクセス制限を判定する関数
        private bool JudgeAdminAccess()
        {
            // 1. ユーザーが権限を持っているか確認
            if (User.IsInRole("Admin_Dev"))
            {
                // 権限がある場合は
                return true;
            }
            return false;
        }
        private bool MerchandiseManagerExists(int id)
        {
          return (_context.MerchandiseManager?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

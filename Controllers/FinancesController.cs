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
    public class FinancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FinancesController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Finances
        public async Task<IActionResult> Index(string financeReceived, string financeInstitution,string searchString, string searchNameString)
        {
            var instituteQuery = _context.Institution
            .OrderBy(i => i.Name)
            .Select(i => i.Name)
            .Distinct();

            // Create a dictionary for institution names
            var institutionDictionary = await _context.Institution
                .ToDictionaryAsync(i => i.Id, i => i.Name);

            // Add the dictionary to ViewBag
            ViewBag.InstitutionDictionary = institutionDictionary;


            // 機関名のリストを取得するクエリを構築
            var instiQuery = _context.Institution
                .OrderBy(i => i.Name)
                .Select(i => i.Name)
                .Distinct();

            // 受取種別のクエリ
            var receivedQuery = _context.Finance
                .OrderBy(m => m.Received)
                .Select(m => m.Received)
                .Distinct();

            // アカウントの基本クエリ
            var finances = _context.Finance.AsQueryable();

            // 用途検索処理
            if (!string.IsNullOrEmpty(searchString))
            {
                finances = finances.Where(s => s.Way.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(searchNameString))
            {
                // 名前に検索文字列が含まれるデータを抽出する
                finances = finances
                    .Join
                    (
                        _userManager.Users,
                        finances => finances.UserId,
                        user => user.Id,
                        (finances, user) => new
                        {
                            Finance = finances,
                            UserName = user.UserName
                        }
                    )
                    .Where(joinResult => joinResult.UserName.Contains(searchNameString))
                    .Select(joinResult => joinResult.Finance);
                //finances = finances.Where(s => s.UserId == searchUserId);
            }

            // 機関での絞り込み
            if (!string.IsNullOrEmpty(financeInstitution))
            {
                //機関DBから機関Idと機関名を取ってくる
                finances = finances
                    .Join
                    (
                        _context.Institution,
                        finance => finance.InstiId,
                        institution => institution.Id,
                        (finance, institution) => new
                        {
                            Finance = finance,
                            ItemName = institution.Name
                        }
                    )
                    .Where(joinResult => joinResult.ItemName.Contains(financeInstitution))
                    .Select(joinResult => joinResult.Finance);
            }

                // 受取種別検索処理
                if (!string.IsNullOrEmpty(financeReceived))
            {
                finances = finances.Where(x => x.Received == financeReceived);
            }

            // 用途ごとの合計金額を計算
            // 合計金額の計算はデータベースクエリで直接行う
            var wayTotalAmounts = await finances
                .GroupBy(a => a.Received)
                .Select(group => new
                {
                    ReceivedType = group.Key.ToString(),
                    Total = group.Sum(a => a.Received == "入金" ? a.Money : -a.Money)
                }).ToListAsync();

            var moneytotal = wayTotalAmounts.Sum(item => item.Total);

            // ユーザの名前
            var userDictionary = _userManager.Users.ToDictionary(e => e.Id);
            // ViewBagに辞書を設定
            ViewBag.UserDictionary = userDictionary;

            // 機関名
            var instiDictionary = _context.Institution.ToDictionary(e => e.Id, e => e.Name);
            // ViewBagに辞書を設定
            ViewBag.InstiDictionary = instiDictionary;

            // ViewModelの構築
            var financeVM = new FinanceViewModel
            {
                Institutions = new SelectList(await instiQuery.ToListAsync()),
                Receiveds = new SelectList(await receivedQuery.ToListAsync()),
                Finances = await finances.ToListAsync(),
                Financemoneytotal = moneytotal,
                WayTotalAmounts = wayTotalAmounts.Sum(w => w.Total)
            };

            return _context.Finance != null ? 
                          View(financeVM) :
                          Problem("Entity set 'ApplicationDbContext.Finance'  is null.");
        }

        // GET: Finances/Create
        public IActionResult Create()
        {
            var institutions = _context.Institution
                .Select(i => new { i.Id, i.Name })
                .OrderBy(i => i.Name);
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);

            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Institutions = new SelectList(institutions, "Id", "Name");

            return View();
        }

        // POST: Finances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Money,Received,InstiId,Way,Created_at")] Finance finance)
        {
            // CreateDateがデフォルト値の場合にエラーを追加
            if (finance.Created_at == DateTime.MinValue)
            {
                ModelState.AddModelError("Created_at", "日付を入力してください");
            }
            if (ModelState.IsValid)
            {
                finance.Updated_at = DateTime.Now;
                _context.Add(finance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(finance);
        }

        // GET: Finances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Finance == null)
            {
                return NotFound();
            }

            var finance = await _context.Finance.FindAsync(id);
            if (finance == null)
            {
                return NotFound();
            }
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            var institutions = _context.Institution
                .Select(i => new { i.Id, i.Name })
                .OrderBy(i => i.Name);
            ViewBag.Institutions = new SelectList(institutions, "Id", "Name");
            return View(finance);
        }

        // POST: Finances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Money,Received,InstiId,Way,Created_at")] Finance finance)
        {
            if (id != finance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    finance.Updated_at = DateTime.Now;
                    _context.Update(finance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinanceExists(finance.Id))
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
            return View(finance);
        }

        // GET: Finances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Finance == null)
            {
                return NotFound();
            }

            var finance = await _context.Finance
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finance == null)
            {
                return NotFound();
            }
            ViewBag.instiName = await _context.Institution
                                .Where(m => m.Id == finance.InstiId)
                                .Select(m => m.Name)
                                .FirstOrDefaultAsync();
            ViewBag.userName = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == finance.UserId);
            return View(finance);
        }

        // POST: Finances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Finance == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Finance'  is null.");
            }
            var finance = await _context.Finance.FindAsync(id);
            if (finance != null)
            {
                _context.Finance.Remove(finance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinanceExists(int id)
        {
          return (_context.Finance?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

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
    public class BorrowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Claim _claim;

        public BorrowsController(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        }

        // GET: Borrows
        //[Authorize(Roles = "Borrow_View, Admin_Dev")]
        public async Task<IActionResult> Index(string borrowStatus, string searchString, string searchNameString)
        {
            // Borrowテーブルから全てのジャンルを取得するLINQクエリ
            var statusQuery = _context.Borrow
                .OrderBy(m => m.Status)
                .Select(m => m.Status);

            // Borrowテーブルから全てのデータを取得するLINQクエリ
            var borrows = _context.Borrow.Select(m => m);

            //ユーザ名の辞書を作成
            var userDictionary = _userManager.Users.ToDictionary(e => e.Id);
            // ViewBagに辞書を設定
            ViewBag.UserDictionary = userDictionary;

            //名前検索
            if (!string.IsNullOrEmpty(searchNameString))
            {
                borrows = borrows
                    .Join(
                        _userManager.Users,
                        bollow => bollow.UserId,
                        user => user.Id,
                        (borrow, user) => new
                        {
                            Borrow = borrow,
                            UserName = user.UserName
                        }
                    )
                    .Where(joinResult => joinResult.UserName.Contains(searchNameString))
                    .Select(joinResult => joinResult.Borrow);
            }

            // タイトル検索処理
            if (!string.IsNullOrEmpty(searchString))
            {
                // タイトルに検索文字列が含まれるデータを抽出するLINQクエリ
                borrows = borrows.Where(s => s.Usage!.Contains(searchString));
            }

            // ジャンル検索処理
            if (!string.IsNullOrEmpty(borrowStatus))
            {
                // 選択したジャンルがと一致するデータを抽出するLINQクエリ
                borrows = borrows.Where(x => x.Status == borrowStatus);
            }

            
            // ジャンルと抽出した映画データをそれぞれリストにしてプロパティに格納する
            var borrowStatusVM = new BorrowViewModel
            {
                Statuses = new SelectList(await statusQuery.Distinct().ToListAsync()),
                Borrows = await borrows.OrderByDescending(x => x.Id).ToListAsync()
            };
            

            return borrowStatusVM != null ?
                    View(borrowStatusVM) :
                    Problem("Entity set 'ApplicationDbContext.Borrow'  is null.");

            /*
            return borrows != null ?
                    View(borrows) :
                    Problem("Entity set 'ApplicationDbContext.Borrow'  is null.");
            */
        }

        // GET: Borrows/Create
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public IActionResult Create()
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            return View();
        }

        // POST: Borrows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public async Task<IActionResult> Create([Bind("Id,Status,UserId,BorrowMoney,Usage,BorrowDate")] Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                borrow.Updated_at = DateTime.Now;
                _context.Add(borrow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(borrow);
        }

        // GET: Borrows/Edit/5
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int? id)
        {
            //ログインしていない場合
            if (_claim == null)
            {
                return Forbid();

            }

            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow == null)
            {
                return NotFound();
            }
            //ログインIDで制御
            var loginusernameid = _claim.Value;
            if ((loginusernameid != borrow.UserId) & !(JudgeAdminAccess()))
            {
                return Forbid();
            }

            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            return View(borrow);
        }

        // POST: Borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,UserId,BorrowMoney,Usage,BorrowDate,ReturnDate,Updated_at")] Borrow borrow)
        {
            if (id != borrow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    borrow.Updated_at = DateTime.Now;
                    _context.Update(borrow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowExists(borrow.Id))
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
            return View(borrow);
        }

        // GET: Borrows/Delete/5
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public async Task<IActionResult> Delete(int? id)
        {
            //ログインしていない場合
            if (_claim == null)
            {
                return Forbid();

            }

            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            //ログインIDで制御
            var loginusernameid = _claim.Value;
            if ((loginusernameid != borrow.UserId) & !(JudgeAdminAccess()))
            {
                return Forbid();
            }
            ViewBag.userName = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == borrow.UserId);

            return View(borrow);
        }

        // POST: Borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Borrow_CED, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Borrow == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Borrow'  is null.");
            }
            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow != null)
            {
                _context.Borrow.Remove(borrow);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Borrows/Return/5
        //[Authorize(Roles = "Borrow_Return, Admin_Dev")]
        public async Task<IActionResult> Return(int? id)
        {

            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow.FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }
            ViewBag.userName = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == borrow.UserId);
            return View(borrow);
        }

        // POST: Borrows/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Borrow_Return, Admin_Dev")]
        public async Task<IActionResult> ReturnConfirmed(int id, DateTime returnDate, [Bind("Id,Status,UserId,BorrowMoney,Usage,BorrowDate,ReturnDate")] Borrow borrow)
        {
            if (_context.Borrow == null)
            {
                return Problem("Entity set '返却日を入力してください。");
            }

            if (!ModelState.IsValid)
            {
                // エラーがある場合、Returnビューを再表示
                var borrow_to_return = await _context.Borrow.FirstOrDefaultAsync(m => m.Id == id);
                if (borrow_to_return == null)
                {
                    return NotFound();
                }
                return View("Return", borrow_to_return);
            }

            var borrow_at = await _context.Borrow.FindAsync(id);
            if (borrow_at != null)
            {
                borrow_at.Status = "済";
                borrow_at.ReturnDate = returnDate; // 返却日時を設定
                _context.Borrow.Update(borrow_at);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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

        private bool BorrowExists(int id)
        {
          return (_context.Borrow?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

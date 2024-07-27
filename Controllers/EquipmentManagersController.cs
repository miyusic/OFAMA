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
    public class EquipmentManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public EquipmentManagersController(UserManager<IdentityUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EquipmentManagers
        public async Task<IActionResult> Index(string searchNameString,string searchEquipString, DateTime? startDate,DateTime? endDate)
        {
            //データのリスト
            var equipMngs = _context.EquipmentManager.Select(m => m);
            //var equips = _context.Equipment.Select(m => m);


            //EquipIdのクエリ
            var equipidQuery = _context.EquipmentManager
                .OrderBy(m => m.EquipId)
                .Select(m => m.EquipId);

            //抽出したEquipIdに対応するEquipNameを取ってくる
            var equipNameQuery = _context.Equipment
                .Join(equipidQuery,
                equip => equip.Id,
                equipid => equipid,
                (equip,equipid)=> new 
                {
                    EquipId = equip.Id,
                    ItemName = equip.ItemName
                }).Select(x => new {x.EquipId, x.ItemName}).OrderBy(x => x.ItemName);

            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            

            var userDictionary = _userManager.Users.ToDictionary(e => e.Id);
            // ViewBagに辞書を設定
            ViewBag.UserDictionary = userDictionary;

            //中身のデータをリストに格納
            //var eqipList = _context.EquipmentManager.ToList();
            // 辞書の作成
            var equipDictionary = _context.Equipment.ToDictionary(e => e.Id, e => e.ItemName);

            // ViewBagに辞書を設定
            ViewBag.EquipDictionary = equipDictionary;

            /*foreach (var eqip in eqipList) 
            {
                // EquipIdに対応するEquipmentのデータを取得
                //var equipment = _context.Equipment.FirstOrDefault(e => e.Id == eqipList);

                // EquipmentItemNameプロパティに備品名を格納
                //equipmentManager.EquipmentItemName = equipment?.ItemName;
            }*/

            // ユーザ名検索処理
            if (!string.IsNullOrEmpty(searchNameString))
            {
                /*
                foreach (KeyValuePair<int, string> kvp in equipDictionary)
                {
                    if (searchString == equipDictionary[kvp.Key])
                    {
                        // タイトルに検索文字列が含まれるデータを抽出する
                        equipMngs = equipMngs.Where(s => s.EquipId == kvp.Key);
                        break;
                    }
                }
                */
                // 名前に検索文字列が含まれるデータを抽出する
                
                equipMngs = equipMngs
                    .Join
                    (
                        _userManager.Users,
                        equipmentManager => equipmentManager.UserId,
                        user => user.Id,
                        (equipmentManager, user) => new
                        {
                            EquipmentManager = equipmentManager,
                            UserName = user.UserName
                        }
                    )
                    .Where(joinResult => joinResult.UserName.Contains(searchNameString))
                    .Select(joinResult => joinResult.EquipmentManager);

                //equipMngs = equipMngs;
            }

            //日付での絞り込み
            if (startDate != null)
            {
                equipMngs = equipMngs.Where(m => m.Created_at >= startDate);
            }

            if(endDate != null)
            {
                equipMngs = equipMngs.Where(m => m.Created_at <= endDate);
            }

            //備品名検索
            if (!string.IsNullOrEmpty(searchEquipString))
            {
                //備品リストから備品Idと備品名を取ってくる
                /*var equips = _context.Equipment
                    .Where(s => s.ItemName.Contains(searchEquipString))
                    .Select(s=> new {s.Id,s.ItemName});*/

                equipMngs = equipMngs
                    .Join
                    (
                        _context.Equipment,
                        equipmentManager => equipmentManager.EquipId,
                        equipment => equipment.Id,
                        (equipmentManager, equipment) => new
                        {
                            EquipmentManager = equipmentManager,
                            ItemName = equipment.ItemName
                        }
                    )
                    .Where(joinResult => joinResult.ItemName.Contains(searchEquipString))
                    .Select(joinResult => joinResult.EquipmentManager);
            }

            var equipMngVM = new EquipmentManagerViewModel
            {
                EquipIds = new SelectList(await equipNameQuery.Distinct().ToListAsync()),
                EquipmentManagers = await equipMngs.ToListAsync()
            };

            return _context.EquipmentManager != null ? 
                          View(equipMngVM) :
                          Problem("Entity set 'ApplicationDbContext.EquipmentManager'  is null.");
        }

        /*
        // GET: EquipmentManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EquipmentManager == null)
            {
                return NotFound();
            }

            var equipmentManager = await _context.EquipmentManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentManager == null)
            {
                return NotFound();
            }

            return View(equipmentManager);
        }
        */

        // GET: EquipmentManagers/Create
        public IActionResult Create()
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var equips = _context.Equipment
                .Select(m => new { m.Id ,m.ItemName})
                .OrderBy(user => user.ItemName);
            ViewBag.Eqips = new SelectList(equips, "Id", "ItemName");
            ViewBag.Today =  DateTime.Today;
            return View();
        }

        // POST: EquipmentManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EquipId,UserId,Amount,Created_at")] EquipmentManager equipmentManager)
        {
            if (ModelState.IsValid)
            {
                var now_date = DateTime.Now;
                equipmentManager.Updated_at = now_date;
                //equipmentManager.Created_at = now_date;
                _context.Add(equipmentManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipmentManager);
        }

        // GET: EquipmentManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //ユーザリスト
            var users = _userManager.Users
                .Select(user => new { user.Id, user.UserName })
                .OrderBy(user => user.UserName);
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            //テーブルから全てのデータを取得するLINQクエリ
            var equips = _context.Equipment
                .Select(m => new { m.Id, m.ItemName })
                .OrderBy(user => user.ItemName);
            ViewBag.Eqips = new SelectList(equips, "Id", "ItemName");
            if (id == null || _context.EquipmentManager == null)
            {
                return NotFound();
            }

            var equipmentManager = await _context.EquipmentManager.FindAsync(id);
            if (equipmentManager == null)
            {
                return NotFound();
            }
            return View(equipmentManager);
        }

        // POST: EquipmentManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EquipId,UserId,Amount,Created_at")] EquipmentManager equipmentManager)
        {
            if (id != equipmentManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var update_date = DateTime.Now;
                    equipmentManager.Updated_at = update_date;
                    _context.Update(equipmentManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentManagerExists(equipmentManager.Id))
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
            return View(equipmentManager);
        }

        // GET: EquipmentManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipmentManager == null)
            {
                return NotFound();
            }

            var equipmentManager = await _context.EquipmentManager
                .FirstOrDefaultAsync(m => m.Id == id);

            //ユーザ名を取得
            var username = await _userManager.Users
                .Where(user => user.Id == equipmentManager.UserId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync();
            ViewBag.UserName = username;

            //Item名を取得
            var equipname = await _context.Equipment
                .Where(user => user.Id == equipmentManager.EquipId)
                .Select(m => m.ItemName)
                .FirstOrDefaultAsync();
            ViewBag.EquipName = equipname;

            if (equipmentManager == null)
            {
                return NotFound();
            }

            return View(equipmentManager);
        }

        // POST: EquipmentManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EquipmentManager == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EquipmentManager'  is null.");
            }
            var equipmentManager = await _context.EquipmentManager.FindAsync(id);
            if (equipmentManager != null)
            {
                _context.EquipmentManager.Remove(equipmentManager);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentManagerExists(int id)
        {
          return (_context.EquipmentManager?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

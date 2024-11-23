using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OFAMA.Data;
using OFAMA.Models;
using Microsoft.AspNetCore.Authorization;

namespace OFAMA.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Equipments
        //[Authorize(Roles = "Equip_View, Admin_Dev")]
        public async Task<IActionResult> Index(string searchExpandableString, string searchString)
        {
            //テーブルから全てのデータを取得するLINQクエリ
            var equips = _context.Equipment.Select(m => m);

            //テーブルから消耗品かどうかを取得する
            //var expandables = _context.Equipment.Select(m => m.isExpandable);

            //備品名検索
            if (!string.IsNullOrEmpty(searchString))
            {
                // タイトルに検索文字列が含まれるデータを抽出するLINQクエリ
                equips = equips.Where(s => s.ItemName!.Contains(searchString));
            }

            //Console.WriteLine(searchExpandableString);
            //消耗品かの絞り込み
            if (!string.IsNullOrEmpty(searchExpandableString))
            {
                if (searchExpandableString.Equals("消耗品"))
                {
                    //消耗品のとき
                    equips = equips.Where(s => s.isExpandable!.Equals(true));
                }
                else if (searchExpandableString.Equals("消耗品ではない"))
                {
                    //消耗品ではないとき
                    equips = equips.Where(s => s.isExpandable!.Equals(false));
                }
            }

            //リストの初期化
            var expandables = new List<String>
            {
                "消耗品",
                "消耗品ではない"
            };

            var equipExpandableVM = new EquipmentViewModel
            {
                Equipments = await equips.OrderByDescending(x => x.Created_at).ToListAsync(),
                Expandables = new SelectList(expandables)
            };

            // ToListAsyncメソッドが呼び出されたらクエリが実行され(遅延実行)、その結果をビューに返す
            return _context.Equipment != null ? 
                          View(equipExpandableVM) :
                          Problem("Entity set 'ApplicationDbContext.Equipment'  is null.");
        }

        /*
        // GET: Equipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Equipment == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }
        */

        // GET: Equipments/Create
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]
        public async Task<IActionResult> Create([Bind("Id,ItemName,isExpandable")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                //名前が一致するItemのレコードを取得
                var equipname = await _context.Equipment
                    .FirstOrDefaultAsync(m => m.ItemName == equipment.ItemName);

                if (equipname == null)
                {
                    //異なる名前なのでOK
                    var now_date = DateTime.Now;
                    equipment.Updated_at = now_date;
                    equipment.Created_at = now_date;
                    _context.Add(equipment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else//同じ名前のデータは登録できない
                {
                    ModelState.AddModelError("ItemName", "この備品は既に存在しています");
                    return View(equipment);
                }
            }
            return View(equipment);
        }

        // GET: Equipments/Edit/5
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Equipment == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,isExpandable","Created_at")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //名前と消耗品のフラグが一致するItemのレコードを取得
                    var equipdata = await _context.Equipment
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.ItemName == equipment.ItemName && m.isExpandable == equipment.isExpandable);

                    if (equipdata == null)
                    {
                        //名前が同じモノがあるかを取得
                        var equipname = await _context.Equipment
                            .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.ItemName == equipment.ItemName);

                        //現在のIDのデータを取得
                        var equipcurrentid = await _context.Equipment
                            .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.Id == equipment.Id);

                        //名前が異なる or //名前は同じだけど消耗品のフラグが違う
                        if ((equipname == null) || ((equipcurrentid.ItemName == equipment.ItemName) && (equipcurrentid.isExpandable != equipment.isExpandable)))
                        {
                            //異なる名前or消耗品のフラグなのでOK
                            var update_date = DateTime.Now;
                            equipment.Updated_at = update_date;
                            _context.Update(equipment);
                            await _context.SaveChangesAsync();
                        }
                        else//同じ名前のデータは登録できない
                        {
                            ModelState.AddModelError("ItemName", "この備品は既に存在しています");
                            return View(equipment);
                        }
                    }
                    else//同じ名前のデータは登録できない
                    {
                        ModelState.AddModelError("ItemName", "この備品は既に存在しています");
                        return View(equipment);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.Id))
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
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Equipment == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Equip_CED, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Equipment == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Equipment'  is null.");
            }
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                // EquipManagerにこの備品を用いている人がいないかを判定
                // 存在しなければ nullを返す
                var use_equipid_data = await _context.EquipmentManager
                    .FirstOrDefaultAsync(m => m.EquipId == id);
                if (use_equipid_data == null)
                {
                    //削除してOK
                    _context.Equipment.Remove(equipment);
                }
                else
                {
                    ModelState.AddModelError("use_equipid_data", "この備品は、他のレコードで使われています");
                    //エラーメッセージを吐く
                    return View(equipment);
                }

            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
          return (_context.Equipment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

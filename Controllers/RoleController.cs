using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// 追加
using Microsoft.AspNetCore.Identity;
using OFAMA.Data;
using Microsoft.EntityFrameworkCore;
using OFAMA.Models;
using Microsoft.AspNetCore.Authorization;

namespace OFAMA.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: Role/Index
        // Model は RoleModel
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> Index()
        {
            var roles = from r in _roleManager.Roles
                        orderby r.Name
                        select new RoleModel
                        {
                            Id = r.Id,
                            Name = r.Name
                        };

            return View(await roles.ToListAsync());
        }


        // GET: Role/Details/5・・・省略
        // ロール名以外の情報は含まれないので不要


        // GET: Role/Create
        // Model は RoleModel クラス
        //[Authorize(Roles = "Role_Create")]
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Role_Create")]
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> Create([Bind("Id,Name")] RoleModel rolemodel)
        {

            if (ModelState.IsValid)
            {
                // ユーザーが入力したロール名を model.Name から
                // 取得し IdentityRole オブジェクトを生成
                var role = new IdentityRole { Name = rolemodel.Name };

                //　上の IdentityRole から新規ロールを作成・登録
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    // 登録に成功したら Role/Index にリダイレクト
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    // result.Succeeded が false の場合 ModelSate にエ
                    // ラー情報を追加しないとエラーメッセージが出ない。
                    // Register.cshtml.cs のものをコピー
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty,
                                                 error.Description);
                    }
                }
            }

            // ロールの登録に失敗した場合、登録画面を再描画
            return View(rolemodel);
        }

        // GET: Role/Edit/5
        // Edit でできるのはロール名の変更のみ
        // Model は RoleModel クラス
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _roleManager.FindByIdAsync(id);

            if (target == null)
            {
                return NotFound();
            }

            RoleModel model = new RoleModel
            {
                Name = target.Name
            };

            return View(model);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> Edit(string id,
                        [Bind("Id,Name")] RoleModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var target = await _roleManager.FindByIdAsync(id);
                if (target == null)
                {
                    return NotFound();
                }

                // ユーザーが入力したロール名を model.Name から取得し
                // て IdentityRole の Name を書き換え
                if (target.Name == "Admin_Dev")
                {
                    ModelState.AddModelError("target.Name","Admin_Devは変更できません");
                    // 更新に失敗した場合、編集画面を再描画
                    return View(model);
                }
                target.Name = model.Name;

                // Name を書き換えた IdentityRole で更新をかける
                var result = await _roleManager.UpdateAsync(target);

                if (result.Succeeded)
                {
                    // 更新に成功したら Role/Index にリダイレクト
                    return RedirectToAction("Index", "Role");
                }

                // result.Succeeded が false の場合 ModelSate にエ
                // ラー情報を追加しないとエラーメッセージが出ない。
                // Register.cshtml.cs のものをコピー
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                                             error.Description);
                }
            }

            // 更新に失敗した場合、編集画面を再描画
            return View(model);
        }

        // GET: Role/Delete/5
        // 階層更新が行われているようで、ユーザーがアサインされて
        // いるロールも削除可能。
        // Model は RoleModel
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var model = new RoleModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }

        // POST: Role/Delete/5
        // 上の Delete(string id) と同シグネチャのメソッド
        // は定義できないので、メソッド名を変えて、下のよう
        // に ActionName("Delete") を設定する
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Role_VCED, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Admin_Devは消せない
            if (role.Name != "Admin_Dev")
            {
                // ユーザーがアサインされているロールも以下の一行で
                // 削除可能。内部で階層更新が行われているらしい。
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    // 削除に成功したら Role/Index にリダイレクト
                    return RedirectToAction("Index", "Role");
                }

                // result.Succeeded が false の場合 ModelSate にエ
                // ラー情報を追加しないとエラーメッセージが出ない。
                // Register.cshtml.cs のものをコピー
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                                             error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Admin_Devは削除できません");
            }

            // 削除に失敗した場合、削除画面を再描画
            var model = new RoleModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }
        
        // 以下は:
        // (1) UserWithRoles で登録済みユーザーの一覧と各ユーザーへの
        //     ロールのアサイン状況を表示し、
        // (2) Edit ボタンクリックで EditRoleAssignment に遷移し、当該
        //     ユーザーへのロールのアサインを編集して保存
        // ・・・を行うアクションメソッド。

        // GET: Role/UserWithRoles
        // ユーザー一覧と各ユーザーにアサインされているロールを表示
        // Model は UserWithRoleInfo クラス

        [Authorize(Roles = "Role_ADet, Admin_Dev")]
        public async Task<IActionResult> UserWithRoles()
        {
            var model = new List<UserWithRoleInfo>();

            // ToListAsync() を付与してここで DB からデータを取得して
            // DataReader を閉じておかないと、下の IsInRole メソッド
            // でエラーになるので注意
            var users = await _userManager.Users.
                        OrderBy(user => user.UserName).ToListAsync();
            var roles = await _roleManager.Roles.
                        OrderBy(role => role.Name).ToListAsync();

            foreach (IdentityUser user in users)
            {
                var info = new UserWithRoleInfo
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.Email
                    
                };

                foreach (IdentityRole role in roles)
                {
                    RoleInfo roleInfo = new RoleInfo();
                    roleInfo.RoleName = role.Name;
                    roleInfo.IsInThisRole = await _userManager.
                                            IsInRoleAsync(user, role.Name);
                    info.UserRoles.Add(roleInfo);
                }
                model.Add(info);
            }

            return View(model);
        }

        // GET: Role/EditRoleAssignment/Id
        // 指定 Id のユーザーのロールへのアサインの編集
        // Model は UserWithRoleInfo クラス
        //[Authorize(Roles = "Role_Assign")]
        [Authorize(Roles = "Role_ADet, Admin_Dev")]
        public async Task<IActionResult> EditRoleAssignment(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserWithRoleInfo
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email
            };

            // ToListAsync() を付与しておかないと下の IsInRole メソッド
            // で　DataReader が閉じてないというエラーになる
            var roles = await _roleManager.Roles.
                          OrderBy(role => role.Name).ToListAsync();

            foreach (IdentityRole role in roles)
            {
                RoleInfo roleInfo = new RoleInfo();
                roleInfo.RoleName = role.Name;
                roleInfo.IsInThisRole = await _userManager.
                                        IsInRoleAsync(user, role.Name);
                model.UserRoles.Add(roleInfo);
            }

            return View(model);
        }

        // POST: Role/EditRoleAssignment/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Role_Assign")]
        [Authorize(Roles = "Role_ADet, Admin_Dev")]
        public async Task<IActionResult> EditRoleAssignment(string id,
          [Bind("UserId,UserName,UserEmail,UserRoles")] UserWithRoleInfo model)
        {
            if (id == null)
            {
                return NotFound();
            }

            // IsInRoleAsync, AddToRoleAsync, RemoveFromRoleAsync メソッド
            // の引数が MVC5 とは異なり、id ではなく MySQLIdentityUser が
            // 必要なのでここで取得しておく
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //adminは変更出来ない
            if (user.UserName == "admin@example.com")
            {
                ModelState.AddModelError("user.UserName", "管理者は変更できません");
                // 編集に失敗した場合、編集画面を再描画
                return View(model);
            }

            if (ModelState.IsValid)
            {
                IdentityResult result;

                foreach (RoleInfo roleInfo in model.UserRoles)
                {
                    // id のユーザーが roleInfo.RoleName のロールに属して
                    // いるか否か。以下でその情報が必要。
                    bool isInRole = await _userManager.
                                    IsInRoleAsync(user, roleInfo.RoleName);

                    // roleInfo.IsInThisRole には編集画面でロールのチェッ
                    // クボックスのチェック結果が格納されている
                    if (roleInfo.IsInThisRole)
                    {
                        // チェックが入っていた場合

                        // 既にロールにアサイン済みのユーザーを AddToRole
                        // するとエラーになるので以下の判定が必要
                        if (isInRole == false)
                        {
                            result = await _userManager.AddToRoleAsync(user,
                                                              roleInfo.RoleName);
                            if (!result.Succeeded)
                            {
                                // result.Succeeded が false の場合 ModelSate にエ
                                // ラー情報を追加しないとエラーメッセージが出ない。
                                // Register.cshtml.cs のものをコピー
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty,
                                                             error.Description);
                                }
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        // チェックが入っていなかった場合

                        // ロールにアサインされてないユーザーを
                        // RemoveFromRole するとエラーになるので以下の
                        // 判定が必要
                        if (isInRole == true)
                        {
                            result = await _userManager.
                                     RemoveFromRoleAsync(user, roleInfo.RoleName);
                            if (!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty,
                                                             error.Description);
                                }
                                return View(model);
                            }
                        }
                    }
                }

                // 編集に成功したら Role/UserWithRoles にリダイレクト
                return RedirectToAction("UserWithRoles", "Role");
            }

            // 編集に失敗した場合、編集画面を再描画
            return View(model);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OFAMA.Models;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using OFAMA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace OFAMA.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        //1.10追加
        //Get
        //[Authorize(Roles = "User_View, Admin_Dev")]
        public async Task<IActionResult> Index()
        {
            var model = new List<UserViewModel>();

            // ToListAsync() を付与してここで DB からデータを取得して
            // DataReader を閉じておかないと、下の IsInRole メソッド
            // でエラーになるので注意
            var users = await _userManager.Users.
                        OrderBy(user => user.UserName).ToListAsync();
            var roles = await _roleManager.Roles.
                        OrderBy(role => role.Name).ToListAsync();

            foreach (IdentityUser user in users)
            {
                //初期化
                int role_cnt = 0;
                var info = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    //Email = user.Email,
                    //EmailConfirmed = user.EmailConfirmed,
                };

                foreach (IdentityRole role in roles)
                {
                    RoleInfo roleInfo = new RoleInfo();
                    roleInfo.RoleName = role.Name;
                    roleInfo.IsInThisRole = await _userManager.
                                            IsInRoleAsync(user, role.Name);
                    //チェックの分だけカウントする
                    if (roleInfo.IsInThisRole)
                    {
                        role_cnt++;
                    }
                    info.UserRoles.Add(roleInfo);
                }
                info.UserRoleSum = role_cnt;

                model.Add(info);
            }

            return View(model);

            
        }

        //Get:Users/Details/5
        //もともとはDetails(int ? id)だった
        //[Authorize(Roles = "Role_View")]
        //[Authorize(Roles = "Role_ADet, Admin_Dev")]

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null )
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new List<UserViewModel>();

            var users = await _userManager.Users.
                        OrderBy(user => user.UserName).ToListAsync();
            var roles = await _roleManager.Roles.
                        OrderBy(role => role.Name).ToListAsync();

            var info = new UserViewModel
            {
                Id = user.Id,

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

            /*
            var model =new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                
            };
            */
            return View(model);
        }
        /*
        //POST:Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Password,ConfirmPassWord")]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);
        }
        */
        //[Authorize(Roles = "User_PasswordChange")]
        //[Authorize(Roles = "Password_Reset, Admin_Dev")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _userManager.FindByIdAsync(id);

            if (target == null)
            {
                return NotFound();
            }

            EditViewModel model = new EditViewModel()
            {
                UserName = target.UserName
            };

            return View(model);
        }
        

        // POST: User/Edit/5
        // UserName をソルトに使っていてパスワードだけもしくは
        // UserName だけを更新するのは NG かと思っていたが問題
        // なかった。（実際どのように対処しているかは不明）
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "User_PasswordChange")]
        //[Authorize(Roles = "Password_Reset, Admin_Dev")]
        public async Task<IActionResult> Edit(
            string id,
            string Password)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var target = await _userManager.FindByIdAsync(id);
                if (target == null)
                {
                    return NotFound();
                }
                //target.UserName = model.UserName;
                //target.Email = model.Email;




                // 新パスワードを入力した場合はパスワードも更新する
                if (!string.IsNullOrEmpty(Password))
                {
                    // MVC5 と違って PasswordValidator プロパティはない
                    // PasswordValidators で IList<IPasswordValidator<TUser>>
                    // を取得できる。PasswordValidators[0] で検証可能
                    // （ホントにそれで良いのかどうかは分からないが）
                    // ValidateAsync メソッドの引数は MVC5 と違うので注意
                    var resultPassword = await _userManager.PasswordValidators[0].
                        ValidateAsync(_userManager, target, Password);

                    if (resultPassword.Succeeded)
                    {
                        // 検証 OK の場合、入力パスワードをハッシュ。
                        // HashPassword メソッドの引数は MVC5 とは異なる
                        var hashedPassword = _userManager.PasswordHasher.
                            HashPassword(target, Password);
                        target.PasswordHash = hashedPassword;
                    }
                    else
                    {
                        // 検証 NG の場合 ModelSate にエラー情報を
                        // 追加して編集画面を再描画
                        // Register.cshtml.cs のものをコピー
                        foreach (var error in resultPassword.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View();
                    }
                }

                var resultUpdate = await _userManager.UpdateAsync(target);

                if (resultUpdate.Succeeded)
                {
                    // 更新に成功したら User/Index にリダイレクト
                    return RedirectToAction("Index", "Users");
                }
                // Register.cshtml.cs のものをコピー
                foreach (var error in resultUpdate.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var errorMessage in error.Errors)
                    {
                        Console.WriteLine(errorMessage.ErrorMessage);
                    }
                }
            }


                // 更新に失敗した場合、編集画面を再描画
                return View();
        }

        // GET: User/Delete/5
        // Model は UserModel
        // 階層更新が行われているようでロールがアサインされている
        // ユーザーも削除可
        //[Authorize(Roles = "User_Delete")]
        //[Authorize(Roles = "User_Delete, Admin_Dev")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var u = await _userManager.FindByIdAsync(id);

            if (u == null)
            {
                return NotFound();
            }

            var model = new UserModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                
            };

            return View(model);
        }

        // POST: User/Delete/5
        // 上の Delete(string id) と同シグネチャのメソッドは
        // 定義できないので、メソッド名を変えて、下のように
        // ActionName("Delete") を設定する
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "User_Delete")]
        //[Authorize(Roles = "User_Delete, Admin_Dev")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _userManager.FindByIdAsync(id);

            if (target == null)
            {
                return NotFound();
            }

            /*
            System.Console.WriteLine(id);
            //ここから削除できるかの判定
            var error_flug = false;
            // EquipManagerにこの備品を用いている人がいないかを判定
            // 存在しなければ nullを返す
            var use_userid_equipdata = await _context.EquipmentManager
                .FirstOrDefaultAsync(m => m.UserId == id);
            
            if(use_userid_equipdata != null)
            {
                //エラーメッセージを吐く
                ModelState.AddModelError("use_equipid_data", "このユーザは備品管理で用いられています。");
                error_flug = true;
            }
            /*
            // MerchandiseManagerにこの備品を用いている人がいないかを判定
            // 存在しなければ nullを返す
            var use_userid_merchdata = await _context.MerchandiseManager
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (use_userid_merchdata != null)
            {
                //エラーメッセージを吐く
                ModelState.AddModelError("use_userid_merchdata", "このユーザは商品管理で用いられています。");
                error_flug = true;
            }

            // Financesにこの備品を用いている人がいないかを判定
            // 存在しなければ nullを返す
            var use_userid_financedata = await _context.Finance
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (use_userid_financedata != null)
            {
                //エラーメッセージを吐く
                ModelState.AddModelError("use_userid_financedata", "このユーザは会計管理で用いられています。");
                error_flug = true;
            }

            // Borrowにこの備品を用いている人がいないかを判定
            // 存在しなければ nullを返す
            var use_userid_borrowdata = await _context.Borrow
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (use_userid_borrowdata != null)
            {
                //エラーメッセージを吐く
                ModelState.AddModelError("use_userid_borrowdata", "このユーザは立替管理で用いられています。");
                error_flug = true;
            }
            

            //エラーがある場合は削除せずに元の画面に戻す
            if (error_flug)
            {
                return View(target);
            }
            */

            // ロールがアサインされているユーザーも以下の一行
            // で削除可能。内部で階層更新が行われているらしい。
            var result = await _userManager.DeleteAsync(target);

            if (result.Succeeded)
            {
                // 削除に成功したら User/Index にリダイレクト
                return RedirectToAction("Index", "Users");
            }

            // Register.cshtml.cs のものをコピー
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(target);
        }
        /*
        public async Task<IActionResult>Edit(string id)
        {
            if (id == null) {
            return NotFound();
            }
            var target = await _context.ApplicationUser.FindAsync(id);

            if (target == null)
            {
                return NotFound();
            }
            UserEdit model = new UserEdit() { Email = target.Email };
            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(string id, UserEdit model)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var target = await _context.ApplicationUser.FindAsync(id);
                target.Email=model.Email;
                target.UserName=model.UserName;
                target.Status=model.Status;
                target.Authority=model.Authority;
                var resultUpdate =_context.Update(target);
                
            }
            return View(model);
        }
        */

        /*
        //Get: Users/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id ==null || _context.ApplicationUser == null)
            {
                return NotFound();
            }
            var applicationuser = await _context.ApplicationUser.FindAsync(id);
            if(applicationuser == null)
            {
                return NotFound();
            }
            return View(applicationuser);
        }
        //Post:Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Status,Authority")]ApplicationUser applicationuser)
        {

            if (string.Compare(id,applicationuser.Id,true)!=0)
            {
                
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("kokomadeOK");
                Console.WriteLine(id);
                Console.WriteLine(applicationuser.Id);
                var saved = false;
                try
                {

                    //applicationuser.Version = Guid.NewGuid();
                    _context.Update(applicationuser);

                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    foreach(var entry in ex.Entries)
                    {
                        if(entry.Entity is ApplicationUser)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();
                            Console.WriteLine("kokomadeOK");


                            foreach(var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = proposedValues[property];

                                proposedValues[property] = databaseValue;
                            }
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for");
                        }
                    }
                    if(!ApplicationUserExists(applicationuser.Id))
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
            return View(applicationuser);
        }
        

        //Get:Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if(id == null || _context.ApplicationUser == null)
            {
                return NotFound();
            }
            var applictationuser = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applictationuser == null)
            {
                return NotFound();
            }
            return View(applictationuser);
        }

        //POST:Users/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelteConfirmed(string id)
        {
            if (_context.ApplicationUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationUsers' is null");
            }
            var applicationuser = await _context.ApplicationUser.FindAsync(id);
            if(applicationuser !=null)
            {
                _context.ApplicationUser.Remove(applicationuser);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.ApplicationUser?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        */
    }
}


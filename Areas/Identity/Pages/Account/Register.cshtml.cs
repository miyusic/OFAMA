// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using java.lang;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OFAMA.Data;
using OFAMA.Models;

namespace OFAMA.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.IUserStore<IdentityUser> _userStore;
        private readonly Microsoft.AspNetCore.Identity.IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        //private readonly KeywordModel _keyword;
        private readonly ApplicationDbContext _context;
        //private  RoleController _roleController;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;


        public RegisterModel(
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
            Microsoft.AspNetCore.Identity.IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager
            //RoleController roleController

            //KeywordModel keyword
            //0427追加(削除済み)

            )
        {
            _userManager = userManager;
            _userStore = userStore;
            //変更 11.25
            _emailStore = (Microsoft.AspNetCore.Identity.IUserEmailStore<IdentityUser>)GetEmailStore();
            //
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            //0427(削除済み)
            //_keyword = new KeywordModel { Id = 0, Keyword = "oto" };
            _context = context;
            //_roleController = roleController;
            _roleManager = roleManager;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [RegularExpression(@"[a-zA-Z0-9 -/:-@\[-\`\{-\~]+", ErrorMessage = "半角英数字記号のみで構成された名前を入力してください")]
            [DataType(DataType.Text)]
            [Display(Name = "ユーザー名")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "メールアドレス")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "{0}は {2} 文字以上 {1} 文字以下で入力してください", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "パスワード")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "確認用パスワード")]
            [Compare("Password", ErrorMessage = "パスワードと確認用パスワードが一致しません")]
            public string ConfirmPassword { get; set; }

            //0427追加
            
            [DataType(DataType.Text)]
            [Display(Name = "事前パスワード")]
            public string Keyword { get; set; }



        }
        //1011
        public async Task<IActionResult> RegisterRole(string id)
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
            var model = new UserWithRoleInfo
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email
            };

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

            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.IdentityResult result;


                foreach (RoleInfo roleInfo in model.UserRoles)
                {
                    Console.WriteLine(roleInfo.RoleName);
                    if (roleInfo.RoleName == "Keyword")
                    {
                        // id のユーザーが roleInfo.RoleName のロールに属して
                        // いるか否か。以下でその情報が必要。
                        bool isInRole = await _userManager.
                                        IsInRoleAsync(user, roleInfo.RoleName);
                        if (isInRole == false)
                        {
                            result = await _userManager.AddToRoleAsync(user, roleInfo.RoleName);
                        }

                    }
                    if (roleInfo.RoleName == "Role_Assign")
                    {
                        // id のユーザーが roleInfo.RoleName のロールに属して
                        // いるか否か。以下でその情報が必要。
                        bool isInRole = await _userManager.
                                        IsInRoleAsync(user, roleInfo.RoleName);
                        if (isInRole == false)
                        {
                            result = await _userManager.AddToRoleAsync(user, roleInfo.RoleName);
                        }

                    }
                }

                // 編集に成功したら Role/UserWithRoles にリダイレクト
                return RedirectToAction("UserWithRoles", "Role");
            }

            // 編集に失敗した場合、編集画面を再描画
            return Page();
        }




        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = CreateUser();
                var user = new IdentityUser 
                {
                    UserName = Input.UserName,
                    Email = Input.Email
                };

                //1011
                var isUniqueUserName = await _userManager.FindByEmailAsync(user.UserName) == null;
                if (!isUniqueUserName)
                {
                    ModelState.AddModelError(string.Empty, "この名前は既に使用されています");
                    return Page();
                }

                var isUniqueEmail = await _userManager.FindByEmailAsync(user.Email) == null;
                if (!isUniqueEmail)
                {
                    ModelState.AddModelError(string.Empty, "メールアドレスは既に登録済みです");
                    return Page();
                }


                var _keyword = _context.Keyword.Select(m => m.Keyword).FirstOrDefault();
                if (_keyword == null)
                {
                    ModelState.AddModelError(string.Empty, "事前パスワードが入力されていません");
                    return Page();
                }


                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                
                //作成したキーワードがtrueかどうかを判断するコード。okならif文の中へ
                if (_keyword.Equals(Input.Keyword))
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("ユーザがパスワードつきの新しいアカウントを作成しました");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        /*await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/
                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $" {callbackUrl}からアカウントの認証を行ってください");

                        //2024.10.05追加
                        await RegisterRole(user.Id);

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    //このループに入らない(クリック認証でもこのループを使わない)
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                else
                {

                    ModelState.AddModelError(string.Empty, "事前パスワードが間違っています");


                }

            }
            
            // If we got this far, something failed, redisplay form(ここにも来ない)
            return Page();
        }

        private Microsoft.AspNetCore.Identity.IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Microsoft.AspNetCore.Identity.IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private Microsoft.AspNetCore.Identity.IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (Microsoft.AspNetCore.Identity.IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

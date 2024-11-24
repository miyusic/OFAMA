using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace OFAMA.Models
{
    public class RoleModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(
          100,
          ErrorMessage = "{0} は {2} 文字以上",
          MinimumLength = 3)]
        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "半角英字のみで構成された名前を入力してください")]
        [Display(Name = "ロール名")]
        public string Name { get; set; }
    }
    // 各ユーザーへのロールのアサイン状況一覧表示と
    // ロールのアサイン・解除用の Model
    // UserWithRoles, EditRoleAssignment に使用
    public class UserWithRoleInfo
    {
        public UserWithRoleInfo()
        {
            UserRoles = new List<RoleInfo>();
        }
        [Key]
        public string UserId { set; get; }

        [Display(Name = "ユーザー名")]
        public string UserName { set; get; }
        public string UserEmail { set; get; }

        [Display(Name = "付与ロール")]
        public IList<RoleInfo> UserRoles { set; get; }
    }

    public class RoleInfo
    {
        [Key]
        [Display(Name = "ロール名")]
        public string RoleName { set; get; }
        [Display(Name = "ロール付与")]
        public bool IsInThisRole { set; get; }
    }
}

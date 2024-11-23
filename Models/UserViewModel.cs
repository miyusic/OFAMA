using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }
        /*
        [EmailAddress]
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }

        [Display(Name = "メール確認済")]
        public bool EmailConfirmed { get; set; }
        */

        public UserViewModel()
        {
            UserRoles = new List<RoleInfo>();
        
        }
        [Display(Name = "ロール")]
        public IList<RoleInfo> UserRoles { set; get; }
        [Display(Name = "ロール数")]
        public int UserRoleSum { set; get; }



    }
    
}

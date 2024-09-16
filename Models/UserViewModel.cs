using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }
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
        public IList<RoleInfo> UserRoles { set; get; }

        public int UserRoleSum { set; get; }



    }
    
}

using javax.print.attribute.standard;
using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace OFAMA.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }
        [Display(Name = "メール確認済")]
        public bool EmailConfirmed { get; set; }
        
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0}は必須です。")]
        [EmailAddress]
        [Display(Name = "ユーザー名")]
        [Key]
        public string UserName { get;  set; }
        
        [EmailAddress]
        [Display(Name = "メールアドレス")]
 
        public string Email { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(100, ErrorMessage =
            "{0}は{2}から{1}文字の範囲で設定してください。",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "パスワード確認")]
        [Compare("Password", ErrorMessage = "確認パスワードが一致しません。")]
        public string ConfirmPassword { get; set; }

        //0427
        [Display(Name = "事前パスワード")]
        public string Keyword { get; set; }
        

    }







    public class EditViewModel
    {
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "ユーザー名")]
        [Key]
        public string UserName { get; set; }
        
        [EmailAddress]
        [Display(Name = "メールアドレス")]
        
        public string Email { get; set; }

        [StringLength(100, ErrorMessage =
            "{0}は{2}から{1}文字の範囲で設定してください。",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "パスワード確認")]
        [Compare("Password", ErrorMessage = "確認パスワードが一致しません。")]
        public string? ConfirmPassword { get; set; }
        
    }
}

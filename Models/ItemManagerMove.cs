using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OFAMA.Models
{
    [UserAmountValidation]
    public class ItemManagerMove
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "1つ目の移動先ユーザ名は必須項目です")]
        [DisplayName("移動先ユーザID1")]
        public String UserId1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "数量は1以上である必要があります")]
        [Required(ErrorMessage = "1つ目の数量は必須項目です")]
        [DisplayName("数量")]
        public int Amount1 { get; set; }

        [DisplayName("移動先ユーザID2")]
        public String? UserId2 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "数量は1以上である必要があります")]
        [DisplayName("数量")]
        public int? Amount2 { get; set; }

        [DisplayName("移動先ユーザID3")]
        public String? UserId3 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "数量は1以上である必要があります")]
        [DisplayName("数量")]
        public int? Amount3 { get; set; }
    }

    public class UserAmountValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ItemManagerMove)validationContext.ObjectInstance;

            // UserId1 と Amount1 の検証 (両方とも有効な値である必要があります)
            if (string.IsNullOrEmpty(model.UserId1) || model.Amount1 <= 0)
            {
                return new ValidationResult("UserId1 と Amount1 は両方とも有効な値でなければなりません。");
            }

            // UserId2 と Amount2 の組み合わせの検証 (どちらか一方が指定されていれば両方とも必要)
            if ((!string.IsNullOrEmpty(model.UserId2) && (model.Amount2 <= 0 || model.Amount2 == null)) ||
                (string.IsNullOrEmpty(model.UserId2) && model.Amount2 > 0))
            {
                return new ValidationResult("UserId2 と Amount2 は両方とも有効な値でなければなりません。");
            }

            // UserId3 と Amount3 の組み合わせの検証 (どちらか一方が指定されていれば両方とも必要)
            if ((!string.IsNullOrEmpty(model.UserId3) && (model.Amount3 <= 0 || model.Amount3 == null)) ||
                (string.IsNullOrEmpty(model.UserId3) && model.Amount3 > 0))
            {
                return new ValidationResult("UserId3 と Amount3 は両方とも有効な値でなければなりません。");
            }

            return ValidationResult.Success;
        }
    }
}

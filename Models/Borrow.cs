using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Borrow
    {
        public int Id { get; set; } // ID（データベースで自動生成されるため、自動増分するID）
        [DisplayName("返却状況")]
        public string Status { get; set; }
        [Required(ErrorMessage = "担当者名は必須項目です")]
        [DisplayName("担当者名")]
        public string UserId { get; set; } // ユーザID
        [Range(1, 1000000, ErrorMessage = "金額は1円以上1,000,000円以下である必要があります。")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "金額は数字のみ入力してください。")]
        [Required(ErrorMessage = "金額は必須項目です")]
        [DisplayName("金額(円)")]
        public int BorrowMoney { get; set; } // 金額
        [Required(ErrorMessage = "用途は必須項目です")]
        [StringLength(20, ErrorMessage = "用途は20文字以内である必要があります")]
        [DisplayName("用途")]
        public string Usage { get; set; } // 用途
        
        [Required(ErrorMessage = "立て替え日は必須項目です")]
        [DisplayName("立て替え日")]
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; }
        [DisplayName("返却日")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; } // 返却日時（nullの場合も考慮）
        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

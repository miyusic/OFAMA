using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Borrow
    {
        public int Id { get; set; } // ID（データベースで自動生成されるため、自動増分するID）
        [DisplayName("返却状況")]
        public string Status { get; set; }
        [Required(ErrorMessage = "担当者を入力してください。")]
        [DisplayName("担当者")]
        public string UserId { get; set; } // ユーザID
        [Required(ErrorMessage = "金額を入力してください。")]
        [DisplayName("金額")]
        public int BorrowMoney { get; set; } // 金額
        [Required(ErrorMessage = "用途を入力してください。")]
        [DisplayName("用途")]
        public string Usage { get; set; } // 用途
        [Required(ErrorMessage = "立て替え日時を入力してください。")]
        [DisplayName("立て替え日時")]
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; }
        [DisplayName("返却日時")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; } // 返却日時（nullの場合も考慮）
        [DisplayName("更新日")]
        public DateTime Updated_at { get; set; }
    }
}


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Finance
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "担当者を入力してください。")]
        [DisplayName("担当者")]
        public string UserId { get; set; }
        //担当者
        [Required(ErrorMessage = "金額を入力してください。")]
        [DisplayName("金額")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "金額は数字のみ入力してください。")]
        public int Money { get; set; }
        //金額
        [Required(ErrorMessage = "入金・出金を入力してください。")]
        [DisplayName("入金・出金")]
        public string Received { get; set; }
        //入出金
        [Required(ErrorMessage = "機関を入力してください。")]
        [DisplayName("機関名")]
        public int InstiId { get; set; }

        //機関
        [Required(ErrorMessage = "用途を入力してください。")]
        [DisplayName("用途")]
        public string Way { get; set; }
        //用途
        [Required(ErrorMessage = "登録日を入力してください。")]
        [DisplayName("登録日")]
        [DataType(DataType.Date)]
        public DateTime? Created_at { get; set; }

        [DisplayName("更新日")]
        public DateTime Updated_at { get; set; }
    }
}

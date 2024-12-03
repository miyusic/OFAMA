
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Finance
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "担当者名は必須項目です")]
        [DisplayName("担当者名")]
        public string UserId { get; set; }
        //担当者
        [Required(ErrorMessage = "金額は必須項目です")]
        [DisplayName("金額(円)")]
        [Range(0, 1000000, ErrorMessage = "金額は0円以上1,000,000円以下である必要があります。")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "金額は数字のみ入力してください。")]
        public int Money { get; set; }
        //金額
        [Required(ErrorMessage = "入金・出金は必須項目です")]
        [DisplayName("入金・出金")]
        public string Received { get; set; }
        //入出金
        [Required(ErrorMessage = "機関名は必須項目です")]
        [DisplayName("機関名")]
        public int InstiId { get; set; }

        //機関
        [Required(ErrorMessage = "用途は必須項目です")]
        [DisplayName("用途")]
        public string Way { get; set; }
        //用途
        [Required(ErrorMessage = "登録日は必須項目です")]
        [DisplayName("登録日")]
        [DataType(DataType.Date)]
        public DateTime? Created_at { get; set; }

        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Merchandise
    {
        //商品Id
        [Required(ErrorMessage = "商品IDは必須項目です")]
        [DisplayName("商品ID")]
        public int Id { get; set; }

        //商品名
        [StringLength(20, ErrorMessage = "商品名は20文字以内である必要があります")]
        [Required(ErrorMessage = "商品名は必須項目です")]
        [DisplayName("商品名")]
        public string ItemName { get; set; }

        //金額
        [Range(0, 1000000, ErrorMessage = "金額は0円以上1,000,000円以下である必要があります。")]
        [Required(ErrorMessage = "金額は必須項目です")]
        [DisplayName("金額(円)")]
        public int Price { get; set; }

        //種別のプルダウンメニュー用(CDとかキーホルダーとか)
        [StringLength(20, ErrorMessage = "商品種別は20文字以内である必要があります")]
        [Required(ErrorMessage = "商品種別は必須項目です")]
        [DisplayName("商品種別")]
        public String? Kind { get; set; }

        //製造日
        [Required(ErrorMessage = "制作年度は必須項目です")]
        [DisplayName("制作年度")]
        [DataType(DataType.Date)]
        public DateTime Created_yaer { get; set; }

        [Required(ErrorMessage = "登録日は必須項目です")]
        [DataType(DataType.Date)]
        [DisplayName("登録日")]
        public DateTime Created_at { get; set; }

        [Required(ErrorMessage = "最終更新日時は必須項目です")]
        [DisplayName("最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime Updated_at { get; set; }
    }
}

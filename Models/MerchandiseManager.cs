using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class MerchandiseManager
    {
        [Required(ErrorMessage = "データIDは必須項目です")]
        [DisplayName("データID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "商品名は必須項目です")]
        [DisplayName("商品名")]
        public int MerchId { get; set; }

        [Required(ErrorMessage = "担当者名は必須項目です")]
        [DisplayName("担当者名")]
        public String UserId { get; set; }

        [Range(1, 10000, ErrorMessage = "数量は1以上10,000以下である必要があります")]
        [Required(ErrorMessage = "数量は必須項目です")]
        [DisplayName("数量")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "実施日は必須項目です")]
        [DisplayName("実施日")]
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }

        [Required(ErrorMessage = "最終更新日時は必須項目です")]
        [DisplayName("最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime Updated_at { get; set; }
    }
}

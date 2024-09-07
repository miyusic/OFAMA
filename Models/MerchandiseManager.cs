using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class MerchandiseManager
    {
        [Required(ErrorMessage = "データIDは必須項目です")]
        [DisplayName("データID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "商品IDは必須項目です")]
        [DisplayName("商品ID")]
        public int MerchId { get; set; }

        [Required(ErrorMessage = "ユーザIDは必須項目です")]
        [DisplayName("ユーザID")]
        public String UserId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "数量は0以上である必要があります")]
        [Required(ErrorMessage = "数量は必須項目です")]
        [DisplayName("数量")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "作成日時は必須項目です")]
        [DisplayName("作成日時")]
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }

        [Required(ErrorMessage = "最終更新日時は必須項目です")]
        [DisplayName("最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime Updated_at { get; set; }
    }
}

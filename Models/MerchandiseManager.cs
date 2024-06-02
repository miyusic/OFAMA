using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class MerchandiseManager
    {
        [DisplayName("データID")]
        public int Id { get; set; }

        [DisplayName("商品ID")]
        public int MerchId { get; set; }

        [DisplayName("ユーザID")]
        public String UserId { get; set; }

        [DisplayName("数量")]
        public int Amount { get; set; }

        [DisplayName("作成日")]
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }

        [DisplayName("更新日")]
        public DateTime Updated_at { get; set; }
    }
}

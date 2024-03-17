using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Merchandise
    {
        //商品Id
        [DisplayName("商品ID")]
        public int Id { get; set; }

        //商品名
        [DisplayName("商品名")]
        public string ItemName { get; set; }

        //金額
        [DisplayName("金額")]
        public int Price { get; set; }

        //種別のプルダウンメニュー用(CDとかキーホルダーとか)
        [DisplayName("種別")]
        public String? Kind { get; set; }

        //製造日
        [DisplayName("制作年")]
        [DataType(DataType.Date)]
        public DateTime Created_yaer { get; set; }
    }
}

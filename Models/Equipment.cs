using System.ComponentModel;

namespace OFAMA.Models
{
    public class Equipment
    {
        //備品Id
        [DisplayName("備品ID")]
        public int Id { get; set; }

        //備品名
        [DisplayName("備品名")]
        public string ItemName { get; set; }

        //種別のプルダウンメニュー用(消耗品か消耗品でない)
        [DisplayName("消耗品")]
        public Boolean isExpandable { get; set; }

        [DisplayName("作成日時")]
        public DateTime Created_at { get; set; }

        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

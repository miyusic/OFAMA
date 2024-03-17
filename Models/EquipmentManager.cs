using System.ComponentModel;

namespace OFAMA.Models
{
    public class EquipmentManager
    {
        [DisplayName("データID")]
        public int Id { get; set; }

        [DisplayName("備品ID")]
        public int EquipId { get; set; }

        [DisplayName("ユーザID")]
        public int UserId { get; set; }

        [DisplayName("数量")]
        public int Amount { get; set; }

        [DisplayName("作成日時")]
        public DateTime Created_at { get; set; }

        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

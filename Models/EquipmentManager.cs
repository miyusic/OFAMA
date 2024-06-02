using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class EquipmentManager
    {
        [DisplayName("データID")]
        public int Id { get; set; }

        [DisplayName("備品ID")]
        public int EquipId { get; set; }

        [DisplayName("ユーザID")]
        public String UserId { get; set; }

        [DisplayName("数量")]
        public int Amount { get; set; }

        [DisplayName("作成日時")]
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }

        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Equipment
    {
        //備品Id
        [Required(ErrorMessage = "備品IDは必須項目です")]
        [DisplayName("備品ID")]
        public int Id { get; set; }

        //備品名
        [Required(ErrorMessage = "備品名は必須項目です")]
        [StringLength(20, ErrorMessage = "備品名は20文字以内である必要があります")]
        [DisplayName("備品名")]
        public string ItemName { get; set; }

        //種別のプルダウンメニュー用(消耗品か消耗品でない)
        [Required(ErrorMessage = "備品種別は必須項目です")]
        [DisplayName("消耗品")]
        public Boolean IsExpandable { get; set; }

        [Required(ErrorMessage = "登録日は必須項目です")]
        [DisplayName("登録日")]
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }

        [Required(ErrorMessage = "最終更新日時は必須項目です")]
        [DataType(DataType.DateTime)]
        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }
}

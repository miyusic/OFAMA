using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Institution

    {
        public int Id { get; set; } // 主キー
        [Required(ErrorMessage = "機関名は必須項目です")]
        [DisplayName("機関名")]
        public string Name { get; set; } // 機関名
        [DisplayName("最終更新日時")]
        public DateTime Updated_at { get; set; }
    }

}
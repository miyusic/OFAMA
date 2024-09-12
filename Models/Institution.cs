using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class Institution

    {
        public int Id { get; set; } // 主キー
        [DisplayName("機関名")]
        public string Name { get; set; } // 機関名
        [DisplayName("更新日")]
        public DateTime Updated_at { get; set; }
    }

}
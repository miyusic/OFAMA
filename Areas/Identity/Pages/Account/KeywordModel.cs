using System.ComponentModel.DataAnnotations;

namespace OFAMA.Areas.Identity.Pages.Account
{
    public class KeywordModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="キーワード")]
        public string? Keyword { get; set; }

        [Display(Name = "最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime Updated_at { get; set; }
    }
}

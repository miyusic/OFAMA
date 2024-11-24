using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class KeywordModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="キーワード")]
        [RegularExpression(@"[a-zA-Z0-9 -/:-@\[-\`\{-\~]+", ErrorMessage = "半角英数字記号のみで構成された名前を入力してください")]
        [StringLength(
          100,
          ErrorMessage = "{0} は {2} 文字以上",
          MinimumLength = 3)]
        public string? Keyword { get; set; }

        [Display(Name = "最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime Updated_at { get; set; }
    }
}

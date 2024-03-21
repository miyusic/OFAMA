using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class MerchandiseManagerViewModel
    {
        public List<MerchandiseManager>? MerchManagers { get; set; }
        public SelectList? MerchKinds { get; set; }

        //検索文字列を格納(ユーザ名)
        public string? SearchNameString { get; set; }

        //検索文字列を格納(備品名)
        public string? SearchMerchString { get; set; }

        //作成日検索で使う開始日時を格納
        public DateTime? StartDate { get; set; }
        //作成日検索で使う終了日時を格納
        public DateTime? EndDate { get; set; }
    }
}

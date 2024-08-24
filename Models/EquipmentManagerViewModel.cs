using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class EquipmentManagerViewModel
    {
        //管理データ全てを格納
        public List<EquipmentManager>? EquipmentManagers { get; set; }

        //備品Id全てを格納
        public SelectList? EquipIds { get; set; }
        //public SelectList? Expandables { get; set; }
        //public string? EquipmentExpandable { get; set; }

        //検索文字列を格納(ユーザ名)
        public string? SearchNameString { get; set; }

        //検索文字列を格納(備品名)
        public string? SearchEquipString { get; set; }

        //作成日検索で使う開始日時を格納
        public DateTime? StartDate { get; set; }
        //作成日検索で使う終了日時を格納
        public DateTime? EndDate { get; set; }
    }

}

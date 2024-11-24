using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class BorrowViewModel
    {
        public List<Borrow>? Borrows { get; set; }

        public SelectList? Statuses { get; set; }

        public string? BorrowStatus { get; set; }

        public string? SearchString { get; set; }

        public string? SearchNameString { get; set; }
        //立て替え日検索で使う日時を格納
        public DateTime? BorrowDate { get; set; }
        //返却日検索で使う日時を格納
        public DateTime? ReturnDate { get; set; }

    }
}
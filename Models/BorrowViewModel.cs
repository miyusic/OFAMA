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


    }
}
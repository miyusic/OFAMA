using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class MerchandiseManagerCreateViewModel
    {
        public Merchandise Merchandise { get; set; }
        public SelectList? Kinds { get; set; }

        public string? MerchKind { get; set; }
    }
}

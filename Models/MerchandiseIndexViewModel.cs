using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class MerchandiseIndexViewModel
    {
        public List<Merchandise>? Merchandises { get; set; }
        public SelectList? Kinds { get; set; }
        public SelectList? Years { get; set; }
        public string? MerchKind { get; set; }
        public string? MerchYear { get; set; }
        public string? SearchString { get; set; }
    }
}

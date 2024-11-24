using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Principal;

namespace OFAMA.Models
{
    public class FinanceViewModel
    {
        public List<Finance>? Finances { get; set; }
        public SelectList? Institutions { get; set; }
        public SelectList? Receiveds { get; set; }

        public string? FinanceInstitution { get; set; }
        public string? FinanceReceived { get; set; }
        public string? SearchString { get; set; }
        public string? SearchNameString { get; set; }
        public int? Financemoneytotal { get; set; }
        public int? WayTotalAmounts { get; set; }
        public Dictionary<string, decimal>? InstitutionTotalAmounts { get; set; }
        //作成日検索で使う開始日時を格納
        public DateTime? StartDate { get; set; }
        //作成日検索で使う終了日時を格納
        public DateTime? EndDate { get; set; }
    }
}
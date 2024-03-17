using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class EquipmentManagerViewModel
    {
        public List<EquipmentManager>? EquipmentManagers { get; set; }

        //public List<String>? EquipNames { get; set; }
        //public SelectList? Expandables { get; set; }
        //public string? EquipmentExpandable { get; set; }
        public string? SearchString { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

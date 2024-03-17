using Microsoft.AspNetCore.Mvc.Rendering;

namespace OFAMA.Models
{
    public class EquipmentViewModel
    {
        public List<Equipment>? Equipments { get; set; }
        public SelectList? Expandables { get; set; }
        public string? EquipmentExpandable { get; set; }
        public string? SearchString { get; set; }

    }
}

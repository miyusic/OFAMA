using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OFAMA.Models
{
    public class UserEdit
    {
        [Display(Name="UserName")]
        public string? UserName { get; set; }
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [PersonalData]
        public string? Status { get; set; }
        [PersonalData]
        public string? Authority { get; set; }

    }
}

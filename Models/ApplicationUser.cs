using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace OFAMA.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Status { get; set; }
        [PersonalData]
        public string? Authority { get; set; }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OFAMA.Models;

namespace OFAMA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<OFAMA.Models.Equipment>? Equipment { get; set; }
        public DbSet<OFAMA.Models.Merchandise>? Merchandise { get; set; }
        public DbSet<OFAMA.Models.EquipmentManager>? EquipmentManager { get; set; }
        public DbSet<OFAMA.Models.MerchandiseManager>? MerchandiseManager { get; set; }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OFAMA.Areas.Identity.Pages.Account;

namespace OFAMA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<IdentityUser> identityUser { get; set; } = default!;
        public DbSet<OFAMA.Models.UserModel>? UserModel { get; set; }
        public DbSet<OFAMA.Models.RegisterViewModel>? RegisterViewModel { get; set; }
        public DbSet<OFAMA.Models.EditViewModel>? EditViewModel { get; set; }
        public DbSet<OFAMA.Models.RoleModel>? RoleModel { get; set; }
        public DbSet<OFAMA.Models.UserWithRoleInfo>? UserWithRoleInfo { get; set; }
        public DbSet<KeywordModel>? Keyword { get; set; }
    }
}
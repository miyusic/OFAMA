
using OFAMA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OFAMA.Areas.Identity.Pages.Account;
using Microsoft.EntityFrameworkCore;

namespace OFAMA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // 備品管理と商品管理
        public DbSet<OFAMA.Models.Equipment>? Equipment { get; set; }
        public DbSet<OFAMA.Models.Merchandise>? Merchandise { get; set; }
        public DbSet<OFAMA.Models.EquipmentManager>? EquipmentManager { get; set; }
        public DbSet<OFAMA.Models.MerchandiseManager>? MerchandiseManager { get; set; }

        // ユーザ登録用のモデル
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
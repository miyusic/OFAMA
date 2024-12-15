using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OFAMA.Models;

namespace OFAMA.Data
{
    public class DbInitializer
    {
        /*
        public static async Task SeedingAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
            if (await context.Roles.AnyAsync())
                return;
            await context.RoleModel.AddRangeAsync(
                new RoleModel { Name = "TestRole" },
                new RoleModel { Name = "TestRole_two"}) ;
            await context.SaveChangesAsync();
        }
        */

        public static async Task InitializeRolesAndAdminUserAsync(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 必要なロールを登録
            string[] roles = { 
                "Admin_Dev",
                "EquipMng_View",
                "EquipMng_CEMD",
                "Equip_View",
                "Equip_CED",
                "MerchMng_View",
                "MerchMng_CEMD",
                "Merch_View",
                "Merch_CED",
                "Finance_View",
                "Finance_CED",
                "Institution_View",
                "Institution_CED",
                "Borrow_View",
                "Borrow_CED",
                "Borrow_Return",
                "User_View",
                "Password_Reset",
                "User_Delete",
                "Role_ADet",
                "Role_VCED",
                "Keyword_All"};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 管理者ユーザーを作成
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@1234";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin_Dev");
                }
            }
        }
    }
}

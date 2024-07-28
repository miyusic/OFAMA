using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using OFAMA.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
/*
namespace OFAMA.Models
{
    public class ApplicationRole:IdentityRole
    {
       
    }

    public class ApplicationRoleManager
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store):base(store)  { }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager>options,IOwinContext context)
        {
            //Dbcontext取得
            var dbContext = context.Get<ApplicationDbContext>();
            //ロールストア作成
            var roleStore = new RoleStore<ApplicationRole>(dbContext);
            //ロールマネージャー作成
            var manager = new ApplicationRoleManager(roleStore);
            if (!manager.Roles.Any())
            {
                manager.Create(new ApplicationRole
                {
                    Name = "Administrator"
                });
            }
            return manager;

        }
    }

}
*/
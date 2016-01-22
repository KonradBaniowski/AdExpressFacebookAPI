using Kantar.AdExpress.Service.DataAccess.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess
{
       public class ApplicationDbInit : CreateDatabaseIfNotExists<IdentityContext>
    {
        protected override void Seed(IdentityContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public void InitializeIdentityForEF(IdentityContext db)
        {
            // This is only for testing purpose
            const string name = "admin@admin.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";
            var applicationRoleManager = IdentityFactory.CreateRoleManager(db);
            var applicationUserManager = IdentityFactory.CreateUserManager(db);
            //Create Role Admin if it does not exist
            var role = applicationRoleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationIdentityRole { Name = roleName };
                applicationRoleManager.Create(role);
            }

            var user = applicationUserManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationIdentityUser { UserName = name, Email = name };
                applicationUserManager.Create(user, password);
                applicationUserManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = applicationUserManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                applicationUserManager.AddToRole(user.Id, role.Name);
            }
            //var context = new AspnetIdentityWithOnionContext("name=AppContext", new DebugLogger());
            var context = new IdentityContext();

            //var image = new Image { Path = "http://lorempixel.com/400/200/" };
            //context.Set<Image>().Add(image);
            //for (var i = 0; i < 100; i++)
            //{
            //    context.Set<Product>().Add(new Product { Name = "My Product", Description = "My Product", Image = image });
            //}
            context.SaveChanges();
        }
        //class DebugLogger : ILogger
        //{
        //    public void Log(string message)
        //    {

        //    }

        //    public void Log(Exception ex)
        //    {

        //    }
        //}
    }
}


using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using Kantar.AdExpress.Service.DataAccess.Extensions;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.IdentityImpl
{
    public class ApplicationRoleManager : IApplicationRoleManager
    {
        private readonly RoleManager<ApplicationIdentityRole, int> _roleManager;
        private bool _disposed;

        public ApplicationRoleManager(RoleManager<ApplicationIdentityRole, int> roleManager)
        {
            _roleManager = roleManager;
        }

        public virtual async Task<ApplicationIdentityResult> CreateAsync(ApplicationRole role)
        {
            var identityRole = role.ToIdentityRole();
            var identityResult = await _roleManager.CreateAsync(identityRole).ConfigureAwait(false);
            role.CopyIdentityRoleProperties(identityRole);
            return identityResult.ToApplicationIdentityResult();
        }

        public ApplicationIdentityResult Create(ApplicationRole role)
        {
            var identityRole = role.ToIdentityRole();
            var identityResult = _roleManager.Create(identityRole);
            role.CopyIdentityRoleProperties(identityRole);
            return identityResult.ToApplicationIdentityResult();
        }

        public virtual async Task<ApplicationIdentityResult> DeleteAsync(int roleId)
        {
            var identityRole = await _roleManager.FindByIdAsync(roleId);
            if (identityRole == null)
            {
                return new ApplicationIdentityResult(new[] { "Invalid role Id" }, false);
            }
            var identityResult = await _roleManager.DeleteAsync(identityRole).ConfigureAwait(false);
            return identityResult.ToApplicationIdentityResult();
        }

        public virtual async Task<ApplicationRole> FindByIdAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false);
            return role.ToApplicationRole();
        }
        public virtual ApplicationRole FindByName(string roleName)
        {
            var role = _roleManager.FindByName(roleName);
            return role.ToApplicationRole();
        }

        public virtual async Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            return role.ToApplicationRole();
        }

        public virtual IEnumerable<ApplicationRole> GetRoles()
        {
            return _roleManager.Roles.ToList().ToApplicationRoleList();
        }

        public virtual async Task<IEnumerable<ApplicationRole>> GetRolesAsync()
        {
            var applicationRoles = await _roleManager.Roles.ToListAsync().ConfigureAwait(false);
            return applicationRoles.ToApplicationRoleList();
        }

        public virtual async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
        }

        public virtual async Task<ApplicationIdentityResult> UpdateAsync(int roleId, string roleName)
        {
            var identityRole = await _roleManager.FindByIdAsync(roleId);
            if (identityRole == null)
            {
                return new ApplicationIdentityResult(new[] { "Invalid role Id" }, false);
            }
            identityRole.Name = roleName;
            var identityResult = await _roleManager.UpdateAsync(identityRole).ConfigureAwait(false);
            return identityResult.ToApplicationIdentityResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                }
            }
            _disposed = true;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Porygon.Identity.Data;

namespace Porygon.Identity.Stores
{
    public class RoleStore : IRoleStore<IdentityRole>
    {
        private readonly IRoleDataManager RoleDataManager;

        public RoleStore(IRoleDataManager roleDataManager)
        {
            RoleDataManager = roleDataManager;
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (await RoleDataManager.InsertAsync(role) > 0)
                return IdentityResult.Success;

            return IdentityResult.Failed(new IdentityError { Description = $"Could not insert role {role.Name}." });
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (await RoleDataManager.DeleteAsync(role.Id) > 0)
                return IdentityResult.Success;

            return IdentityResult.Failed(new IdentityError { Description = $"Could not delete role {role.Name}." });
        }

        public void Dispose()
        {

        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (roleId == null) throw new ArgumentNullException(nameof(roleId));

            return await RoleDataManager.GetAsync(roleId);
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (normalizedRoleName == null) throw new ArgumentNullException(nameof(normalizedRoleName));

            return await RoleDataManager.GetByName(normalizedRoleName);
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (await RoleDataManager.UpdateAsync(role) > 0)
                return IdentityResult.Success;

            return IdentityResult.Failed(new IdentityError { Description = $"Could not update role {role.Name}." });
        }
    }
}

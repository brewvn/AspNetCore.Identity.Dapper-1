using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
	{
		Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);
		Task<bool> CreateAsync(TRole role, CancellationToken cancellationToken);
		Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken);
		Task<TRole> FindByIdAsync(TKey key);
		Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken);
		Task<bool> UpdateAsync(TRole role, CancellationToken cancellationToken);
		Task SetRoleNameAsync(TKey roleId, string roleName);
	}
}

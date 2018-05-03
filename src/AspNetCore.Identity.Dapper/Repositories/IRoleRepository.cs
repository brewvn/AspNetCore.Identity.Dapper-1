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

	public class RoleRepository<TRole, TKey, TUserRole, TRoleClaim> : IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
	{
		public RoleRepository()
		{
		}

		public Task<bool> CreateAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<TRole> FindByIdAsync(TKey key)
		{
			throw new NotImplementedException();
		}

		public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetRoleNameAsync(TKey roleId, string roleName)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}

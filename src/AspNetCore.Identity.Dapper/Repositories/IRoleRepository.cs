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
		Task AddClaimAsync<TRole, TKey>(TRole role, Claim claim, CancellationToken cancellationToken)
			where TRole : IdentityRole<TKey>
			where TKey : IEquatable<TKey>;
	}
}

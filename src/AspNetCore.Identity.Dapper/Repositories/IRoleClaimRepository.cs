using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IRoleClaimRepository<TRoleCliam, TKey> where TKey : IEquatable<TKey> where TRoleCliam : IdentityRoleClaim<TKey>, new()
	{
		Task AddClaimAsync(TKey roleId, Claim claim, CancellationToken cancellationToken);
		Task<IList<Claim>> GetClaimsAsync(TKey roleId);
		Task RemoveClaimAsync(TKey id, Claim claim, CancellationToken cancellationToken);
	}
}

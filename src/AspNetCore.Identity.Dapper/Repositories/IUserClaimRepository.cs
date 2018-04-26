using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IUserClaimRepository<TUserClaim, TKey> where TKey : IEquatable<TKey> where TUserClaim : IdentityUserClaim<TKey>, new()
	{
		Task<IList<Claim>> GetClaimsAsync(TKey userId);

		/// <summary>
		/// value & type 
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		IEnumerable<TUserClaim> GetUsersAsync(Claim claim);
		Task AddClaimsAsync(TKey userId, IEnumerable<Claim> claims, CancellationToken cancellationToken);
		Task RemoveClaimsAsync(TKey userId, IEnumerable<Claim> claims, CancellationToken cancellationToken);
		Task ReplaceClaimAsync(TKey userId, Claim claim, Claim newClaim, CancellationToken cancellationToken);
	}
}

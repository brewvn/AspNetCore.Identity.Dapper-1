using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
		where TUserClaim : IdentityUserClaim<TKey>
		where TUserLogin : IdentityUserLogin<TKey>
		where TRole : IdentityRole<TKey>
	{
		Task<TKey> CreateAsync(TUser user, CancellationToken cancellationToken);
		Task DeleteAsync(TKey id, CancellationToken cancellationToken);
		Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
		Task<TUser> FindByIdAsync(TKey id, CancellationToken cancellationToken);
		Task<TUser> FindByNameAsync(string normalizedUserName);

		Task<bool> UpdateAsync(TUser user, CancellationToken cancellationToken);
		Task<IList<TUser>> FindByIdsAsync(params TKey[] ids);
	}
}

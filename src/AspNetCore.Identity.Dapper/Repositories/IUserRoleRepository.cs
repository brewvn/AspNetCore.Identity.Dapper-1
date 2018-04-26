using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IUserRoleRepository<TUser, TUserRole, TKey> where TKey : IEquatable<TKey> where TUserRole : IdentityUserRole<TKey>, new() where TUser : IdentityUser<TKey>
	{
		Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId, CancellationToken cancellationToken);
		Task AddUserRoleAsync(TUserRole userRole, CancellationToken cancellationToken);
		Task<IList<string>> GetRolesAsync(TKey userId);
		Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);
		Task<bool> IsInRoleAsync(TKey userId, string normalizedRoleName);
		Task RemoveFromRoleAsync(TKey userId, string normalizedRoleName, CancellationToken cancellationToken);
	}
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Linq;
using System.Security.Claims;
using AspNetCore.Identity.Dapper.Repositories;
using System.Globalization;

namespace AspNetCore.Identity.Dapper
{
	public class UserStore : UserStore<IdentityUser, IdentityRole, string>
	{
		public UserStore(IdentityErrorDescriber describer, IConnectionProvider connectionProvider, SqlConfiguration sqlConfiguration) : base(describer, connectionProvider, sqlConfiguration) { }
	}

	public class UserStore<TUser> : UserStore<TUser, IdentityRole, string>
		 where TUser : IdentityUser<string>, new()
	{
		public UserStore(IdentityErrorDescriber describer, IConnectionProvider connectionProvider, SqlConfiguration sqlConfiguration) : base(describer, connectionProvider, sqlConfiguration) { }
	}

	public class UserStore<TUser, TRole, TKey> : UserStore<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>, IdentityRoleClaim<TKey>>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// Constructs a new instance of <see cref="UserStore{TUser, TRole, TContext, TKey}"/>.
		/// </summary>
		/// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
		public UserStore(IdentityErrorDescriber describer, IConnectionProvider connectionProvider, SqlConfiguration sqlConfiguration) : base(describer, connectionProvider, sqlConfiguration) { }
	}

	public class UserStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
		UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
		// TODO: IProtectedUserStore<TUser>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>, new()
		where TUserRole : IdentityUserRole<TKey>, new()
		where TUserLogin : IdentityUserLogin<TKey>, new()
		where TUserToken : IdentityUserToken<TKey>, new()
		where TRoleClaim : IdentityRoleClaim<TKey>, new()
	{
		private readonly IConnectionProvider _connectionProvider;
		private readonly SqlConfiguration _sqlConfiguration;

		public UserStore(IdentityErrorDescriber describer, IConnectionProvider connectionProvider, SqlConfiguration sqlConfiguration) : base(describer)
		{
			_connectionProvider = connectionProvider;
			_sqlConfiguration = sqlConfiguration;
		}

		public override IQueryable<TUser> Users => throw new NotImplementedException();

		public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (claims == null)
			{
				throw new ArgumentNullException(nameof(claims));
			}
			using (var conn = _connectionProvider.Create())
			{
				var results = new List<dynamic>();
				foreach (var claim in claims)
				{
					var userClaim = Activator.CreateInstance<TUserClaim>();
					userClaim.UserId = user.Id;
					userClaim.ClaimType = claim.Type;
					userClaim.ClaimValue = claim.Value;
					results.Add(userClaim);
				}
				await conn.ExecuteAsync(_sqlConfiguration.AddUserClaims, results);
			}
		}

		public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (login == null)
			{
				throw new ArgumentNullException(nameof(login));
			}
			var userLogin = new
			{
				UserId = user.Id,
				login.LoginProvider,
				login.ProviderKey,
				login.ProviderDisplayName
			};
			using (var conn = _connectionProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.AddLogin, userLogin);
			}
		}

		public override async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (string.IsNullOrWhiteSpace(normalizedRoleName))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
			}
			var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
			if (roleEntity == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist.", normalizedRoleName));
			}
			var userRole = CreateUserRole(user, roleEntity);
			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.AddUserRole, userRole);
			}
		}

		public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.CreateUser, user);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.DeleteUserById, user);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(normalizedEmail))
				throw new ArgumentNullException(nameof(normalizedEmail));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUser>(conn, _sqlConfiguration.FindUserByNormalizedEmail, new { NormalizedEmail = normalizedEmail });
			}
		}

		public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUser>(conn, _sqlConfiguration.FindUserById, new { Id = userId });
			}
		}

		public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(normalizedUserName))
				throw new ArgumentNullException(nameof(normalizedUserName));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUser>(conn, _sqlConfiguration.FindUserByNormalizedName, new { NormalizedName = normalizedUserName });
			}
		}

		public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _connectionProvider.Create())
			{
				var userClaims = await SqlMapper.QueryAsync(conn, _sqlConfiguration.FindUserClaimsByUserId, new { UserId = user.Id });
				var results = userClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
				return results;
			}
		}

		public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _connectionProvider.Create())
			{
				var userLogins = await SqlMapper.QueryAsync(conn, _sqlConfiguration.FindUserLoginsByUserId, new { UserId = user.Id });
				var results = userLogins.Select(y => new UserLoginInfo(y.LoginProvider, y.ProviderKey, y.Name)).ToList();
				return results;
			}
		}

		public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _connectionProvider.Create())
			{
				var roles = await SqlMapper.QueryAsync<string>(conn, _sqlConfiguration.FindUserRolesByUserId, new { UserId = user.Id });
				return roles.ToList();
			}
		}

		public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			using (var conn = _connectionProvider.Create())
			{
				var users = await SqlMapper.QueryAsync<TUser>(conn, _sqlConfiguration.FindUsersByClaim, new { ClaimValue = claim.Value, ClaimType = claim.Type });
				return users.ToList();
			}
		}

		public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			using (var conn = _connectionProvider.Create())
			{
				var users = await SqlMapper.QueryAsync<TUser>(conn, _sqlConfiguration.FindUsersByRoleName, new { RoleName = normalizedRoleName });
				return users.ToList();
			}
		}

		public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			using (var conn = _connectionProvider.Create())
			{
				var counts = await SqlMapper.QueryFirstOrDefaultAsync<int>(conn, _sqlConfiguration.CountUserRolesByUserId, new { RoleName = normalizedRoleName, UserId = user.Id });
				return counts > 0;
			}
		}

		public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claims == null)
				throw new ArgumentNullException(nameof(claims));

			using (var conn = _connectionProvider.Create())
			{
				var paramters = claims.Select(c => new { UserId = user.Id, ClaimType = c.Type, ClaimValue = c.Value });
				await conn.ExecuteTransactionAsync(_sqlConfiguration.RemoveUserClaimsByUserIdAndClaims, paramters, rows => rows == claims.Count());
			}
		}

		public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.RemoveUserRolesByUserIdAndNormalizedRoleName, new { RoleName = normalizedRoleName, UserId = user.Id });
			}
		}

		public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrEmpty(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.RemoveUserLogins, new
				{
					UserId = user.Id,
					LoginProvider = loginProvider,
					ProviderKey = providerKey
				});
			}
		}

		public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			if (newClaim == null)
				throw new ArgumentNullException(nameof(newClaim));

			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.ReplaceUserClaim, new
				{
					NewClaimType = newClaim.Type,
					NewClaimValue = newClaim.Value,
					UserId = user.Id,
					ClaimType = claim.Type,
					ClaimValue = claim.Value
				});
			}
		}

		public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.UpdateUser, user);
				return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Update user failed." });
			}
		}

		protected override async Task AddUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.AddUserToken, token);
			}
		}

		protected override async Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TRole>(conn, _sqlConfiguration.FindRoleByNormalizedName);
			}
		}

		protected override async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUserToken>(conn, _sqlConfiguration.FindUserTokenByUserIdAndLoginProviderAndName, new
				{
					UserId = user.Id,
					LoginProvider = loginProvider,
					Name = name
				});
			}
		}

		protected override async Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUser>(conn, _sqlConfiguration.FindUserById, new { Id = userId });
			}
		}

		protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUserLogin>(conn, _sqlConfiguration.FindUserLoginByUserIdAndLoginProviderAndProviderKey, new
				{
					UserId = userId,
					LoginProvider = loginProvider,
					ProviderKey = providerKey
				});
			}
		}

		protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUserLogin>(conn, _sqlConfiguration.FindUserLoginByLoginProviderAndProviderKey, new
				{
					LoginProvider = loginProvider,
					ProviderKey = providerKey
				});
			}
		}

		protected override async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			if (roleId == null)
				throw new ArgumentNullException(nameof(roleId));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstOrDefaultAsync<TUserRole>(conn, _sqlConfiguration.FindUserRoleByUserIdAndRoleId, new
				{
					UserId = userId,
					RoleId = roleId
				});
			}
		}

		protected override async Task RemoveUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			using (var conn = _connectionProvider.Create())
			{
				await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.RemoveUserTokenById, token);
			}
		}
	}
}

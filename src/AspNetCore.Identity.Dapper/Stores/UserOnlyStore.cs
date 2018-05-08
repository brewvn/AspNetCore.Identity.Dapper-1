using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Stores
{
	public class UserOnlyStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> :
		UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
		IUserLoginStore<TUser>,
		IUserClaimStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserEmailStore<TUser>,
		IUserLockoutStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IQueryableUserStore<TUser>,
		IUserTwoFactorStore<TUser>,
		IUserAuthenticationTokenStore<TUser>,
		IUserAuthenticatorKeyStore<TUser>,
		IUserTwoFactorRecoveryCodeStore<TUser>
		// TODO: IProtectedUserStore<TUser>
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>, new()
		where TUserLogin : IdentityUserLogin<TKey>, new()
		where TUserToken : IdentityUserToken<TKey>, new()
	{
		private readonly IStoreProvider _storeProvider;
		private readonly SqlConfiguration _sqlConfiguration;

		public UserOnlyStore(IdentityErrorDescriber describer, IStoreProvider connectionProvider) : base(describer)
		{
			_storeProvider = connectionProvider;
			_sqlConfiguration = connectionProvider.SqlConfiguration;
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
			using (var conn = _storeProvider.Create())
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
			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.AddLogin, userLogin);
			}
		}

		public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.CreateUser, user);
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
			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.DeleteUserById, user);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(normalizedEmail))
				throw new ArgumentNullException(nameof(normalizedEmail));

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUser>(_sqlConfiguration.FindUserByNormalizedEmail, new { NormalizedEmail = normalizedEmail });
			}
		}

		public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUser>(_sqlConfiguration.FindUserById, new { Id = userId });
			}
		}

		public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrEmpty(normalizedUserName))
				throw new ArgumentNullException(nameof(normalizedUserName));

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUser>(_sqlConfiguration.FindUserByNormalizedName, new { NormalizedUserName = normalizedUserName });
			}
		}

		public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _storeProvider.Create())
			{
				var userClaims = await conn.QueryAsync(_sqlConfiguration.FindUserClaimsByUserId, new { UserId = user.Id });
				var results = userClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
				return results;
			}
		}

		public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			using (var conn = _storeProvider.Create())
			{
				var userLogins = await conn.QueryAsync(_sqlConfiguration.FindUserLoginsByUserId, new { UserId = user.Id });
				var results = userLogins.Select(y => new UserLoginInfo(y.LoginProvider, y.ProviderKey, y.ProviderDisplayName)).ToList();
				return results;
			}
		}

		public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			using (var conn = _storeProvider.Create())
			{
				var users = await conn.QueryAsync<TUser>(_sqlConfiguration.FindUsersByClaim, new { ClaimValue = claim.Value, ClaimType = claim.Type });
				return users.ToList();
			}
		}

		public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claims == null)
				throw new ArgumentNullException(nameof(claims));

			using (var conn = _storeProvider.Create())
			{
				var paramters = claims.Select(c => new { UserId = user.Id, ClaimType = c.Type, ClaimValue = c.Value });
				await conn.ExecuteAsync(_sqlConfiguration.RemoveUserClaimsByUserIdAndClaims, paramters);
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

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.RemoveUserLogins, new
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

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.ReplaceUserClaim, new
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

			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.UpdateUser, user);
				return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Update user failed." });
			}
		}

		protected override async Task AddUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.AddUserToken, token);
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

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUserToken>(_sqlConfiguration.FindUserTokenByUserIdAndLoginProviderAndName, new
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

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUser>(_sqlConfiguration.FindUserById, new { Id = userId });
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

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUserLogin>(_sqlConfiguration.FindUserLoginByUserIdAndLoginProviderAndProviderKey, new
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

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TUserLogin>(_sqlConfiguration.FindUserLoginByLoginProviderAndProviderKey, new
				{
					LoginProvider = loginProvider,
					ProviderKey = providerKey
				});
			}
		}

		protected override async Task RemoveUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.RemoveUserTokenById, token);
			}
		}
	}
}

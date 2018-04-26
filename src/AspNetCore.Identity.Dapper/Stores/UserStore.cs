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
	public class UserStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
		UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
		//IProtectedUserStore<TUser>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>, new()
		where TUserRole : IdentityUserRole<TKey>, new()
		where TUserLogin : IdentityUserLogin<TKey>, new()
		where TUserToken : IdentityUserToken<TKey>, new()
		where TRoleClaim : IdentityRoleClaim<TKey>, new()
	{
		//private const string ParameterNotation = "@";
		//private const string SchemaName = "[dbo]";
		//private const string TableColumnStartNotation = "[";
		//private const string TableColumnEndNotation = "]";
		//private const string InsertRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
		//private const string DeleteRoleQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
		//private const string UpdateRoleQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
		//private const string SelectRoleByNameQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE [Name] = %NAME%";
		//private const string SelectRoleByIdQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
		//private const string InsertUserQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% OUTPUT INSERTED.Id VALUES(%VALUES%)";
		//private const string DeleteUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
		//private const string UpdateUserQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
		//private const string SelectUserByUserNameQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [UserName] = %USERNAME%";
		//private const string SelectUserByEmailQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [Email] = %EMAIL%";
		//private const string SelectUserByIdQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [Id] = %ID%";
		//private const string InsertUserClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
		//private const string InsertUserLoginQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
		//private const string InsertUserRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
		//private const string GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT TOP 1 %USERFILTER%, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id] INNER JOIN %SCHEMA%.%USERLOGINTABLE% ON %SCHEMA%.%USERTABLE%.[Id] = %SCHEMA%.%USERLOGINTABLE%.[UserId] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";
		//private const string GetClaimsByUserIdQuery = "SELECT [ClaimType], [ClaimValue] FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID%";
		//private const string GetRolesByUserIdQuery = "SELECT [Name] FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%USERROLETABLE% WHERE [UserId] = %ID% AND %SCHEMA%.%ROLETABLE%.[Id] = %SCHEMA%.%USERROLETABLE%.[RoleId]";
		//private const string GetUserLoginInfoByIdQuery = "SELECT [LoginProvider], [Name], [ProviderKey] FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID%";
		//private const string GetUsersByClaimQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERCLAIMTABLE% WHERE [ClaimValue] = %CLAIMVALUE% AND [ClaimType] = %CLAIMTYPE%";
		//private const string GetUsersInRoleQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.[Name] = %ROLENAME% AND %SCHEMA%.%USERROLETABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id] AND %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id]";
		//private const string IsInRoleQuery = "SELECT 1 FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.[Name] = %ROLENAME% AND %SCHEMA%.%USERTABLE%.[Id] = %USERID% AND %SCHEMA%.%USERROLETABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id] AND %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id]";
		//private const string RemoveClaimsQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
		//private const string RemoveUserFromRoleQuery = "DELETE FROM %SCHEMA%.%USERROLETABLE% WHERE [UserId] = %USERID% AND [RoleId] = (SELECT [Id] FROM %SCHEMA%.%ROLETABLE% WHERE [Name] = %ROLENAME%)";
		//private const string RemoveLoginForUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %USERID% AND [LoginProvider] = %LOGINPROVIDER% AND [ProviderKey] = %PROVIDERKEY%";
		//private const string UpdateClaimForUserQuery = "UPDATE %SCHEMA%.%TABLENAME% SET [ClaimType] = %NEWCLAIMTYPE%, [ClaimValue] = %NEWCLAIMVALUE% WHERE [UserId] = %USERID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
		//private const string SelectClaimByRoleQuery = "SELECT %SCHEMA%.%ROLECLAIMTABLE%.* FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%ROLECLAIMTABLE% WHERE [RoleId] = %ROLEID% AND %SCHEMA%.%ROLECLAIMTABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id]";
		//private const string InsertRoleClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
		//private const string DeleteRoleClaimQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [RoleId] = %ROLEID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
		//private const string RoleTable = "[IdentityRole]";
		//private const string UserTable = "[IdentityUser]";
		//private const string UserClaimTable = "[IdentityUserClaim]";
		//private const string UserRoleTable = "[IdentityUserRole]";
		//private const string UserLoginTable = "[IdentityLogin]";
		//private const string RoleClaimTable = "[IdentityRoleClaim]";

		private readonly IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> _userRepository;
		private readonly IUserClaimRepository<TUserClaim, TKey> _userClaimRepository;

		private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;
		private readonly IUserTokenRepository<TUserToken, TKey> _userTokenRepository;
		private readonly IUserLoginRepository<TUserLogin, TKey> _userLoginRepository;
		private readonly IUserRoleRepository<TUser, TUserRole, TKey> _userRoleRepository;

		public UserStore(IdentityErrorDescriber describer,
			IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> userRepository,
			IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepository,
			IUserTokenRepository<TUserToken, TKey> userTokenRepository,
			IUserLoginRepository<TUserLogin, TKey> userLoginRepository,
			IUserRoleRepository<TUser, TUserRole, TKey> userRoleRepository
			) : base(describer)
		{
			_userRepository = userRepository;
			_roleRepository = roleRepository;
			_userTokenRepository = userTokenRepository;
			_userLoginRepository = userLoginRepository;
			_userRoleRepository = userRoleRepository;
		}

		public override IQueryable<TUser> Users => throw new NotImplementedException();

		public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (claims == null)
			{
				throw new ArgumentNullException(nameof(claims));
			}

			await _userClaimRepository.AddClaimsAsync(user.Id, claims, cancellationToken);
		}

		public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (login == null)
			{
				throw new ArgumentNullException(nameof(login));
			}
			await _userLoginRepository.AddLoginAsync(user.Id, login, cancellationToken);
		}

		public override async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
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
			await _userRoleRepository.AddUserRoleAsync(userRole, cancellationToken);
		}

		public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var result = await _userRepository.CreateAsync(user, cancellationToken);

			if (!result.Equals(default(TKey)))
			{
				user.Id = result;
				return IdentityResult.Success;
			}
			else
			{
				return IdentityResult.Failed();
			}
		}

		public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}


			try
			{
				await _userRepository.DeleteAsync(user.Id, cancellationToken);
			}
			catch
			{
				return IdentityResult.Failed(new IdentityError { Description = "Remove user failed." });
			}
			return IdentityResult.Success;
		}

		public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(normalizedEmail))
				throw new ArgumentNullException(nameof(normalizedEmail));

			return await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
		}

		public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));

			return await _userRepository.FindByIdAsync((TKey)Convert.ChangeType(userId, typeof(TKey)), cancellationToken);
		}

		public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(normalizedUserName))
				throw new ArgumentNullException(nameof(normalizedUserName));

			return await _userRepository.FindByNameAsync(normalizedUserName);
		}

		public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			return await _userClaimRepository.GetClaimsAsync(user.Id);
		}

		public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			return await _userLoginRepository.GetLoginsAsync(user.Id);
		}

		public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			return await _userRoleRepository.GetRolesAsync(user.Id);
		}

		public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));
			IEnumerable<TUserClaim> userClaims = _userClaimRepository.GetUsersAsync(claim);
			var ids = userClaims.Select(uc => uc.UserId).ToArray();
			return await _userRepository.FindByIdsAsync(ids);
		}

		public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			return await _userRoleRepository.GetUsersInRoleAsync(normalizedRoleName, cancellationToken);
		}

		public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			return await _userRoleRepository.IsInRoleAsync(user.Id, normalizedRoleName);
		}

		public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claims == null)
				throw new ArgumentNullException(nameof(claims));

			await _userClaimRepository.RemoveClaimsAsync(user.Id, claims, cancellationToken);
		}

		public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			await _userRoleRepository.RemoveFromRoleAsync(user.Id, normalizedRoleName, cancellationToken);
		}

		public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrEmpty(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrEmpty(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			await _userLoginRepository.RemoveLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
		}

		public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			if (newClaim == null)
				throw new ArgumentNullException(nameof(newClaim));

			await _userClaimRepository.ReplaceClaimAsync(user.Id, claim, newClaim, cancellationToken);
		}

		public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var result = await _userRepository.UpdateAsync(user, cancellationToken);
			return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Update user failed." });
		}

		protected override async Task AddUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			await _userTokenRepository.AddUserTokenAsync(token);
		}

		protected override async Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));

			return await _roleRepository.FindRoleAsync(normalizedRoleName, cancellationToken);
		}

		protected override async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			return await _userTokenRepository.FindTokenAsync(user.Id, loginProvider, name, cancellationToken);
		}

		protected override async Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
		{
			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			return await _userRepository.FindByIdAsync(userId, cancellationToken);
		}

		protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			return await _userLoginRepository.FindUserLoginAsync(userId, loginProvider, providerKey, cancellationToken);
		}

		protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(loginProvider))
				throw new ArgumentNullException(nameof(loginProvider));

			if (string.IsNullOrWhiteSpace(providerKey))
				throw new ArgumentNullException(nameof(providerKey));

			return await _userLoginRepository.FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
		}

		protected override async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
		{
			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			if (roleId == null)
				throw new ArgumentNullException(nameof(roleId));

			return await _userRoleRepository.FindUserRoleAsync(userId, roleId, cancellationToken);
		}

		protected override async Task RemoveUserTokenAsync(TUserToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			await _userTokenRepository.RemoveUserTokenAsync(token.UserId);
		}
	}
}

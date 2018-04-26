//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Dapper;
//using AspNetCore.Identity.Dapper.Repositories;

//namespace AspNetCore.Identity.Dapper
//{
//	public class RoleStore<TRole, TKey, TUserRole, TRoleClaim> :
//	   IQueryableRoleStore<TRole>,
//	   IRoleClaimStore<TRole>
//	   where TRole : IdentityRole<TKey>
//	   where TKey : IEquatable<TKey>
//	   where TUserRole : IdentityUserRole<TKey>, new()
//	   where TRoleClaim : IdentityRoleClaim<TKey>, new()
//	{
//		private static string ParameterNotation = "@";
//		private static string SchemaName = "[dbo]";
//		private static string TableColumnStartNotation = "[";
//		private static string TableColumnEndNotation = "]";
//		private static string InsertRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
//		private static string DeleteRoleQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
//		private static string UpdateRoleQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
//		private static string SelectRoleByNameQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE [Name] = %NAME%";
//		private static string SelectRoleByIdQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
//		private static string InsertUserQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% OUTPUT INSERTED.Id VALUES(%VALUES%)";
//		private static string DeleteUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [Id] = %ID%";
//		private static string UpdateUserQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
//		private static string SelectUserByUserNameQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [UserName] = %USERNAME%";
//		private static string SelectUserByEmailQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [Email] = %EMAIL%";
//		private static string SelectUserByIdQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] =  %SCHEMA%.%USERTABLE%.[Id] WHERE [Id] = %ID%";
//		private static string InsertUserClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
//		private static string InsertUserLoginQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
//		private static string InsertUserRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
//		private static string GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT TOP 1 %USERFILTER%, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id] INNER JOIN %SCHEMA%.%USERLOGINTABLE% ON %SCHEMA%.%USERTABLE%.[Id] = %SCHEMA%.%USERLOGINTABLE%.[UserId] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";
//		private static string GetClaimsByUserIdQuery = "SELECT [ClaimType], [ClaimValue] FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID%";
//		private static string GetRolesByUserIdQuery = "SELECT [Name] FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%USERROLETABLE% WHERE [UserId] = %ID% AND %SCHEMA%.%ROLETABLE%.[Id] = %SCHEMA%.%USERROLETABLE%.[RoleId]";
//		private static string GetUserLoginInfoByIdQuery = "SELECT [LoginProvider], [Name], [ProviderKey] FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID%";
//		private static string GetUsersByClaimQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERCLAIMTABLE% WHERE [ClaimValue] = %CLAIMVALUE% AND [ClaimType] = %CLAIMTYPE%";
//		private static string GetUsersInRoleQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.[Name] = %ROLENAME% AND %SCHEMA%.%USERROLETABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id] AND %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id]";
//		private static string IsInRoleQuery = "SELECT 1 FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.[Name] = %ROLENAME% AND %SCHEMA%.%USERTABLE%.[Id] = %USERID% AND %SCHEMA%.%USERROLETABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id] AND %SCHEMA%.%USERROLETABLE%.[UserId] = %SCHEMA%.%USERTABLE%.[Id]";
//		private static string RemoveClaimsQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %ID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
//		private static string RemoveUserFromRoleQuery = "DELETE FROM %SCHEMA%.%USERROLETABLE% WHERE [UserId] = %USERID% AND [RoleId] = (SELECT [Id] FROM %SCHEMA%.%ROLETABLE% WHERE [Name] = %ROLENAME%)";
//		private static string RemoveLoginForUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [UserId] = %USERID% AND [LoginProvider] = %LOGINPROVIDER% AND [ProviderKey] = %PROVIDERKEY%";
//		private static string UpdateClaimForUserQuery = "UPDATE %SCHEMA%.%TABLENAME% SET [ClaimType] = %NEWCLAIMTYPE%, [ClaimValue] = %NEWCLAIMVALUE% WHERE [UserId] = %USERID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
//		private static string SelectClaimByRoleQuery = "SELECT %SCHEMA%.%ROLECLAIMTABLE%.* FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%ROLECLAIMTABLE% WHERE [RoleId] = %ROLEID% AND %SCHEMA%.%ROLECLAIMTABLE%.[RoleId] = %SCHEMA%.%ROLETABLE%.[Id]";
//		private static string InsertRoleClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
//		private static string DeleteRoleClaimQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE [RoleId] = %ROLEID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
//		private static string RoleTable = "[AspNetRoles]";
//		private static string UserTable = "[AspNetUsers]";
//		private static string UserClaimTable = "[AspNetUserClaims]";
//		private static string UserRoleTable = "[AspNetUserRoles]";
//		private static string UserLoginTable = "[AspNetUserLogins]";
//		private static string RoleClaimTable = "[AspNetRoleClaims]";

//		private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;

//		public RoleStore(IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepository,
//		  )
//		{
//			_roleRepository = roleRepository;
//		}

//		// Dapper 不可能实现这个
//		public IQueryable<TRole> Roles => throw new NotImplementedException();

//		public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			if (role == null)
//				throw new ArgumentNullException(nameof(role));

//			await _roleRepository.AddClaimAsync(role, claim, cancellationToken);
//		}

//		public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
//		{
//			if (role == null)
//				throw new ArgumentNullException(nameof(role));

//			if (await _roleRepository.CreateAsync(role, cancellationToken))
//			{
//				return IdentityResult.Success;
//			}
//			else
//			{
//				return IdentityResult.Failed(new IdentityError { Description = $"Create role {role} failed." });
//			}
//		}

//		public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
//		{
//			if (role == null)
//				throw new ArgumentNullException(nameof(role));

//			if (await _roleRepository.DeleteAsync(role.Id, cancellationToken))
//			{
//				return IdentityResult.Success;
//			}
//			else
//			{
//				return IdentityResult.Failed(new IdentityError { Description = $"Create role {role} failed." });
//			}
//		}

//		public void Dispose()
//		{

//		}

//		public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
//		{
//			if (string.IsNullOrWhiteSpace(roleId))
//				throw new ArgumentNullException(nameof(roleId));

//			return await _roleRepository.FindByIdAsync((TKey)Convert.ChangeType(roleId, typeof(TKey)));

//		}

//		public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
//		{
//			if (string.IsNullOrWhiteSpace(normalizedRoleName))
//				throw new ArgumentNullException(nameof(normalizedRoleName));

//			return await _roleRepository.FindByNameAsync(normalizedRoleName, cancellationToken);
//		}

//		public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			throw new NotImplementedException();
//		}

//		public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			throw new NotImplementedException();
//		}

//		public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}

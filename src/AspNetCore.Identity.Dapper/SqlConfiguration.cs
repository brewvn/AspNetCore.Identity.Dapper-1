using System;
using System.Linq;

namespace AspNetCore.Identity.Dapper
{
	public class SqlConfiguration
	{
		private readonly string _userTable;
		private readonly string _roleTable;
		private readonly string _userClaimsTable;
		private readonly string _userRolesTable;
		private readonly string _userLoginsTable;
		private readonly string _roleClaimsTable;
		private readonly string _userTokensTable;

		protected bool PrimaryKeyAutoIncrease { get; set; }

		public string Schema { get; }

		#region User

		public virtual string AddUserClaims { get; }
		public string AddLogin { get; }
		public string AddUserRole { get; }
		public string DeleteUserById { get; }
		public string CreateUser { get; private set; }
		public string FindUserByNormalizedEmail { get; }
		public string FindUserById { get; }
		public string FindUserByNormalizedName { get; }
		public string FindUserClaimsByUserId { get; }
		public string FindUserLoginsByUserId { get; }
		public string FindUserRolesByUserId { get; }
		public string FindUsersByClaim { get; }
		public string FindUsersByRoleName { get; }
		public string CountUserRolesByUserId { get; }
		public string RemoveUserClaimsByUserIdAndClaims { get; }
		public string RemoveUserRolesByUserIdAndRoleId { get; }
		public string RemoveUserLogins { get; }
		public string ReplaceUserClaim { get; }
		public string UpdateUser { get; }
		public string AddUserToken { get; }
		public string FindUserTokenByUserIdAndLoginProviderAndName { get; }
		public string FindUserLoginByUserIdAndLoginProviderAndProviderKey { get; }
		public string FindUserLoginByLoginProviderAndProviderKey { get; }
		public string FindUserRoleByUserIdAndRoleId { get; }
		public string RemoveUserTokenById { get; }

		#endregion

		#region Role 

		public string AddRoleClaim { get; }
		public string CreateRole { get; }
		public string DeleteRoleById { get; }
		public string FindRoleById { get; }
		public string FindRoleByNormalizedName { get; }
		public string FindRoleClaimsByRoleId { get; }
		public string RemoveRoleClaimsByRoleIdAndClaim { get; }
		public string SetNormalizedRoleNameById { get; }
		public string SetRoleNameById { get; }
		public string UpdateRole { get; }

		#endregion

		public SqlConfiguration(string schema, string userTable = "AspNetUsers", string roleTable = "AspNetRoles",
			string userClaimsTable = "AspNetUserClaims",
			string userRolesTable = "AspNetUserRoles", string userLoginsTable = "AspNetUserLogins",
			string roleClaimsTable =
				"AspNetRoleClaims",
			string userTokensTable = "AspNetUserTokens")
		{
			Schema = schema;
			_userTable = userTable;
			_roleTable = roleTable;
			_userClaimsTable = userClaimsTable;
			_userRolesTable = userRolesTable;
			_userLoginsTable = userLoginsTable;
			_roleClaimsTable = roleClaimsTable;
			_userTokensTable = userTokensTable;

			var schemaName = $"[{Schema}].[dbo]";
			AddUserClaims =
				$"INSERT INTO {schemaName}.[{_userClaimsTable}]([ClaimType],[ClaimValue],[UserId])VALUES(@ClaimType,@ClaimValue,@UserId)"
				;
			AddUserRole = $"INSERT INTO {schemaName}.[{_userRolesTable}] ([UserId],[RoleId]) VALUES (@UserId,@RoleId)";
			FindUserByNormalizedEmail =
				$"SELECT * FROM {schemaName}.[{_userTable}] WHERE [NormalizedEmail] = @NormalizedEmail";
			AddLogin =
				$"INSERT INTO {schemaName}.[{_userLoginsTable}] ([LoginProvider],[ProviderKey],[ProviderDisplayName],[UserId]) VALUES (@LoginProvider,@ProviderKey,@ProviderDisplayName,@UserId)"
				;
			FindUserLoginByLoginProviderAndProviderKey =
				$"SELECT * FROM {schemaName}.[{_userLoginsTable}] WHERE LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey";
			DeleteUserById = $"DELETE FROM {schemaName}.[{_userTable}] WHERE Id=@Id";
			CreateUser =
				$"INSERT INTO {schemaName}.[{_userTable}]([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])VALUES (@Id,@AccessFailedCount,@ConcurrencyStamp,@Email,@EmailConfirmed,@LockoutEnabled,@LockoutEnd,@NormalizedEmail,@NormalizedUserName,@PasswordHash,@PhoneNumber,@PhoneNumberConfirmed,@SecurityStamp,@TwoFactorEnabled,@UserName)"
				;
			FindUserById = $"SELECT * FROM {schemaName}.[{_userTable}] WHERE Id=@Id";
			FindUserByNormalizedName =
				$"SELECT * FROM {schemaName}.[{_userTable}] WHERE [NormalizedUserName] = @NormalizedUserName";
			FindUserClaimsByUserId = $"SELECT * FROM {schemaName}.[{_userClaimsTable}] WHERE UserId=@UserId";
			UpdateUser =
				$"UPDATE {schemaName}.[{_userTable}] SET [AccessFailedCount]=@AccessFailedCount,[ConcurrencyStamp]=@ConcurrencyStamp,[Email]=@Email,[EmailConfirmed]=@EmailConfirmed,[LockoutEnabled]=@LockoutEnabled,[LockoutEnd]=@LockoutEnd,[NormalizedEmail]=@NormalizedEmail,[NormalizedUserName]=@NormalizedUserName,[PasswordHash]=@PasswordHash,[PhoneNumber]=@PhoneNumber,[PhoneNumberConfirmed]=@PhoneNumberConfirmed,[SecurityStamp]=@SecurityStamp,[TwoFactorEnabled]=@TwoFactorEnabled,[UserName]=@UserName WHERE Id = @Id"
				;
			FindUserLoginsByUserId = $"SELECT * FROM {schemaName}.[{_userLoginsTable}] WHERE UserId=@UserId";
			FindUserRolesByUserId =
				$"SELECT {schemaName}.[{_roleTable}].Name FROM {schemaName}.[{_userRolesTable}],{schemaName}.[{_roleTable}] WHERE UserId=@UserId"
				;
			FindUsersByClaim =
				$"SELECT {schemaName}.[{_userTable}].* FROM {schemaName}.[{_userTable}], {schemaName}.[{_userClaimsTable}] WHERE {schemaName}.[{_userClaimsTable}].[ClaimType]=@ClaimType AND {schemaName}.[{_userClaimsTable}].[ClaimValue]=@ClaimValue"
				;
			FindUsersByRoleName =
				$"SELECT {schemaName}.[{_userTable}].* FROM {schemaName}.[{_userTable}], {schemaName}.[{_userRolesTable}], {schemaName}.[{_roleTable}] WHERE {schemaName}.[{_roleTable}].NormalizedName=@NormalizedName"
				;
			CountUserRolesByUserId = $"SELECT COUNT(*) FROM {schemaName}.[{_userRolesTable}] WHERE UserId=@UserId";
			RemoveUserClaimsByUserIdAndClaims =
				$"DELETE FROM {schemaName}.[{_userClaimsTable}] WHERE UserId=@UserId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue"
				;
			RemoveUserLogins =
				$"DELETE FROM {schemaName}.[{_userLoginsTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey"
				;
			ReplaceUserClaim =
				$"UPDATE {schemaName}.[{_userClaimsTable}] SET [ClaimType]=@NewClaimType, [ClaimValue]=@NewClaimValue WHERE UserId=@UserId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue"
				;
			AddUserToken =
				$"INSERT INTO {schemaName}.[{_userTokensTable}] ([UserId],[LoginProvider],[Name],[Value]) VALUES (@UserId,@LoginProvider,@Name,@Value)"
				;
			FindUserTokenByUserIdAndLoginProviderAndName =
				$"SELECT * FROM {schemaName}.[{_userTokensTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND Name=@Name"
				;
			FindUserLoginByUserIdAndLoginProviderAndProviderKey =
				$"SELECT * FROM {schemaName}.[{_userLoginsTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey"
				;
			FindUserRoleByUserIdAndRoleId =
				$"SELECT * FROM {schemaName}.[{_userRolesTable}] WHERE UserId=@UserId AND RoleId=@RoleId";
			RemoveUserTokenById = $"DELETE FROM {schemaName}.[{_userTokensTable}] WHERE Id=@Id";
			RemoveUserRolesByUserIdAndRoleId =
				$"DELETE FROM {schemaName}.[{_userRolesTable}] WHERE UserId=@UserId AND RoleId=@RoleId";
			CreateRole =
				$"INSERT INTO {schemaName}.[{_roleTable}] ([Id],[ConcurrencyStamp],[Name],[NormalizedName]) VALUES (@Id,@ConcurrencyStamp,@Name,@NormalizedName)"
				;
			AddRoleClaim =
				$"INSERT INTO {schemaName}.[{_roleClaimsTable}]([ClaimType],[ClaimValue],[RoleId])VALUES(@ClaimType,@ClaimValue,@RoleId)"
				;
			DeleteRoleById = $"DELETE FROM {schemaName}.[{_roleTable}] WHERE Id=@Id";
			FindRoleById = $"SELECT * FROM {schemaName}.[{_roleTable}] WHERE Id=@Id";
			FindRoleByNormalizedName = $"SELECT * FROM {schemaName}.[{_roleTable}] WHERE NormalizedName=@NormalizedName";
			FindRoleClaimsByRoleId = $"SELECT * FROM {schemaName}.[{_roleClaimsTable}] WHERE RoleId=@RoleId";
			RemoveRoleClaimsByRoleIdAndClaim =
				$"DELETE FROM {schemaName}.[{_roleClaimsTable}] WHERE RoleId=@RoleId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue"
				;
			SetNormalizedRoleNameById =
				$"UPDATE {schemaName}.[{_roleTable}] SET NormalizedName=@NormalizedName WHERE Id=@Id";
			SetRoleNameById = $"UPDATE {schemaName}.[{_roleTable}] SET Name=@Name WHERE Id=@Id";
			UpdateRole =
				$"UPDATE {schemaName}.[{_roleTable}] SET ConcurrencyStamp=@ConcurrencyStamp,Name=@Name,NormalizedName=@NormalizedName WHERE Id=@Id"
				;
		}

		public void InitUserSql(Type userType)
		{
			var keyType = userType.GetProperty("Id").PropertyType;
			if (keyType == typeof(int) || keyType == typeof(long))
			{
				PrimaryKeyAutoIncrease = true;
			}

			var colums = userType.GetProperties().Where(p => p.CanWrite && p.Name != "Id").Select(p => p.Name).ToList();
			colums.Sort();
			var columsSql = string.Join(",", colums.Select(c => $"[{c}]"));
			var valuesSql = string.Join(",", colums.Select(c => $"@{c}"));
			var schemaName = $"[{Schema}].[dbo]";
			CreateUser = PrimaryKeyAutoIncrease
				? $"INSERT INTO {schemaName}.[{_userTable}]({columsSql})VALUES ({valuesSql})"
				: $"INSERT INTO {schemaName}.[{_userTable}]([Id],{columsSql})VALUES (@Id,{valuesSql})";
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.Dapper
{
	public class SqlConfiguration
	{
		public string Schema { get; set; }

		#region User

		public virtual string AddUserClaims { get; }
		public string AddLogin { get; }
		public string AddUserRole { get; internal set; }
		public string DeleteUserById { get; internal set; }
		public string CreateUser { get; }
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
		public string RemoveUserRolesByUserIdAndNormalizedRoleName { get; }
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

		public string RoleTable { get; } = "AspNetRoles";
		public string UserTable { get; } = "AspNetUsers";
		public string UserClaimsTable { get; } = "AspNetUserClaims";
		public string UserRolesTable { get; } = "AspNetUserRoles";
		public string UserLoginsTable { get; } = "AspNetUserLogins";
		public string RoleClaimsTable { get; } = "AspNetRoleClaims";
		public string UserTokensTable { get; } = "AspNetUserTokens";

		public SqlConfiguration(string schema)
		{
			Schema = schema;
			var schemaName = $"[{Schema}].[dbo]";

			AddUserClaims = $"INSERT INTO {schemaName}.[{UserClaimsTable}]([ClaimType],[ClaimValue],[UserId])VALUES(@ClaimType,@ClaimValue,@UserId)";
			AddUserRole = $"INSET INTO {schemaName}.[{UserRolesTable}] ([UserId],[RoleId]) VALUES (@UserId,@RoleId)";
			FindUserByNormalizedEmail = $"SELECT * FROM {schemaName}.[{UserTable}] WHERE [NormalizedEmail] = @NormalizedEmail";
			AddLogin = $"INSERT INTO {schemaName}.[{UserLoginsTable}] ([LoginProvider],[ProviderKey],[ProviderDisplayName],[UserId]) VALUES (@LoginProvider,@ProviderKey,@ProviderDisplayName,@UserId)";
			FindUserLoginByLoginProviderAndProviderKey = $"SELECT * FROM {schemaName}.[{UserLoginsTable}] WHERE LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey";
			DeleteUserById = $"DELETE FROM {schemaName}.[{UserRolesTable}] WHERE Id=@Id";
			CreateUser = $"INSERT INTO {schemaName}.[{UserTable}]([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])VALUES (@Id,@AccessFailedCount,@ConcurrencyStamp,@Email,@EmailConfirmed,@LockoutEnabled,@LockoutEnd,@NormalizedEmail,@NormalizedUserName,@PasswordHash,@PhoneNumber,@PhoneNumberConfirmed,@SecurityStamp,@TwoFactorEnabled,@UserName)";
			FindUserById = $"SELECT * FROM {schemaName}.[{RoleTable}] WHERE Id=@Id";
			FindUserByNormalizedName = $"SELECT * FROM {schemaName}.[{UserTable}] WHERE [NormalizedName] = @NormalizedName";
			FindUserClaimsByUserId = $"SELECT * FROM {schemaName}.[{UserClaimsTable}] WHERE UserId=@UserId";
			UpdateUser = $"UPDATE {schemaName}.[{UserTable}] SET [AccessFailedCount]=@AccessFailedCount,[ConcurrencyStamp]=@ConcurrencyStamp,[Email]=@Email,[EmailConfirmed]=@EmailConfirmed,[LockoutEnabled]=@LockoutEnabled,[LockoutEnd]=@LockoutEnd,[NormalizedEmail]=@NormalizedEmail,[NormalizedUserName]=@NormalizedUserName,[PasswordHash]=@PasswordHash,[PhoneNumber]=@PhoneNumber,[PhoneNumberConfirmed]=@PhoneNumberConfirmed,[SecurityStamp]=@SecurityStamp,[TwoFactorEnabled]=@TwoFactorEnabled,[UserName]=@UserName WHERE Id = @Id";
			FindUserLoginsByUserId = $"SELECT * FROM {schemaName}.[{UserLoginsTable}] WHERE UserId=@UserId";
			FindUserRolesByUserId = $"SELECT * FROM {schemaName}.[{UserRolesTable}] WHERE UserId=@UserId";
			FindUsersByClaim = $"SELECT {schemaName}.[{UserTable}].* FROM {schemaName}.[{UserTable}], {schemaName}.[{UserClaimsTable}] WHERE {schemaName}.[{UserClaimsTable}].[ClaimType]=@ClaimType AND {schemaName}.[{UserClaimsTable}].[ClaimValue]=@ClaimValue";
			FindUsersByRoleName= $"SELECT {schemaName}.[{UserTable}].* FROM {schemaName}.[{UserTable}], {schemaName}.[{UserRolesTable}], {schemaName}.[{RoleTable}] WHERE {schemaName}.[{RoleTable}].NormalizedName=@NormalizedName";
			CountUserRolesByUserId = $"SELECT COUNT(*) FROM {schemaName}.[{UserRolesTable}] WHERE UserId=@UserId";
			RemoveUserClaimsByUserIdAndClaims = $"DELETE FROM {schemaName}.[UserClaimsTable] WHERE UserId=@UserId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue";
			RemoveUserLogins = $"DELETE FROM {schemaName}.[{UserLoginsTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey";
			ReplaceUserClaim = $"UPDATE {schemaName}.[{UserClaimsTable}] SET [ClaimType]=@ClaimType, [ClaimValue]=@ClaimValue WHERE UserId=@UserId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue";
			AddUserToken = $"INSERT INTO {schemaName}.[{UserTokensTable}] ([UserId],[LoginProvider],[Name],[Value]) VALUES (@UserId,@LoginProvider,@Name,@Value)";
			FindUserTokenByUserIdAndLoginProviderAndName = $"SELECT * FROM {schemaName}.[{UserTokensTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND Name=@Name";
			FindUserLoginByUserIdAndLoginProviderAndProviderKey = $"SELECT * FROM {schemaName}.[{UserLoginsTable}] WHERE UserId=@UserId AND LoginProvider=@LoginProvider AND ProviderKey=@ProviderKey";
			FindUserRoleByUserIdAndRoleId = $"SELECT * FROM {schemaName}.[{UserRolesTable}] WHERE UserId=@UserId AND RoleId=@RoleId";
			RemoveUserTokenById = $"DELETE FROM {schemaName}.[{UserTokensTable}] WHERE Id=@Id";

			CreateRole = $"INSERT INTO {schemaName}.[{RoleTable}] ([Id],[ConcurrencyStamp],[Name],[NormalizedName]) VALUES (@Id,@ConcurrencyStamp,@Name,@NormalizedName)";
			AddRoleClaim = $"INSERT INTO {schemaName}.[{RoleClaimsTable}]([ClaimType],[ClaimValue],[RoleId])VALUES(@ClaimType,@ClaimValue,@RoleId)";
			DeleteRoleById = $"DELETE FROM {schemaName}.[{RoleTable}] WHERE Id=@Id";
			FindRoleById = $"SELECT * FROM {schemaName}.[{RoleTable}] WHERE Id=@Id";
			FindRoleByNormalizedName = $"SELECT * FROM {schemaName}.[{RoleTable}] WHERE NormalizedName=@NormalizedName";
			FindRoleClaimsByRoleId = $"SELECT * FROM {schemaName}.[{RoleClaimsTable}] WHERE RoleId=@RoleId";
			RemoveRoleClaimsByRoleIdAndClaim = $"DELETE FROM {schemaName}.[{RoleClaimsTable}] WHERE RoleId=@RoleId AND [ClaimType]=@ClaimType AND [ClaimValue]=@ClaimValue";
			SetNormalizedRoleNameById = $"UPDATE {schemaName}.[{RoleTable}] SET NormalizedName=@NormalizedName WHERE Id=@Id";
			SetRoleNameById = $"UPDATE {schemaName}.[{RoleTable}] SET Name=@Name WHERE Id=@Id";
			UpdateRole = $"UPDATE {schemaName}.[{RoleTable}] SET ConcurrencyStamp=@ConcurrencyStamp,Name=@Name,NormalizedName=@NormalizedName WHERE Id=@Id";
		}
	}
}

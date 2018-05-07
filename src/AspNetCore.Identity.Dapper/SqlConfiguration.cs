using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.Dapper
{
	public class SqlConfiguration
	{
		public string Schema { get; set; }

		#region User

		public virtual string AddUserClaims { get; } = "INSERT";
		public string AddLogin { get; } = "INSERT";
		public string FindRoleByNormalizedRoleName { get; }
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
		public string FindUsersByRole { get; }
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

			CreateUser = $"INSERT INTO [{Schema}].[dbo].[{UserTable}]([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])VALUES (@Id,@AccessFailedCount,@ConcurrencyStamp,@Email,@EmailConfirmed,@LockoutEnabled,@LockoutEnd,@NormalizedEmail,@NormalizedUserName,@PasswordHash,@PhoneNumber,@PhoneNumberConfirmed,@SecurityStamp,@TwoFactorEnabled,@UserName)";
		}
	}
}

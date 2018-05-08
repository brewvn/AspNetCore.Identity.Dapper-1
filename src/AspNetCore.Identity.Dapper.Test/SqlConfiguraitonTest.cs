using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AspNetCore.Identity.Dapper.Test
{
	public class SqlConfiguraitonTest
	{
		public class User1 : IdentityUser
		{
			public string Nick { get; set; }
		}

		public class User2 : IdentityUser<int>
		{
			public string Nick { get; set; }
		}

		[Fact(DisplayName = "InitUserSql")]
		public void InitUserSql()
		{
			SqlConfiguration sqlConfiguration = new SqlConfiguration("test");
			sqlConfiguration.InitUserSql(typeof(User1));
			var sql1 = "INSERT INTO [test].[dbo].[AspNetUsers]([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[Nick],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])VALUES (@Id,@AccessFailedCount,@ConcurrencyStamp,@Email,@EmailConfirmed,@LockoutEnabled,@LockoutEnd,@Nick,@NormalizedEmail,@NormalizedUserName,@PasswordHash,@PhoneNumber,@PhoneNumberConfirmed,@SecurityStamp,@TwoFactorEnabled,@UserName)";
			Assert.Equal(sql1, sqlConfiguration.CreateUser);

			sqlConfiguration.InitUserSql(typeof(User2));
			var sql2 = "INSERT INTO [test].[dbo].[AspNetUsers]([AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[Nick],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])VALUES (@AccessFailedCount,@ConcurrencyStamp,@Email,@EmailConfirmed,@LockoutEnabled,@LockoutEnd,@Nick,@NormalizedEmail,@NormalizedUserName,@PasswordHash,@PhoneNumber,@PhoneNumberConfirmed,@SecurityStamp,@TwoFactorEnabled,@UserName)";
			Assert.Equal(sql2, sqlConfiguration.CreateUser);
		}
	}
}

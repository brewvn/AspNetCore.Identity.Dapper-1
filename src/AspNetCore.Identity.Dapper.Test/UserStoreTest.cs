using Microsoft.AspNetCore.Identity;
using System;
using Xunit;

namespace AspNetCore.Identity.Dapper.Test
{
	public class UserStoreTest
	{
		private UserStore CreateUserStore()
		{
			return new UserStore(new IdentityErrorDescriber(), new TestSqlServerConnectionProvider(), new SqlConfiguration("IdentityDapperTest"));
		}

		private UserManager<IdentityUser> CreateUserManager()
		{
			var userStore = CreateUserStore();
			return MockHelpers.TestUserManager(userStore);
		}

		[Fact(DisplayName = "CreateUser")]
		public void CreateAsync()
		{
			var userManager = CreateUserManager();

			var str = Guid.NewGuid().ToString("N");
			var result = userManager.CreateAsync(new IdentityUser
			{
				Email = str + "@163.com",
				PasswordHash = str
			}).Result;
		}
	}
}

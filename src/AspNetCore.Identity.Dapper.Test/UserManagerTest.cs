using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Identity.Dapper.Test
{
	public class UserManagerTest
	{
		[Fact(DisplayName = "Create")]
		public async void CreateAsync()
		{
			var userManager = MockHelpers.TestUserManager();
			var user = await AddUser(userManager);
			Assert.NotNull(user);
		}

		[Fact(DisplayName = "AddClaim")]
		public async void AddClaimAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager);

			var result = await userManager.AddClaimAsync(user, new Claim("test", "value"));

			Assert.True(result.Succeeded);
		}

		[Fact(DisplayName = "AddClaims")]
		public async void AddClaimsAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager);
			var result = await userManager.AddClaimsAsync(user, new List<Claim> { new Claim("test1", "value1"), new Claim("test2", "value2") });

			Assert.True(result.Succeeded);
		}

		[Fact(DisplayName = "Update")]
		public async void UpdateAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager);

			user.Email = "zlzforever@163.com";
			await userManager.UpdateAsync(user);

			var userNew = await userManager.FindByEmailAsync("zlzforever@163.com");

			Assert.NotNull(userNew);
		}

		[Fact(DisplayName = "FindByEmail")]
		public async void FindByEmailAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager);

			var userNew = await userManager.FindByEmailAsync(user.Email);

			Assert.NotNull(userNew);
		}

		[Fact(DisplayName = "AddLogin")]
		public async void AddLoginAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager);

			var userNew = await userManager.AddLoginAsync(user, new UserLoginInfo("provider1", "key1", "name1"));

			Assert.NotNull(userNew);
		}

		[Fact(DisplayName = "AddPassword")]
		public async void AddPasswordAsync()
		{
			var userManager = MockHelpers.TestUserManager();

			var user = await AddUser(userManager, new IdentityUser { Email = Guid.NewGuid().ToString("N") + "@163.com" });

			var result = await userManager.AddPasswordAsync(user, "ZLZFOREVERabcd@100");
			Assert.True(result.Succeeded);
			var userNew = await userManager.FindByEmailAsync(user.Email);

			Assert.NotNull(userNew);
			Assert.NotNull(userNew.PasswordHash);
		}

		private async Task<IdentityUser> AddUser(UserManager<IdentityUser> userManager, IdentityUser user = null)
		{
			var str = Guid.NewGuid().ToString("N");
			if (user == null)
			{
				user = new IdentityUser
				{
					Email = str + "@163.com",
					PasswordHash = str
				};
			}
			var result = await userManager.CreateAsync(user);
			return result.Succeeded ? user : null;
		}

		private void Test()
		{
			var userManager = MockHelpers.TestUserManager();
		}
	}
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using AspNetCore.Identity.Dapper.Stores;

namespace AspNetCore.Identity.Dapper.Test
{
	public class UserStoreTest
	{
		[Fact(DisplayName = "CreateUser")]
		public async void CreateUserAsync()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			Assert.NotNull(user);
		}

		[Fact(DisplayName = "AddClaimsAndGetClaims")]
		public async void AddClaims()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			await store.AddClaimsAsync(user, new[] { new Claim("test", "value") });

			var claim = (await store.GetClaimsAsync(user)).FirstOrDefault();
			Assert.Equal("test", claim.Type);
			Assert.Equal("value", claim.Value);
		}

		[Fact(DisplayName = "GetUsersForClaim")]
		public async void GetUsersForClaim()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			var claim = new Claim("test", "value");
			await store.AddClaimsAsync(user, new[] { claim });
			var users = await store.GetUsersForClaimAsync(claim);
			Assert.Contains(users, u => u.Id == user.Id);
		}

		[Fact(DisplayName = "AddLoginAndFindByLogin")]
		public async void AddLoginAsyncAndFindByLoginAsync()
		{
			var store = TestHelpers.CreateUserStore();

			var user = await AddUser(store);

			var key = Guid.NewGuid().ToString("N");

			await store.AddLoginAsync(user, new UserLoginInfo("provider1", key, "name1"));

			var login = (await store.GetLoginsAsync(user)).FirstOrDefault();
			Assert.Equal("provider1", login.LoginProvider);
			Assert.Equal(key, login.ProviderKey);
			Assert.Equal("name1", login.ProviderDisplayName);
		}

		[Fact(DisplayName = "AddToRoleAndGetRoles")]
		public async void AddToRoleAndGetRoles()
		{
			var roleStore = TestHelpers.CreateRoleStore();
			var role = await RoleStoreTest.AddRole(roleStore);

			var store = TestHelpers.CreateUserStore();

			var user = await AddUser(store);

			await store.AddToRoleAsync(user, role.NormalizedName);

			var roles = (await store.GetRolesAsync(user)).ToList();
			Assert.Contains(role.Name, roles);
		}

		[Fact(DisplayName = "IsInRole")]
		public async void IsInRole()
		{
			var roleStore = TestHelpers.CreateRoleStore();
			var role = await RoleStoreTest.AddRole(roleStore);

			var store = TestHelpers.CreateUserStore();

			var user = await AddUser(store);

			await store.AddToRoleAsync(user, role.NormalizedName);

			var isInRole = await store.IsInRoleAsync(user, role.NormalizedName);
			Assert.True(isInRole);
		}

		[Fact(DisplayName = "DeleteAndFindByName")]
		public async void DeleteAsyncAndFindByName()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			await store.DeleteAsync(user);
			var userNew = await store.FindByNameAsync(user.NormalizedUserName);
			Assert.Null(userNew);
		}

		[Fact(DisplayName = "FindByEmail")]
		public async void FindByEmailAsync()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);

			var userNew = await store.FindByEmailAsync(user.Email);
			AssertUser(user, userNew);
		}

		[Fact(DisplayName = "FindById")]
		public async void FindByIdAsync()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);

			var userNew = await store.FindByIdAsync(user.Id);
			AssertUser(user, userNew);
		}

		[Fact(DisplayName = "FindRole")]
		public async void FindRoleAsync()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);

			var email = await store.GetEmailAsync(user);
			Assert.Equal(user.Email, email);
		}

		[Fact(DisplayName = "GetUsersForClaim")]
		public async void GetUsersForClaimAsync()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			var claim = new Claim("test", "value");
			await store.AddClaimsAsync(user, new[] { claim });
			var users = await store.GetUsersForClaimAsync(claim);
			Assert.Contains(users, u => u.Id == user.Id);
		}

		[Fact(DisplayName = "RemoveClaims")]
		public async void RemoveClaims()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			await store.AddClaimsAsync(user, new[] { new Claim("test", "value") });
			var claim = (await store.GetClaimsAsync(user)).FirstOrDefault();
			Assert.Equal("test", claim.Type);
			Assert.Equal("value", claim.Value);
			await store.RemoveClaimsAsync(user, new[] { claim });
			claim = (await store.GetClaimsAsync(user)).FirstOrDefault();
			Assert.Null(claim);
		}

		[Fact(DisplayName = "RemoveFromRole")]
		public async void RemoveFromRole()
		{
			var roleStore = TestHelpers.CreateRoleStore();
			var role = await RoleStoreTest.AddRole(roleStore);

			var store = TestHelpers.CreateUserStore();

			var user = await AddUser(store);

			await store.AddToRoleAsync(user, role.NormalizedName);

			var roles = (await store.GetRolesAsync(user)).ToList();
			Assert.Contains(role.Name, roles);

			await store.RemoveFromRoleAsync(user, role.NormalizedName);

			roles = (await store.GetRolesAsync(user)).ToList();
			Assert.DoesNotContain(role.Name, roles);
		}

		[Fact(DisplayName = "RemoveLogin")]
		public async void RemoveLogin()
		{
			var store = TestHelpers.CreateUserStore();

			var user = await AddUser(store);

			var key = Guid.NewGuid().ToString("N");

			await store.AddLoginAsync(user, new UserLoginInfo("provider1", key, "name1"));
			await store.RemoveLoginAsync(user, key, "name1");

			var login = (await store.GetLoginsAsync(user)).FirstOrDefault(l => l.ProviderKey == "provider1" && l.ProviderKey == key && l.ProviderDisplayName == "name1");
			Assert.Null(login);
		}

		[Fact(DisplayName = "ReplaceClaim")]
		public async void ReplaceClaim()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			var claim = new Claim("test", "value");
			await store.AddClaimsAsync(user, new[] { new Claim("test", "value") });
			var newClaim = new Claim("test2", "value2");
			await store.ReplaceClaimAsync(user, claim, newClaim);


			var resultClaim = (await store.GetClaimsAsync(user)).FirstOrDefault();
			Assert.Equal("test2", resultClaim.Type);
			Assert.Equal("value2", resultClaim.Value);
		}

		[Fact(DisplayName = "Update")]
		public async void Update()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
			var newName = Guid.NewGuid().ToString("N");
			user.AccessFailedCount = 1;
			user.ConcurrencyStamp = "aaa";
			user.Email = "1@1.com";
			user.EmailConfirmed = true;
			user.LockoutEnabled = true;
			user.LockoutEnd = DateTime.Now;
			user.NormalizedEmail = "2@2.com";
			user.NormalizedUserName = newName.ToUpper();
			user.PasswordHash = "ppp";
			user.PhoneNumber = "123";
			user.PhoneNumberConfirmed = true;
			user.SecurityStamp = "bbb";
			user.TwoFactorEnabled = true;
			user.UserName = newName;

			await store.UpdateAsync(user);

			var newUser = await store.FindByIdAsync(user.Id);
			AssertUser(user, newUser);
		}

		[Fact(DisplayName = "AddUserToken")]
		public async void AddUserToken()
		{
			var store = TestHelpers.CreateUserStore();
			var user = await AddUser(store);
		}

		private async Task<IdentityUser> AddUser(UserStore store, IdentityUser user = null)
		{
			var str = Guid.NewGuid().ToString("N");
			if (user == null)
			{
				user = new IdentityUser
				{
					Email = str + "@163.com",
					NormalizedEmail = str + "@163.com",
					PasswordHash = str,
					NormalizedUserName = str
				};
			}
			var result = await store.CreateAsync(user);
			return result.Succeeded ? user : null;
		}

		private void AssertUser(IdentityUser user, IdentityUser userNew)
		{
			Assert.Equal(user.AccessFailedCount, userNew.AccessFailedCount);
			Assert.Equal(user.ConcurrencyStamp, userNew.ConcurrencyStamp);
			Assert.Equal(user.Email, userNew.Email);
			Assert.Equal(user.EmailConfirmed, userNew.EmailConfirmed);
			Assert.Equal(user.Id, userNew.Id);
			Assert.Equal(user.LockoutEnabled, userNew.LockoutEnabled);
			Assert.Equal(user.LockoutEnd, userNew.LockoutEnd);
			Assert.Equal(user.NormalizedEmail, userNew.NormalizedEmail);
			Assert.Equal(user.NormalizedUserName, userNew.NormalizedUserName);
			Assert.Equal(user.PasswordHash, userNew.PasswordHash);
			Assert.Equal(user.PhoneNumber, userNew.PhoneNumber);
			Assert.Equal(user.PhoneNumberConfirmed, userNew.PhoneNumberConfirmed);
			Assert.Equal(user.SecurityStamp, userNew.SecurityStamp);
			Assert.Equal(user.TwoFactorEnabled, userNew.TwoFactorEnabled);
			Assert.Equal(user.UserName, userNew.UserName);
		}
	}
}

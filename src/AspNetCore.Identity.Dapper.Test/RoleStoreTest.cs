using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper.Stores;
using Xunit;

namespace AspNetCore.Identity.Dapper.Test
{
	public class RoleStoreTest
	{
		[Fact(DisplayName = "AddClaimAndGetClaims")]
		public async void AddClaim()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);
			var claim = new Claim("roleClaim", "roleClaimValue");
			await store.AddClaimAsync(role, claim);

			var resultClaim = (await store.GetClaimsAsync(role)).FirstOrDefault();
			Assert.Equal("roleClaim", resultClaim.Type);
			Assert.Equal("roleClaimValue", resultClaim.Value);
		}

		[Fact(DisplayName = "CreateRole")]
		public async void CreateRoleAsync()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);
			Assert.NotNull(role);

			var roleNew = await store.FindByNameAsync(role.NormalizedName);
			Assert.NotNull(roleNew);
		}

		[Fact(DisplayName = "DeleteAndFindByName")]
		public async void DeleteAsyncAndFindByName()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);

			await store.DeleteAsync(role);
			var userNew = await store.FindByNameAsync(role.NormalizedName);
			Assert.Null(userNew);
		}

		[Fact(DisplayName = "FindById")]
		public async void FindByIdAsync()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);

			var roleNew = await store.FindByIdAsync(role.Id);
			AssertRole(role, roleNew);
		}

		[Fact(DisplayName = "RemoveClaims")]
		public async void RemoveClaims()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);

			var claim = new Claim("roleClaim", "roleClaimValue");
			await store.AddClaimAsync(role, claim);

			await store.RemoveClaimAsync(role, claim);
			claim = (await store.GetClaimsAsync(role)).FirstOrDefault();
			Assert.Null(claim);
		}

		[Fact(DisplayName = "SetNormalizedRoleName")]
		public async void SetNormalizedRoleName()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);
			var name = Guid.NewGuid().ToString("N");
			await store.SetNormalizedRoleNameAsync(role, name);

			var newRole = await store.FindByIdAsync(role.Id);

			Assert.Equal(name, newRole.NormalizedName);
		}

		[Fact(DisplayName = "SetRoleName")]
		public async void SetRoleName()
		{
			var store = TestHelpers.CreateRoleStore();
			var role = await AddRole(store);
			var name = Guid.NewGuid().ToString("N");
			await store.SetRoleNameAsync(role, name);

			var newRole = await store.FindByIdAsync(role.Id);

			Assert.Equal(name, newRole.Name);
		}

		private void AssertRole(IdentityRole role, IdentityRole roleNew)
		{
			Assert.Equal(role.ConcurrencyStamp, roleNew.ConcurrencyStamp);
			Assert.Equal(role.Id, roleNew.Id);
			Assert.Equal(role.Name, roleNew.Name);
			Assert.Equal(role.NormalizedName, roleNew.NormalizedName);
		}

		public static async Task<IdentityRole> AddRole(RoleStore roleStore, IdentityRole role = null)
		{
			var str = Guid.NewGuid().ToString("N");
			if (role == null)
			{
				role = new IdentityRole
				{
					Name = str,
					NormalizedName = str.ToUpper()
				};
			}
			var result = await roleStore.CreateAsync(role);
			return result.Succeeded ? role : null;
		}
	}
}

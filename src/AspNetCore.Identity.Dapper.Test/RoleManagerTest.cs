using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Identity.Dapper.Test
{
	public class RoleManagerTest
	{
		[Fact(DisplayName = "CreateAsync")]
		public async void CreateAsync()
		{
			var roleManage = MockHelpers.TestRoleManager();
			var role = await AddRole(roleManage);
			Assert.NotNull(role);
		}

		private async Task<IdentityRole> AddRole(RoleManager<IdentityRole> roleManage, IdentityRole role = null)
		{
			var str = Guid.NewGuid().ToString("N");
			if (role == null)
			{
				role = new IdentityRole
				{
					Name = str
				};
			}
			var result = await roleManage.CreateAsync(role);
			return result.Succeeded ? role : null;
		}
	}
}

using AspNetCore.Identity.Dapper.Stores;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Test
{
	public static class TestHelpers
	{
		public static RoleStore CreateRoleStore()
		{
			var provider = new TestSqlServerConnectionProvider();
			provider.SqlConfiguration.InitUserSql(typeof(IdentityUser));
			return new RoleStore(new IdentityErrorDescriber(), provider);
		}

		public static UserStore CreateUserStore()
		{
			var provider = new TestSqlServerConnectionProvider();
			provider.SqlConfiguration.InitUserSql(typeof(IdentityUser));
			return new UserStore(new IdentityErrorDescriber(), provider);
		}
	}
}

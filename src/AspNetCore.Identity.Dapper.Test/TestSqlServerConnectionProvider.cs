using System.Data.Common;
using System.Data.SqlClient;

namespace AspNetCore.Identity.Dapper.Test
{
	public class TestSqlServerConnectionProvider : IStoreProvider
	{
		public SqlConfiguration SqlConfiguration { get; }

		public TestSqlServerConnectionProvider()
		{
			SqlConfiguration = new SqlConfiguration(Create().Database);
		}

		public DbConnection Create()
		{
			return new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=AspNetCore.Identity.Dapper.Samples.Web;Integrated Security=True");
		}
	}
}

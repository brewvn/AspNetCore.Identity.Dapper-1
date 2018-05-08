using System.Data.Common;
using System.Data.SqlClient;

namespace AspNetCore.Identity.Dapper
{
	public class SqlServerProvider : IStoreProvider
	{
		private readonly string _connectionString;

		public SqlServerProvider(string connectionString)
		{
			_connectionString = connectionString;
			SqlConfiguration = new SqlConfiguration(Create().Database);
		}

		public SqlServerProvider(string connectionString, SqlConfiguration sqlConfiguration)
		{
			_connectionString = connectionString;
			SqlConfiguration = sqlConfiguration;
		}

		public SqlConfiguration SqlConfiguration { get; }

		public DbConnection Create()
		{
			return new SqlConnection(_connectionString);
		}
	}
}

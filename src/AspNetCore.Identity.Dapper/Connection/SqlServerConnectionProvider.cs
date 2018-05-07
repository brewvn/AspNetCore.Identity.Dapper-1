using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace AspNetCore.Identity.Dapper
{
	public class SqlServerConnectionProvider : IConnectionProvider
	{
		public DbConnection Create()
		{
			var conn = SqlClientFactory.Instance.CreateConnection();
			if (conn.State != System.Data.ConnectionState.Open)
			{
				conn.Open();
			}
			return conn;
		}
	}
}

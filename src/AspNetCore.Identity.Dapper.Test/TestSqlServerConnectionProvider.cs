using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace AspNetCore.Identity.Dapper.Test
{
	public class TestSqlServerConnectionProvider : IConnectionProvider
	{
		public DbConnection Create()
		{
			return new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
		}
	}
}

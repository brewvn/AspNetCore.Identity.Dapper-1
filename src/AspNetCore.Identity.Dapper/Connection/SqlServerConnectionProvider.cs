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
		private readonly IConfiguration _configuration;

		public SqlServerConnectionProvider(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public DbConnection Create()
		{
			return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace AspNetCore.Identity.Dapper
{
	public interface IConnectionProvider
	{
		DbConnection Create();
	}
}

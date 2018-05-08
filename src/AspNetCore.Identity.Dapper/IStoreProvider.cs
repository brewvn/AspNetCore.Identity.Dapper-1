using System.Data.Common;

namespace AspNetCore.Identity.Dapper
{
	public interface IStoreProvider
	{
		DbConnection Create();
		SqlConfiguration SqlConfiguration { get; }
	}
}

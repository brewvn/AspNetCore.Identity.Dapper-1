using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
	public static class DapperExtensions
	{
		public static async Task<bool> ExecuteTransactionAsync(this IDbConnection cnn, string sql, object param = null, Func<int, bool> rollBackChecker = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			bool success = false;
			var transaction = cnn.BeginTransaction();
			try
			{
				var rows = await cnn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
				var rollBack = rollBackChecker == null ? false : rollBackChecker(rows);

				if (rollBack)
				{
					transaction.Rollback();
				}
				else
				{
					transaction.Commit();
					success = true;
				}
			}
			catch
			{
				transaction.Rollback();
			}
			return await Task.FromResult(success);
		}
	}
}

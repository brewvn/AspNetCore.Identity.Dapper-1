using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IUserTokenRepository<TUserToken, TKey> where TKey : IEquatable<TKey> where TUserToken : IdentityUserToken<TKey>, new()
	{
		Task AddUserTokenAsync(TUserToken token);
		Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken);
		Task RemoveUserTokenAsync(TKey userId);
	}

	public class UserTokenRepository<TUserToken, TKey> : IUserTokenRepository<TUserToken, TKey> where TKey : IEquatable<TKey> where TUserToken : IdentityUserToken<TKey>, new()
	{
		public Task AddUserTokenAsync(TUserToken token)
		{
			throw new NotImplementedException();
		}

		public Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveUserTokenAsync(TKey userId)
		{
			throw new NotImplementedException();
		}
	}
}

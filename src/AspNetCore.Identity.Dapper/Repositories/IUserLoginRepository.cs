﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper.Repositories
{
	public interface IUserLoginRepository<TUserLogin, TKey> where TKey : IEquatable<TKey> where TUserLogin : IdentityUserLogin<TKey>, new()
	{
		Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken);
		Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
		Task AddLoginAsync(TKey userId, UserLoginInfo login, CancellationToken cancellationToken);
		Task<IList<UserLoginInfo>> GetLoginsAsync(TKey userId);
		Task RemoveLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken);
	}
}

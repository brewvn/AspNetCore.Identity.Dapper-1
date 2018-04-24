using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace AspNetCore.Identity.Dapper
{
	public interface ISqlGenerator
	{
		string TableName { get; }

		string PasswordHash { get; }

		string EmailConfirmed { get; }

		string NormalizedEmail { get; }

		string Email { get; }

		string NormalizedUserName { get; }

		string UserName { get; }
	}

	public class SqlGenerator : ISqlGenerator
	{
		public virtual string TableName => "TableName";

		public virtual string PasswordHash => "PasswordHash";

		public virtual string EmailConfirmed => "EmailConfirmed";

		public virtual string NormalizedEmail => "NormalizedEmail";

		public virtual string Email => "Email";

		public virtual string NormalizedUserName => "NormalizedUserName";

		public virtual string UserName => "UserName";

		public virtual string CreateUserSql => "";

		public virtual string DeleteUserSql => $"DELETE FROM {TableName} WHERE Id = @Id;";

		public virtual string FindByEmail => $"SELECT * FROM {TableName} Where {Email} = @Email;";

		public virtual string FindById => $"SELECT * FROM {TableName} Where Id = @Id;";

		public virtual string SetEmail => $"UPDATE {TableName} SET {Email} = @Email WHERE Id = @Id;";

		public virtual string SetEmailConfirmed => $"UPDATE {TableName} SET {EmailConfirmed} = @EmailConfirmed WHERE Id = @Id;";

		public virtual string SetNormalizedEmail => $"UPDATE {TableName} SET {NormalizedEmail} = @NormalizedEmail WHERE Id = @Id;";

		public virtual string SetNormalizedUserName => $"UPDATE {TableName} SET {NormalizedUserName} = @NormalizedUserName WHERE Id = @Id;";

		public virtual string SetPasswordHash => $"UPDATE {TableName} SET {PasswordHash} = @PasswordHash WHERE Id = @Id;";

		public virtual string SetUserName => $"UPDATE {TableName} SET {UserName} = @UserName WHERE Id = @Id;";

		public virtual string Update => $"UPDATE {TableName} SET {UserName} = @UserName WHERE Id = @Id;";
	}

	/// <summary>
	/// This store is only partially implemented. It supports user creation and find methods.
	/// </summary>
	public class UserStore<TUser> : IUserStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserEmailStore<TUser>
		where TUser : IdentityUser
	{
		private readonly SqlGenerator _sqlGenerator;
		private readonly IDbConnection _connection;

		public UserStore(SqlGenerator sqlGenerator, IDbConnection connection)
		{
			_sqlGenerator = sqlGenerator;
			_connection = connection;
		}

		#region createuser
		public async Task<IdentityResult> CreateAsync(TUser user,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			var rows = await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, user);
			if (rows > 0)
			{
				return IdentityResult.Success;
			}
			else
			{
				return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user}." });
			}
		}
		#endregion

		public async Task<IdentityResult> DeleteAsync(TUser user,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			int rows = await _connection.ExecuteAsync(_sqlGenerator.DeleteUserSql, new { user.Id });

			if (rows > 0)
			{
				return IdentityResult.Success;
			}
			return IdentityResult.Failed(new IdentityError { Description = $"Could not delete user {user.Email}." });
		}

		public void Dispose()
		{
		}

		public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			return await _connection.QueryFirstAsync<TUser>(_sqlGenerator.CreateUserSql, new { NormalizedEmail = normalizedEmail });
		}

		public async Task<TUser> FindByIdAsync(string userId,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (userId == null) throw new ArgumentNullException(nameof(userId));
			Guid idGuid;
			if (!Guid.TryParse(userId, out idGuid))
			{
				throw new ArgumentException("Not a valid Guid id", nameof(userId));
			}

			return await _connection.QueryFirstAsync<TUser>(_sqlGenerator.CreateUserSql, new { Id = userId });
		}

		public async Task<TUser> FindByNameAsync(string userName,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (userName == null) throw new ArgumentNullException(nameof(userName));

			return await _connection.QueryFirstAsync<TUser>(_sqlGenerator.CreateUserSql, new { UserName = userName });
		}

		public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.EmailConfirmed);
		}

		public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.NormalizedEmail);
		}

		public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.PasswordHash);
		}

		public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.UserName);
		}

		public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return await Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
		}

		public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, Email = email });
		}

		public async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, EmailConfirmed = confirmed });
		}

		public async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(normalizedEmail)) throw new ArgumentNullException(nameof(normalizedEmail));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, NormalizedEmail = normalizedEmail });
		}

		public async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, NormalizedName = normalizedName });
		}

		public async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, PasswordHash = passwordHash });
		}

		public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

			await _connection.ExecuteAsync(_sqlGenerator.CreateUserSql, new { user.Id, UserName = userName });
		}

		public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null) throw new ArgumentNullException(nameof(user));

			var rows = await _connection.ExecuteAsync(_sqlGenerator.Update, user);
			if (rows > 0)
			{
				return IdentityResult.Success;
			}
			else
			{
				return IdentityResult.Failed(new IdentityError { Description = $"Update user {user} failed." });
			}
		}
	}
}

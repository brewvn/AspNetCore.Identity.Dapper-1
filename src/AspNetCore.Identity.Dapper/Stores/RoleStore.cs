using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using AspNetCore.Identity.Dapper.Repositories;

namespace AspNetCore.Identity.Dapper
{
	public class RoleStore<TRole, TKey, TUserRole, TRoleClaim> :
	   IQueryableRoleStore<TRole>,
	   IRoleClaimStore<TRole>
	   where TRole : IdentityRole<TKey>
	   where TKey : IEquatable<TKey>
	   where TUserRole : IdentityUserRole<TKey>, new()
	   where TRoleClaim : IdentityRoleClaim<TKey>, new()
	{
		private readonly IConnectionProvider _connectionProvider;
		private readonly SqlConfiguration _sqlConfiguration;

		public RoleStore(IConnectionProvider connectionProvider, SqlConfiguration sqlConfiguration)
		{
			_connectionProvider = connectionProvider;
			_sqlConfiguration = sqlConfiguration;
		}

		// Dapper 不可能实现这个
		public IQueryable<TRole> Roles => throw new NotImplementedException();

		public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _connectionProvider.Create())
			{
				var roleClaim = Activator.CreateInstance<TRoleClaim>();
				roleClaim.RoleId = role.Id;
				roleClaim.ClaimType = claim.Type;
				roleClaim.ClaimValue = claim.Value;

				await conn.ExecuteAsync(_sqlConfiguration.AddRoleClaim, new { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
			}
		}

		public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.CreateRole, role);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.DeleteRoleById, role);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public void Dispose()
		{
		}

		public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(roleId))
				throw new ArgumentNullException(nameof(roleId));

			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstAsync<TRole>(conn, _sqlConfiguration.FindRoleById, new { Id = roleId });
			}
		}

		public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));
			using (var conn = _connectionProvider.Create())
			{
				return await SqlMapper.QueryFirstAsync<TRole>(conn, _sqlConfiguration.FindRoleByNormalizedName, new { NormalizedRoleName = normalizedRoleName });
			}
		}

		public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _connectionProvider.Create())
			{
				var roleClaims = await SqlMapper.QueryAsync(conn, _sqlConfiguration.FindRoleClaimsByRoleId, new { RoleId = role.Id });
				var results = roleClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
				return results;
			}
		}

		public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.NormalizedName);
		}

		public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.Id.ToString());
		}

		public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.Name);
		}

		public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			using (var conn = _connectionProvider.Create())
			{
				var paramters = new { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
				await conn.ExecuteAsync(_sqlConfiguration.RemoveRoleClaimsByRoleIdAndClaim, paramters);
			}
		}

		public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (string.IsNullOrWhiteSpace(normalizedName))
				throw new ArgumentNullException(nameof(normalizedName));

			using (var conn = _connectionProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.SetNormalizedRoleNameById, new { role.Id, NormalizedName = normalizedName });
			}
		}

		public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException(nameof(roleName));

			using (var conn = _connectionProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.SetRoleNameById, new { role.Id, Name = roleName });
			}
		}

		public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _connectionProvider.Create())
			{
				var result = await SqlMapper.ExecuteAsync(conn, _sqlConfiguration.UpdateRole, role);
				return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Update user failed." });
			}
		}
	}
}

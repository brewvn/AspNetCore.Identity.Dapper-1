using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Stores
{
	public class RoleStore : RoleStore<IdentityRole>
	{
		public RoleStore(IdentityErrorDescriber describer, IStoreProvider connectionProvider) : base(describer, connectionProvider)
		{
		}
	}

	public class RoleStore<TRole> : RoleStore<TRole, string>
		where TRole : IdentityRole<string>
	{
		public RoleStore(IdentityErrorDescriber describer, IStoreProvider connectionProvider) : base(describer, connectionProvider)
		{
		}
	}

	public class RoleStore<TRole, TKey> : RoleStore<TRole, TKey, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		public RoleStore(IdentityErrorDescriber describer, IStoreProvider connectionProvider) : base(describer, connectionProvider)
		{
		}
	}


	public class RoleStore<TRole, TKey, TUserRole, TRoleClaim> : RoleStoreBase<TRole, TKey, TUserRole, TRoleClaim>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserRole : IdentityUserRole<TKey>, new()
		where TRoleClaim : IdentityRoleClaim<TKey>, new()
	{
		private readonly IStoreProvider _storeProvider;
		private readonly SqlConfiguration _sqlConfiguration;

		public RoleStore(IdentityErrorDescriber describer, IStoreProvider connectionProvider) : base(describer)
		{
			_storeProvider = connectionProvider;
			_sqlConfiguration = connectionProvider.SqlConfiguration;
		}

		// Dapper 不可能实现这个
		public override IQueryable<TRole> Roles => throw new NotImplementedException();

		public override async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _storeProvider.Create())
			{
				var roleClaim = Activator.CreateInstance<TRoleClaim>();
				roleClaim.RoleId = role.Id;
				roleClaim.ClaimType = claim.Type;
				roleClaim.ClaimValue = claim.Value;

				await conn.ExecuteAsync(_sqlConfiguration.AddRoleClaim, new { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
			}
		}

		public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.CreateRole, role);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.DeleteRoleById, role);
				return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
			}
		}

		public override async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(roleId))
				throw new ArgumentNullException(nameof(roleId));

			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TRole>(_sqlConfiguration.FindRoleById, new { Id = roleId });
			}
		}

		public override async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(normalizedRoleName))
				throw new ArgumentNullException(nameof(normalizedRoleName));
			using (var conn = _storeProvider.Create())
			{
				return await conn.QueryFirstOrDefaultAsync<TRole>(_sqlConfiguration.FindRoleByNormalizedName, new { NormalizedName = normalizedRoleName });
			}
		}

		public override async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _storeProvider.Create())
			{
				var roleClaims = await conn.QueryAsync(_sqlConfiguration.FindRoleClaimsByRoleId, new { RoleId = role.Id });
				var results = roleClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
				return results;
			}
		}

		public override async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.NormalizedName);
		}

		public override async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.Id.ToString());
		}

		public override async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Task.FromResult(role.Name);
		}

		public override async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			using (var conn = _storeProvider.Create())
			{
				var paramters = new { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
				await conn.ExecuteAsync(_sqlConfiguration.RemoveRoleClaimsByRoleIdAndClaim, paramters);
			}
		}

		public override async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (string.IsNullOrWhiteSpace(normalizedName))
				throw new ArgumentNullException(nameof(normalizedName));

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.SetNormalizedRoleNameById, new { role.Id, NormalizedName = normalizedName });
			}
		}

		public override async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException(nameof(roleName));

			using (var conn = _storeProvider.Create())
			{
				await conn.ExecuteAsync(_sqlConfiguration.SetRoleNameById, new { role.Id, Name = roleName });
			}
		}

		public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (var conn = _storeProvider.Create())
			{
				var result = await conn.ExecuteAsync(_sqlConfiguration.UpdateRole, role);
				return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Update user failed." });
			}
		}
	}
}

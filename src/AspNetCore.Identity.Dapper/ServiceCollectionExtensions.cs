using AspNetCore.Identity.Dapper.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Data;
using System.Reflection;

namespace AspNetCore.Identity.Dapper
{
	public static class ServiceCollectionExtensions
	{
		public static IdentityBuilder AddDapperStores(this IdentityBuilder builder)
		{
			var userType = builder.UserType;
			var roleType = builder.RoleType;

			var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
			if (userType == null)
			{
				throw new InvalidOperationException($"{nameof(userType)} is not a IdentityUser.");
			}

			builder.Services.AddSingleton(new SqlConfiguration(""));
			var keyType = identityUserType.GenericTypeArguments[0];
			var userClaimType = typeof(IdentityUserClaim<>).MakeGenericType(keyType);
			var userRoleType = typeof(IdentityUserRole<>).MakeGenericType(keyType);
			var userLoginType = typeof(IdentityUserLogin<>).MakeGenericType(keyType);
			var userTokenType = typeof(IdentityUserToken<>).MakeGenericType(keyType);
			var roleClaimType = typeof(IdentityRoleClaim<>).MakeGenericType(keyType);
			AddStores(builder.Services, builder.UserType, builder.RoleType, keyType, userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType);
			return builder;
		}

		private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type keyType,
			Type userClaimType,
			Type userRoleType, Type userLoginType, Type userTokenType, Type roleClaimType)
		{
			if (roleType != null)
			{
				var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
				if (identityRoleType == null)
				{
					throw new InvalidOperationException($"{nameof(userType)} is not a IdentityRole.");
				}

				Type userStoreType = null;
				Type roleStoreType = null;

				userStoreType = typeof(UserStore<,,,,,,,>).MakeGenericType(userType,
					identityRoleType,
					keyType, userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType);

				roleStoreType = typeof(RoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType);

				services.AddScoped<IConnectionProvider, SqlServerConnectionProvider>();
				services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
				services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
			}
			else
			{
				// No Roles
				//	Type userStoreType = null;
				//	var connectionType = FindGenericBaseType(connection, typeof(connection));
				//	if (connectionType == null)
				//	{
				//		// If its a custom DbContext, we can only add the default POCOs
				//		userStoreType = typeof(UserOnlyStore<,,>).MakeGenericType(userType, connection, keyType);
				//	}
				//	else
				//	{
				//		userStoreType = typeof(UserOnlyStore<,,,,,>).MakeGenericType(userType, connection,
				//			connectionType.GenericTypeArguments[1],
				//			connectionType.GenericTypeArguments[2],
				//			connectionType.GenericTypeArguments[3],
				//			connectionType.GenericTypeArguments[4]);
				//	}
				//services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
			}
		}

		private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
		{
			TypeInfo typeInfo = currentType.GetTypeInfo();
			while (typeInfo.BaseType != (Type)null)
			{
				typeInfo = typeInfo.BaseType.GetTypeInfo();
				Type left = typeInfo.IsGenericType ? typeInfo.GetGenericTypeDefinition() : null;
				if (left != (Type)null && left == genericBaseType)
				{
					return typeInfo;
				}
			}
			return null;
		}
	}
}

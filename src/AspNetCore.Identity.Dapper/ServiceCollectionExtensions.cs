using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using AspNetCore.Identity.Dapper.Stores;

namespace AspNetCore.Identity.Dapper
{
	public static class ServiceCollectionExtensions
	{
		public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, IStoreProvider storeProvider)
		{
			var userType = builder.UserType;
			var roleType = builder.RoleType;

			storeProvider.SqlConfiguration.InitUserSql(userType);

			builder.Services.AddSingleton(storeProvider);

			var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
			if (identityUserType == null)
			{
				throw new InvalidOperationException($"{nameof(userType)} is not a IdentityUser.");
			}

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
			Type userRoleType,
			Type userLoginType,
			Type userTokenType,
			Type roleClaimType)
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

				services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
				services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
			}
			else
			{
				//No Roles
				Type userStoreType = null;

				userStoreType = typeof(UserOnlyStore<,,,,>).MakeGenericType(userType,
					keyType, userClaimType, userLoginType, userTokenType);

				services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
			}
		}

		private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
		{
			TypeInfo typeInfo = currentType.GetTypeInfo();
			while (typeInfo.BaseType != null)
			{
				typeInfo = typeInfo.BaseType.GetTypeInfo();
				Type left = typeInfo.IsGenericType ? typeInfo.GetGenericTypeDefinition() : null;
				if (left != null && left == genericBaseType)
				{
					return typeInfo;
				}
			}
			return null;
		}
	}
}

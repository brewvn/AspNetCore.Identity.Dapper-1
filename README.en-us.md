### AspNetCore.Identity.Dapper　　　　　　　　　　　　　　　　　　[嶄猟](README.md)

There is an official identity provider used by EF from Microsfot, but some reasons we want to query sql directly instead of EF. This project is used to change ORM from EF to Dapper with a little impact.

#### How to run the sample

- Install SqlExpress or SqlServer, change the connection string in appsettings.Development.json if you want to test via SqlServer
- Set AspNetCore.Identity.Dapper.Samples.Web as the startup project
- Run command: update-database in Package Mananger Console
- Press F5 to have fun

#### How to run testcases

- Execute step 1,2,3 for "How to run the sample" to make sure the database is ready
- Run all testcases

#### If you want to use other database

- Implement a IStoreProvider used to create DbConnection, FOLLOW: https://github.com/dotnet-china/AspNetCore.Identity.Dapper/blob/master/src/AspNetCore.Identity.Dapper/SqlServerProvider.cs
- Implement SqlConfiguration used to config sql, forexample your need: MySqlSqlConfiguraiton

#### Proposal

- I hope your user entity inherit from IdentityUser means the primary key is string(GUID), don't inherit from IdentityUser<>
- If your user entity's primary key is int or long, there is a contract that the primary key is auto increase, if you want implement a other database provider, please make sure your insert sql ignore the id column
- Only support extend identityuser(add, remove columns), InitUserSql method in SqlConfiguraiton can generate your user columns's sql. I think Role, UserClaims are no need to extend?
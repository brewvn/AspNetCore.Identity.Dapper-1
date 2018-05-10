### AspNetCore.Identity.Dapper

微软实现了 EF 的 ASP.NET Identity providers, 在某些特定场景下, 需要使用纯 SQL 替代 EF, 此项目的目标就是实现 EF providers 和 Dapper providers 的无缝切换, 而不需要做太大的改动。

#### 运行项目的方法

- 安装 SqlExpress 或者 SqlServer, 如果想使用 SqlServer 请在配置文件中修改连接字符串
- 设置 AspNetCore.Identity.Dapper.Samples.Web 为启动项目
- 在 Package Mananger Console 中运行 update-database
- 运行项目

#### 运行测试项目的

- 执行"运行项目方法"的前3步
- 运行测试用例

#### 扩展至其它数据库的方法

- 实现 IStoreProvider 用于创建 DbConnection, 参考 https://github.com/dotnet-china/AspNetCore.Identity.Dapper/blob/master/src/AspNetCore.Identity.Dapper/SqlServerProvider.cs
- 继承实现 SqlConfiguration 用于提供对应的SQL语句, 如: MySqlSqlConfiguraiton

#### 说明与建议

- Identity 相关实体主键建议使用默认的 string 即 Guid
- Identity 相关实体主键为 int 或 long 时, 约定主键为自增, 在生成 insert sql 语句时注意忽略 Id
- 仅支持 IdentityUser 扩展(添加、删除字段), SqlConfiguration 的 InitUserSql 方法实现的对列名的动态化, Role、UserClaims 之类的表没有扩展的必要?
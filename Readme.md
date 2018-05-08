### AspNetCore.Identity.Dapper

微软实现了 EF 的 Identity, 在某些场景下, EF 不能够满足业务需求, 因此使用纯 SQL 会是一个不错的选择。此项目的目标是实现 EF 和 Dapper的无缝切换, 而不需要做任何改动。

#### 运行项目的方法

- 安装 SqlExpress
- 设置 AspNetCore.Identity.Dapper.Samples.Web 为启动项目
- 在 Package Mananger Console中运行 update-database
- 运行项目

#### 运行测试项目的

- 执行"运行项目方法"的前3步
- 运行测试用例

#### 扩展至其它数据库的方法

- 实现 IStoreProvider 用于创建 DbConnection, 参考 https://github.com/dotnet-china/AspNetCore.Identity.Dapper/blob/master/src/AspNetCore.Identity.Dapper/SqlServerProvider.cs
- 继承实现 SqlConfiguration 用于提供对应的SQL语句, 如: MySqlSqlConfiguraiton

#### 说明与建议

- Identity相关实体主键建议使用默认的string即Guid
- Identity相关实体主键为int或long时, 约定主键为自增, 在生成insert Sql语句时注意忽略 id
- 仅支持 IdentityUser 扩展(添加、删除字段), SqlConfiguration 的 InitUserSql方法实现的对列名的动态化, Role、UserClaims之类的表没有扩展的必要?
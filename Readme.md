### AspNetCore.Identity.Dapper

微软实现了EF的Identity, 在某些对性能有要求的场景下, 使用Dapper会是一个更好的选择。此项目的目标是实现EF和Dapper的无缝切换, 而不需要做任何改动。

#### 运行项目的方法

- 安装 SqlExpress
- 设置 AspNetCore.Identity.Dapper.Samples.Web 为启动项目
- 在Startup中删除AddEntityFrameworkStores的注释后, 注释AddDapperStores
- 在 Package Mananger Console中运行 update-database
- 在Startup中注释AddEntityFrameworkStores, 删除AddDapperStores的注释
- 运行项目

#### 运行测试项目的

- 执行"运行项目方法"的前4步
- 运行测试用例

#### 注意与建议

- Identity相关实体主键建议使用默认的string即Guid
- Identity相关实体主键为int或long时, 约定主键为自增, 在生成insert Sql语句时注意忽略 id
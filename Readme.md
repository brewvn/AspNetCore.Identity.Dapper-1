### AspNetCore.Identity.Dapper������������������������������������[English](README.en-us.md)

΢��ʵ���� EF �� ASP.NET Identity providers, ��ĳЩ�ض�������, ��Ҫʹ�ô� SQL ��� EF, ����Ŀ��Ŀ�����ʵ�� EF providers �� Dapper providers ���޷��л�, ������Ҫ��̫��ĸĶ���

#### ������Ŀ�ķ���

- ��װ SqlExpress ���� SqlServer, �����ʹ�� SqlServer ���������ļ����޸������ַ���
- ���� AspNetCore.Identity.Dapper.Samples.Web Ϊ������Ŀ
- �� Package Mananger Console ������ update-database
- ������Ŀ

#### ʹ�÷���

			services.AddIdentity<ApplicationUser, IdentityRole>(x =>
			{
				x.Password.RequireUppercase = false;
				x.Password.RequireNonAlphanumeric = false;
			})
			//.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDapperStores(new SqlServerProvider(Configuration.GetConnectionString("DefaultConnection")))

#### ���в�����Ŀ��

- ִ��"������Ŀ����"��ǰ3��
- ���в�������

#### ��չ���������ݿ�ķ���

- ʵ�� IStoreProvider ���ڴ��� DbConnection, �ο� https://github.com/dotnet-china/AspNetCore.Identity.Dapper/blob/master/src/AspNetCore.Identity.Dapper/SqlServerProvider.cs
- �̳�ʵ�� SqlConfiguration �����ṩ��Ӧ��SQL���, ��: MySqlSqlConfiguraiton

#### ˵���뽨��

- Identity ���ʵ����������ʹ��Ĭ�ϵ� string �� Guid
- Identity ���ʵ������Ϊ int �� long ʱ, Լ������Ϊ����, ������ insert sql ���ʱע����� Id
- ��֧�� IdentityUser ��չ(��ӡ�ɾ���ֶ�), SqlConfiguration �� InitUserSql ����ʵ�ֵĶ������Ķ�̬��, Role��UserClaims ֮��ı�û����չ�ı�Ҫ?
### AspNetCore.Identity.Dapper

΢��ʵ���� EF �� Identity, ��ĳЩ������, EF ���ܹ�����ҵ������, ���ʹ�ô� SQL ����һ�������ѡ�񡣴���Ŀ��Ŀ����ʵ�� EF �� Dapper���޷��л�, ������Ҫ���κθĶ���

#### ������Ŀ�ķ���

- ��װ SqlExpress
- ���� AspNetCore.Identity.Dapper.Samples.Web Ϊ������Ŀ
- �� Package Mananger Console������ update-database
- ������Ŀ

#### ���в�����Ŀ��

- ִ��"������Ŀ����"��ǰ3��
- ���в�������

#### ��չ���������ݿ�ķ���

- ʵ�� IStoreProvider ���ڴ��� DbConnection, �ο� https://github.com/dotnet-china/AspNetCore.Identity.Dapper/blob/master/src/AspNetCore.Identity.Dapper/SqlServerProvider.cs
- �̳�ʵ�� SqlConfiguration �����ṩ��Ӧ��SQL���, ��: MySqlSqlConfiguraiton

#### ˵���뽨��

- Identity���ʵ����������ʹ��Ĭ�ϵ�string��Guid
- Identity���ʵ������Ϊint��longʱ, Լ������Ϊ����, ������insert Sql���ʱע����� id
- ��֧�� IdentityUser ��չ(��ӡ�ɾ���ֶ�), SqlConfiguration �� InitUserSql����ʵ�ֵĶ������Ķ�̬��, Role��UserClaims֮��ı�û����չ�ı�Ҫ?
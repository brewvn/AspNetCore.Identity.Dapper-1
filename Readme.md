### AspNetCore.Identity.Dapper

΢��ʵ����EF��Identity, ��ĳЩ��������Ҫ��ĳ�����, ʹ��Dapper����һ�����õ�ѡ�񡣴���Ŀ��Ŀ����ʵ��EF��Dapper���޷��л�, ������Ҫ���κθĶ���

#### ������Ŀ�ķ���

- ��װ SqlExpress
- ���� AspNetCore.Identity.Dapper.Samples.Web Ϊ������Ŀ
- ��Startup��ɾ��AddEntityFrameworkStores��ע�ͺ�, ע��AddDapperStores
- �� Package Mananger Console������ update-database
- ��Startup��ע��AddEntityFrameworkStores, ɾ��AddDapperStores��ע��
- ������Ŀ

#### ���в�����Ŀ��

- ִ��"������Ŀ����"��ǰ4��
- ���в�������

#### ע���뽨��

- Identity���ʵ����������ʹ��Ĭ�ϵ�string��Guid
- Identity���ʵ������Ϊint��longʱ, Լ������Ϊ����, ������insert Sql���ʱע����� id
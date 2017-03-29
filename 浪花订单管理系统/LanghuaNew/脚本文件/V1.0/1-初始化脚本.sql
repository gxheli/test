
DELETE  Users
INSERT  [dbo].[Users]( UserName ,PassWord ,NickName ,LateOnLineTime)SELECT 'dingding',SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', '123')), 3,32) ,'����',GETDATE()

IF NOT EXISTS(SELECT 1 FROM ServiceTypes)
BEGIN
	SET IDENTITY_INSERT ServiceTypes ON
	DELETE  ServiceTypes
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  1 ,'��ͨ'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  2 ,'�г�'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  3 ,'��Ʊ'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  4 ,'�Ƶ�'
	SET IDENTITY_INSERT ServiceTypes OFF
END
IF NOT EXISTS(SELECT 1 FROM OrderSourses)
BEGIN
	SET IDENTITY_INSERT OrderSourses ON
	DELETE  OrderSourses
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 1, N'�Ա�' )
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 2, N'��è' )
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 3, N'ͬ�з���' )
	SET IDENTITY_INSERT OrderSourses OFF
END
IF NOT EXISTS(SELECT 1 FROM Currencies)
BEGIN
	SET IDENTITY_INSERT Currencies ON
	DELETE  Currencies
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'�����' ,1 ,N'RMB' ,1 ,1)
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'̩��' ,5 ,N'THB' ,0 ,2)
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'���' ,6 ,N'MYR' ,0 ,3)
	SET IDENTITY_INSERT Currencies OFF
END
IF NOT EXISTS(SELECT 1 FROM COUNTRIES)
BEGIN
	SET IDENTITY_INSERT COUNTRIES ON
	DELETE  COUNTRIES
	INSERT  [dbo].[COUNTRIES]( [CountryNAME] ,[CountrYENNAME] ,[CountryID])VALUES  ( N'̩��' ,N'Thailand' ,1)
	INSERT  [dbo].[COUNTRIES]( [CountryNAME] ,[CountrYENNAME] ,[CountryID])VALUES  ( N'ӡ��' ,N'Indonesia' ,2)
	SET IDENTITY_INSERT COUNTRIES OFF
	
	SET IDENTITY_INSERT Cities ON
	DELETE  Cities
	INSERT  INTO dbo.Cities( CityID ,CityName ,CityEnName ,CountryID )VALUES  ( 1 ,N'�ռ���' ,N'Phuket' ,1  )
	INSERT  INTO dbo.Cities( CityID ,CityName ,CityEnName ,CountryID )VALUES  ( 2 ,N'���嵺' ,N'Bali' ,2  )
	SET IDENTITY_INSERT Cities OFF

	SET IDENTITY_INSERT Areas ON
	DELETE  Areas
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 1 , N'�Ͷ�' ,N'Patong' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 2 ,N'����' ,N'Karon' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 3 ,N'����' ,N'Kata' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 4 ,N'����' ,N'Kuta' ,2 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 5 ,N'�׼���' ,N'Legian' ,2 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 6 , N'��Ŭ��' , N'Sanur' ,2 ,0 )
	SET IDENTITY_INSERT Areas OFF
END
IF NOT EXISTS(SELECT 1 FROM Hotals)
BEGIN
	SET IDENTITY_INSERT Hotals ON
	DELETE  Hotals
	INSERT  INTO dbo.Hotals( HotalID ,HotalName ,HotalPhone ,HotalAddress ,AreaID)VALUES ( 1 ,N'Siralanna Phuket Hotel' ,N'0066-76-364718' ,N'162/94-97 Phang Muang Sai Kor Road, Kathu, Phuket, Patong Beach' ,1 )
	INSERT  INTO dbo.Hotals( HotalID ,HotalName ,HotalPhone ,HotalAddress ,AreaID)VALUES ( 2 ,N'De Coze Hotel' ,N'0066-76-322825' ,N'162/147 Phang Muang Sai Kor RD., Kathu, Phuket, Patong Beach' ,2 )
	INSERT  INTO dbo.Hotals( HotalID ,HotalName ,HotalPhone ,HotalAddress ,AreaID)VALUES ( 3 ,N'Deevana Patong Resort & Spa' ,N'0066-76-341414' ,N'43/2 Raj-U-Thid 200 Pee Road., Patong., Kathu, Patong Beach' ,5 )
	SET IDENTITY_INSERT Hotals OFF
END
---------------------------------------------------------------------------��Ӧ��--------------------------------------------------------------------------------------
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'����' ,N'TH' ,1 ,N'test@test.com' ,N'123456789' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='TH')
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'̩��' ,N'TL' ,1 ,N'tl@test.com' ,N'90832121' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='TL')
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'һ�����' ,N'YQ' ,2 ,N'yq@test.com' ,N'777328921' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='YQ')

-------------------------------------------------------------------2016-07-09���ӽű���Ȩ�޹ܿأ�Begin------------------------------------------------------------------------

SET IDENTITY_INSERT Roles ON
DELETE Roles
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (1, N'��������Ա' ,N'SuperAdmin' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (2, N'����Ա' ,N'Admin' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (3, N'��ƷרԱ' ,N'ProductStaff' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (4, N'����Ա' ,N'Operator' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (5, N'��ǰ�ͷ�' , N'Pre-Sales' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (6, N'�ۺ�ͷ�' ,N'After-Sales' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (7, N'����' ,N'Financial' , N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (8, N'������Դ' ,N'HumanResources' ,N'' ,GETDATE())
SET IDENTITY_INSERT Roles OFF

--��dingding�˺Ÿ��賬������Ա
INSERT INTO dbo.UserRoles
        ( Role_RoleID, User_UserID )
SELECT 1,UserID FROM dbo.Users a
LEFT JOIN UserRoles b ON a.UserID = b.User_UserID
WHERE UserName='dingding' AND b.Role_RoleID IS NULL


------����ҳ��Begin-------
SET IDENTITY_INSERT RoleRights ON
DELETE dbo.RoleRights
--dbcc checkident('RoleRights',reseed,0)

INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 1,N'Home',N'Index' )
--���� 2-10
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 2,N'Orders',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 3,N'Orders',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 4,N'Orders',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 5,N'Orders',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 6,N'Orders',N'OrderFinish' )--�������ҳ
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 7,N'Orders',N'OrderFinishPost' )--�����ܽ��
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 8,N'Orders',N'OrderOperation' )--�鿴������־
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 9,N'Orders',N'ExportExcel' )--���������б�
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 10,N'Orders',N'Receive' )--�ӵ���ť
--��Ʒ 11-19
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 11,N'ServiceItems',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 12,N'ServiceItems',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 13,N'ServiceItems',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 14,N'ServiceItems',N'FormSetting' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 15,N'ServiceItems',N'PriceSetting' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 16,N'ServiceItems',N'UploadFile' )--�ϴ�ģ��
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 17,N'ServiceItems',N'DownFile' )--����ģ��
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 18,N'ServiceItems',N'FormField' )--�鿴���ֶ�Ĭ��ֵ
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 19,N'ServiceItems',N'ExportExcel' )--�����б�
--���� 20-24
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 20,N'ServiceRules',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 21,N'ServiceRules',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 22,N'ServiceRules',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 23,N'ServiceRules',N'RulesOperation' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 24,N'ServiceRules',N'ExportExcel' )--�����б�
--֧����ת�� 25-31
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 25,N'AlipayTransfers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 26,N'AlipayTransfers',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 27,N'AlipayTransfers',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 28,N'AlipayTransfers',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 29,N'AlipayTransfers',N'CheckAlipayTransfer' )--��ʵ
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 30,N'AlipayTransfers',N'TransferAlipayTransfer' )--ת��
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 31,N'AlipayTransfers',N'DeleteAlipayTransfer' )--����
--���� 32
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 32,N'TBOrderStates',N'Index' )
--��λ���۱� 33-35
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 33,N'SellControls',N'Index' )--��λ���۱�
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 34,N'SellControls',N'SaveSellControl' )--���ÿ�λ��Ʒ��ť
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 35,N'SellControls',N'SetSellControl' )--���ÿ�λ���۰�ť
--�ͻ� 36-39
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 36,N'Customers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 37,N'Customers',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 38,N'Customers',N'SetReturnList' )--���ûط�����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 39,N'Customers',N'ExportExcel' )--�����б�
--�������� 40-43
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 40,N'Countries',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 41,N'Countries',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 42,N'Countries',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 43,N'Countries',N'ExportExcel' )--�����б�
--���� 44-47
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 44,N'Currencies',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 45,N'Currencies',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 46,N'Currencies',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 47,N'Currencies',N'ExportExcel' )--�����б�
--�Ƶ� 48-51
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 48,N'Hotals',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 49,N'Hotals',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 50,N'Hotals',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 51,N'Hotals',N'ExportExcel' )--�����б�
--������Դ 52-55
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 52,N'OrderSourses',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 53,N'OrderSourses',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 54,N'OrderSourses',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 55,N'OrderSourses',N'ExportExcel' )--�����б�
--��Ӧ�� 56-59
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 56,N'Suppliers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 57,N'Suppliers',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 58,N'Suppliers',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 59,N'Suppliers',N'ExportExcel' )--�����б�
--�û� 60-65
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 60,N'Users',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 61,N'Users',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 62,N'Users',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 63,N'Users',N'ExportExcel' )--�����б�
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 64,N'Users',N'UserData' )--��������
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 65,N'Users',N'UsersOperation' )--�û���־
--����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 66,N'Insurance',N'Index' )--����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 67,N'Insurance',N'ExportExcel' )--�����б�
--����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 68,N'TBOrderStates',N'Index' )--����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 69,N'TBOrderStates',N'CheckDelivery' )--��©����
--�����ļ�
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 70,N'Orders',N'DownloadPdf' )--����PDF
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 71,N'Orders',N'execl' )--�����ó��ű�/�г��ű�
--�˵�����
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 72,N'BillReports',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 73,N'BillReports',N'DownFile' )--������ϸ

SET IDENTITY_INSERT RoleRights OFF

DELETE dbo.RoleRoleRights
--��������Ա����ȫ��Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 1,RoleRightID FROM RoleRights
--����Ա����Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,2--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,10--�ӵ���ť
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,11--��Ʒ�б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,12
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,13
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,14
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,15
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,16
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,17
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,18
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,19
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,20--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,21
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,22
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,23
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,24
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,25--֧����ת���б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,33--��λ���۱�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,34
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,35
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,38--���ûط�����
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,40--��������
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,41
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,42
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,43
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,44--����
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,45
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,46
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,47
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,56--��Ӧ�̹���
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,57
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,58
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,59
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,70
--��ƷרԱ����Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,2--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,11--��Ʒ�б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,12
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,13
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,14
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,15
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,16
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,17
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,18
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,19
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,20--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,21
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,22
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,23
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,24
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,25--֧����ת���б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,33--��λ���۱�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,34
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,35
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,56--��Ӧ�̹���
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,57
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,58
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,59
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,70
--����Ա����Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,2--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,10--�ӵ���ť
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,25--֧����ת���б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,33--��λ���۱�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,66
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,67
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,68
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,69
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,70
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,71
--��ǰ�ͷ�����Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,2--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,25--֧����ת���б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,31
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,33--��λ���۱�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,70
--�ۺ�ͷ�����Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,2--�����б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,25--֧����ת���б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,29--��ʵ
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,31--����
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,33--��λ���۱�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,38
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,66
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,67
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,70
--������Ȩ��
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,2
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,25
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,30
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,31
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,36--�ͻ��б�
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,70
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,72
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,73
--������Դ
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,60
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,61
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,62
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,63
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,65
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,70


------����ҳ��End---------
-------------------------------------------------------------------2016-07-09���ӽű���Ȩ�޹ܿأ�End------------------------------------------------------------------------


--------------------------------------------------------------------------------�ֶ�-����-����-ע��Begin---------------------------------------------------------------------------
DELETE dbo.FormFields
dbcc checkident('FormFields',reseed,0)
-------------------ϵͳ�ֶ�------------------
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'�г̹�˾',N'TripCompany',N'ϵͳ�ֶ�','Test Company')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'�ź�',N'VoucherNo',N'ϵͳ�ֶ�','JY0809-01A')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'���ؽ���ʱ��',N'PickupTime',N'ϵͳ�ֶ�','09:00-09:15')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ������',N'cnName',N'ϵͳ�ֶ�','����')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ������ƴ��',N'enName',N'ϵͳ�ֶ�','ZHANG SAN')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ�˵绰',N'Tel',N'ϵͳ�ֶ�','13812345678')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ�˱��õ绰',N'BakTel',N'ϵͳ�ֶ�','13812345679')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ�˵�������',N'Email',N'ϵͳ�ֶ�','test@test.com')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ϵ��΢��',N'Wechat',N'ϵͳ�ֶ�','wexinid')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'�Ա�ID',N'TBID',N'ϵͳ�ֶ�','���ֵ��˻�')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'Ԥ����ĿӢ������',N'enProductName',N'ϵͳ�ֶ�','Racha Island day tour')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'Ԥ����Ŀ��������',N'cnProductName',N'ϵͳ�ֶ�','�ʵ۵�һ����')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'�������Ӣ������',N'enAttachedItem',N'ϵͳ�ֶ�','Diving Once 1�ˣ�Diving Twice 1��')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'���������������',N'cnAttachedItem',N'ϵͳ�ֶ�','��Ǳһ�� 1�ˣ���Ǳ���� 1��')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'Ԥ����Ŀ����',N'ProductCode',N'ϵͳ�ֶ�','#101')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��������',N'Adult',N'ϵͳ�ֶ�','3')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ͯ����',N'Child',N'ϵͳ�ֶ�','2')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'Ӥ������',N'Infant',N'ϵͳ�ֶ�','1')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��ע',N'Remark',N'ϵͳ�ֶ�','��ע����')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'����',N'Nights',N'ϵͳ�ֶ�','1')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'����',N'NoOfRoom',N'ϵͳ�ֶ�','2')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'��Ӧ����ϵ��ʽ',N'SupplyTel',N'ϵͳ�ֶ�','093-5829781 С��')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'ϵͳ������',N'LHOrderNo',N'ϵͳ�ֶ�','201609240943331188')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'�Ա�������',N'TBOrderNo',N'ϵͳ�ֶ�','201609240943331188')

------------���ֶ�------------------
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelArea',N'�����Ƶ�����',N'PickupHotelArea',N'������ѡ��','�Ͷ�')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelName',N'�����Ƶ�����',N'PickupHotelName',N'��д�Ƶ�Ӣ��ȫ������Ҫ��ַ����Ҫ���ģ�','Siralanna Phuket Hotel')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelTel',N'�����Ƶ�绰',N'PickupHotelTel',N'','0066-76-364718')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelAddress',N'�����Ƶ��ַ',N'PickupHotelAddress',N'����Ӣ����д','162/94-97 Phang Muang Sai Kor Road, Kathu, Phuket, Patong Beach')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelArea',N'���ؾƵ�����',N'ReturnHotelArea',N'������ѡ��','�Ͷ�')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelName',N'���ؾƵ�����',N'ReturnHotelName',N'��д�Ƶ�Ӣ��ȫ������Ҫ��ַ����Ҫ���ģ�','Siralanna Phuket Hotel')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelTel',N'���ؾƵ�绰',N'ReturnHotelTel',N'','0066-76-364718')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelAddress',N'���ؾƵ��ַ',N'ReturnHotelAddress',N'����Ӣ����д','162/94-97 Phang Muang Sai Kor Road, Kathu, Phuket, Patong Beach')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList',N'�μ��г̵Ŀ���',N'MemberList',N'','ZHAO YUAN(G51885176), LIU WEN JIA(E79023583)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_1',N'�μ��г̵Ŀ���',N'MemberList_1',N'','ZHAO YUAN(G51885176,1980-01-01), LIU WEN JIA(E79023583,1980-03-01)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_2',N'�μ��г̵Ŀ���',N'MemberList_2',N'','����(ZHAO YUAN,G51885176,1980-01-01), ���ļ�(LIU WEN JIA,E79023583,1980-03-01)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_3',N'�μ��г̵Ŀ���',N'MemberList_3',N'','ZHAO YUAN(G51885176,1980-01-01,F), LIU WEN JIA(E79023583,1980-03-01,M)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'DivingMember',N'�μ���Ǳ�Ŀ���',N'DivingMember',N'','LIANRUIQING, 173cm, 66kg, 42<br/>LIANRUILIANG, 171cm, 50kg, 41<br/>WANGHAITANG, 171cm, 60kg, 40')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ServiceTime',N'����ʱ��',N'ServiceTime',N'Ӫҵʱ��09:00-20:00','16:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'DepartureTime',N'�������ʱ��',N'DepartureTime',N'ֱ�����Ʊ�ϵ�ʱ�䣬ǧ��Ҫ���κ�ʱ�任��','2016-01-01 10:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ArrivalTime',N'���ൽ��ʱ��',N'ArrivalTime',N'ֱ�����Ʊ�ϵ�ʱ�䣬ǧ��Ҫ���κ�ʱ�任��','2016-01-01 16:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupTime',N'ȥ�Ƶ����ʱ��',N'PickTime',N'Ϊ��֤������������Ҫ��ǰ3Сʱ����','2016-01-01 07:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'FlightNo',N'�����',N'FlightNo',N'','CZ3869')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ServiceDate',N'��������',N'ServiceDate',N'','2016-01-01')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'BackDate',N'��������',N'BackDate',N'','2016-01-01')

----------------------------------------------------------------------�ֶ�-����-����-ע��End---------------------------------------------------------------------------



DELETE  Users
INSERT  [dbo].[Users]( UserName ,PassWord ,NickName ,LateOnLineTime)SELECT 'dingding',SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', '123')), 3,32) ,'丁丁',GETDATE()

IF NOT EXISTS(SELECT 1 FROM ServiceTypes)
BEGIN
	SET IDENTITY_INSERT ServiceTypes ON
	DELETE  ServiceTypes
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  1 ,'交通'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  2 ,'行程'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  3 ,'门票'
	INSERT  ServiceTypes( ServiceTypeID, ServiceTypeName )SELECT  4 ,'酒店'
	SET IDENTITY_INSERT ServiceTypes OFF
END
IF NOT EXISTS(SELECT 1 FROM OrderSourses)
BEGIN
	SET IDENTITY_INSERT OrderSourses ON
	DELETE  OrderSourses
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 1, N'淘宝' )
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 2, N'天猫' )
	INSERT  INTO dbo.OrderSourses( OrderSourseID, OrderSourseName )VALUES  ( 3, N'同行分销' )
	SET IDENTITY_INSERT OrderSourses OFF
END
IF NOT EXISTS(SELECT 1 FROM Currencies)
BEGIN
	SET IDENTITY_INSERT Currencies ON
	DELETE  Currencies
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'人民币' ,1 ,N'RMB' ,1 ,1)
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'泰铢' ,5 ,N'THB' ,0 ,2)
	INSERT  [dbo].[Currencies]( [CurrencyName] ,[ExchangeRate] ,[CurrencyNo] ,[CurrencyChangeType] ,[CurrencyID])VALUES  ( N'马币' ,6 ,N'MYR' ,0 ,3)
	SET IDENTITY_INSERT Currencies OFF
END
IF NOT EXISTS(SELECT 1 FROM COUNTRIES)
BEGIN
	SET IDENTITY_INSERT COUNTRIES ON
	DELETE  COUNTRIES
	INSERT  [dbo].[COUNTRIES]( [CountryNAME] ,[CountrYENNAME] ,[CountryID])VALUES  ( N'泰国' ,N'Thailand' ,1)
	INSERT  [dbo].[COUNTRIES]( [CountryNAME] ,[CountrYENNAME] ,[CountryID])VALUES  ( N'印尼' ,N'Indonesia' ,2)
	SET IDENTITY_INSERT COUNTRIES OFF
	
	SET IDENTITY_INSERT Cities ON
	DELETE  Cities
	INSERT  INTO dbo.Cities( CityID ,CityName ,CityEnName ,CountryID )VALUES  ( 1 ,N'普吉岛' ,N'Phuket' ,1  )
	INSERT  INTO dbo.Cities( CityID ,CityName ,CityEnName ,CountryID )VALUES  ( 2 ,N'巴厘岛' ,N'Bali' ,2  )
	SET IDENTITY_INSERT Cities OFF

	SET IDENTITY_INSERT Areas ON
	DELETE  Areas
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 1 , N'巴东' ,N'Patong' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 2 ,N'卡伦' ,N'Karon' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 3 ,N'卡塔' ,N'Kata' ,1 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 4 ,N'库塔' ,N'Kuta' ,2 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 5 ,N'雷吉安' ,N'Legian' ,2 ,0 )
	INSERT  INTO dbo.Areas( AreaID ,AreaName , AreaEnName ,CityID ,AreaEnableState)VALUES  ( 6 , N'萨努尔' , N'Sanur' ,2 ,0 )
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
---------------------------------------------------------------------------供应商--------------------------------------------------------------------------------------
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'佳运' ,N'TH' ,1 ,N'test@test.com' ,N'123456789' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='TH')
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'泰利' ,N'TL' ,1 ,N'tl@test.com' ,N'90832121' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='TL')
INSERT  INTO dbo.Suppliers( SupplierName ,SupplierNo ,CountryID ,EMail ,ContactWay ,SupplierEnableState ,SupplierSysName ,SupplierPWD)
SELECT N'一起溜达' ,N'YQ' ,2 ,N'yq@test.com' ,N'777328921' ,0 ,N'' ,N'' WHERE NOT EXISTS(SELECT 1 FROM Suppliers WHERE SupplierNo='YQ')

-------------------------------------------------------------------2016-07-09增加脚本（权限管控）Begin------------------------------------------------------------------------

SET IDENTITY_INSERT Roles ON
DELETE Roles
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (1, N'超级管理员' ,N'SuperAdmin' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (2, N'管理员' ,N'Admin' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (3, N'产品专员' ,N'ProductStaff' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (4, N'操作员' ,N'Operator' ,N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (5, N'售前客服' , N'Pre-Sales' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (6, N'售后客服' ,N'After-Sales' ,N'' , GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (7, N'财务' ,N'Financial' , N'' ,GETDATE())
INSERT INTO dbo.Roles( RoleID,RoleName ,RoleEnName ,RoleRemark ,CreateTime)VALUES (8, N'人力资源' ,N'HumanResources' ,N'' ,GETDATE())
SET IDENTITY_INSERT Roles OFF

--给dingding账号赋予超级管理员
INSERT INTO dbo.UserRoles
        ( Role_RoleID, User_UserID )
SELECT 1,UserID FROM dbo.Users a
LEFT JOIN UserRoles b ON a.UserID = b.User_UserID
WHERE UserName='dingding' AND b.Role_RoleID IS NULL


------增加页面Begin-------
SET IDENTITY_INSERT RoleRights ON
DELETE dbo.RoleRights
--dbcc checkident('RoleRights',reseed,0)

INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 1,N'Home',N'Index' )
--订单 2-10
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 2,N'Orders',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 3,N'Orders',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 4,N'Orders',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 5,N'Orders',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 6,N'Orders',N'OrderFinish' )--订单完成页
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 7,N'Orders',N'OrderFinishPost' )--保存总金额
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 8,N'Orders',N'OrderOperation' )--查看操作日志
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 9,N'Orders',N'ExportExcel' )--导出订单列表
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 10,N'Orders',N'Receive' )--接单按钮
--产品 11-19
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 11,N'ServiceItems',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 12,N'ServiceItems',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 13,N'ServiceItems',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 14,N'ServiceItems',N'FormSetting' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 15,N'ServiceItems',N'PriceSetting' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 16,N'ServiceItems',N'UploadFile' )--上传模板
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 17,N'ServiceItems',N'DownFile' )--下载模板
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 18,N'ServiceItems',N'FormField' )--查看表单字段默认值
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 19,N'ServiceItems',N'ExportExcel' )--导出列表
--规则 20-24
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 20,N'ServiceRules',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 21,N'ServiceRules',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 22,N'ServiceRules',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 23,N'ServiceRules',N'RulesOperation' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 24,N'ServiceRules',N'ExportExcel' )--导出列表
--支付宝转账 25-31
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 25,N'AlipayTransfers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 26,N'AlipayTransfers',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 27,N'AlipayTransfers',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 28,N'AlipayTransfers',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 29,N'AlipayTransfers',N'CheckAlipayTransfer' )--核实
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 30,N'AlipayTransfers',N'TransferAlipayTransfer' )--转账
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 31,N'AlipayTransfers',N'DeleteAlipayTransfer' )--作废
--发货 32
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 32,N'TBOrderStates',N'Index' )
--控位销售表 33-35
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 33,N'SellControls',N'Index' )--控位销售表
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 34,N'SellControls',N'SaveSellControl' )--配置控位产品按钮
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 35,N'SellControls',N'SetSellControl' )--设置控位销售按钮
--客户 36-39
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 36,N'Customers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 37,N'Customers',N'Details' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 38,N'Customers',N'SetReturnList' )--配置回访名单
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 39,N'Customers',N'ExportExcel' )--导出列表
--国家区域 40-43
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 40,N'Countries',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 41,N'Countries',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 42,N'Countries',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 43,N'Countries',N'ExportExcel' )--导出列表
--货币 44-47
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 44,N'Currencies',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 45,N'Currencies',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 46,N'Currencies',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 47,N'Currencies',N'ExportExcel' )--导出列表
--酒店 48-51
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 48,N'Hotals',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 49,N'Hotals',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 50,N'Hotals',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 51,N'Hotals',N'ExportExcel' )--导出列表
--订单来源 52-55
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 52,N'OrderSourses',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 53,N'OrderSourses',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 54,N'OrderSourses',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 55,N'OrderSourses',N'ExportExcel' )--导出列表
--供应商 56-59
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 56,N'Suppliers',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 57,N'Suppliers',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 58,N'Suppliers',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 59,N'Suppliers',N'ExportExcel' )--导出列表
--用户 60-65
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 60,N'Users',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 61,N'Users',N'Create' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 62,N'Users',N'Edit' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 63,N'Users',N'ExportExcel' )--导出列表
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 64,N'Users',N'UserData' )--个人资料
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 65,N'Users',N'UsersOperation' )--用户日志
--保险
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 66,N'Insurance',N'Index' )--保险
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 67,N'Insurance',N'ExportExcel' )--导出列表
--发货
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 68,N'TBOrderStates',N'Index' )--发货
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 69,N'TBOrderStates',N'CheckDelivery' )--查漏发货
--下载文件
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 70,N'Orders',N'DownloadPdf' )--下载PDF
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 71,N'Orders',N'execl' )--下载用车团表/行程团表
--账单报表
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 72,N'BillReports',N'Index' )
INSERT INTO dbo.RoleRights( RoleRightID, ControllerName, ActionName )VALUES  ( 73,N'BillReports',N'DownFile' )--下载明细

SET IDENTITY_INSERT RoleRights OFF

DELETE dbo.RoleRoleRights
--超级管理员赋予全部权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 1,RoleRightID FROM RoleRights
--管理员赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,2--订单列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,10--接单按钮
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,11--产品列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,12
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,13
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,14
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,15
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,16
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,17
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,18
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,19
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,20--规则列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,21
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,22
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,23
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,24
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,25--支付宝转账列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,33--控位销售表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,34
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,35
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,38--配置回访名单
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,40--国家区域
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,41
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,42
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,43
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,44--货币
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,45
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,46
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,47
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,56--供应商管理
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,57
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,58
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,59
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 2,70
--产品专员赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,2--订单列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,11--产品列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,12
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,13
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,14
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,15
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,16
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,17
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,18
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,19
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,20--规则列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,21
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,22
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,23
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,24
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,25--支付宝转账列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,33--控位销售表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,34
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,35
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,56--供应商管理
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,57
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,58
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,59
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 3,70
--操作员赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,2--订单列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,10--接单按钮
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,25--支付宝转账列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,33--控位销售表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,66
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,67
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,68
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,69
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,70
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 4,71
--售前客服赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,2--订单列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,25--支付宝转账列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,31
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,33--控位销售表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 5,70
--售后客服赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,2--订单列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,3
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,4
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,5
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,6
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,7
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,8
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,25--支付宝转账列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,29--核实
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,31--作废
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,33--控位销售表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,38
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,66
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,67
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 6,70
--财务赋予权限
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,2
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,9
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,25
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,26
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,27
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,28
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,30
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,31
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,36--客户列表
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,37
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,39
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,70
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,72
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 7,73
--人力资源
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,1
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,60
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,61
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,62
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,63
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,64
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,65
INSERT INTO dbo.RoleRoleRights( Role_RoleID ,RoleRight_RoleRightID) SELECT 8,70


------增加页面End---------
-------------------------------------------------------------------2016-07-09增加脚本（权限管控）End------------------------------------------------------------------------


--------------------------------------------------------------------------------字段-代号-名称-注解Begin---------------------------------------------------------------------------
DELETE dbo.FormFields
dbcc checkident('FormFields',reseed,0)
-------------------系统字段------------------
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'行程公司',N'TripCompany',N'系统字段','Test Company')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'团号',N'VoucherNo',N'系统字段','JY0809-01A')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'当地接人时间',N'PickupTime',N'系统字段','09:00-09:15')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人姓名',N'cnName',N'系统字段','张三')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人姓名拼音',N'enName',N'系统字段','ZHANG SAN')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人电话',N'Tel',N'系统字段','13812345678')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人备用电话',N'BakTel',N'系统字段','13812345679')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人电子邮箱',N'Email',N'系统字段','test@test.com')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'联系人微信',N'Wechat',N'系统字段','wexinid')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'淘宝ID',N'TBID',N'系统字段','欢乐的浪花')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'预定项目英文名称',N'enProductName',N'系统字段','Racha Island day tour')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'预定项目中文名称',N'cnProductName',N'系统字段','皇帝岛一日游')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'额外服务英文名称',N'enAttachedItem',N'系统字段','Diving Once 1人，Diving Twice 1人')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'额外服务中文名称',N'cnAttachedItem',N'系统字段','深潜一次 1人，深潜二次 1人')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'预定项目编码',N'ProductCode',N'系统字段','#101')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'成人数量',N'Adult',N'系统字段','3')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'儿童数量',N'Child',N'系统字段','2')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'婴儿数量',N'Infant',N'系统字段','1')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'备注',N'Remark',N'系统字段','备注内容')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'晚数',N'Nights',N'系统字段','1')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'间数',N'NoOfRoom',N'系统字段','2')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'供应商联系方式',N'SupplyTel',N'系统字段','093-5829781 小玉')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'系统订单号',N'LHOrderNo',N'系统字段','201609240943331188')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'systemfield',N'淘宝订单号',N'TBOrderNo',N'系统字段','201609240943331188')

------------表单字段------------------
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelArea',N'出发酒店区域',N'PickupHotelArea',N'请认真选择','巴东')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelName',N'出发酒店名称',N'PickupHotelName',N'请写酒店英文全名，不要地址！不要中文！','Siralanna Phuket Hotel')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelTel',N'出发酒店电话',N'PickupHotelTel',N'','0066-76-364718')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupHotelAddress',N'出发酒店地址',N'PickupHotelAddress',N'请用英文填写','162/94-97 Phang Muang Sai Kor Road, Kathu, Phuket, Patong Beach')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelArea',N'返回酒店区域',N'ReturnHotelArea',N'请认真选择','巴东')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelName',N'返回酒店名称',N'ReturnHotelName',N'请写酒店英文全名，不要地址！不要中文！','Siralanna Phuket Hotel')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelTel',N'返回酒店电话',N'ReturnHotelTel',N'','0066-76-364718')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ReturnHotelAddress',N'返回酒店地址',N'ReturnHotelAddress',N'请用英文填写','162/94-97 Phang Muang Sai Kor Road, Kathu, Phuket, Patong Beach')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList',N'参加行程的客人',N'MemberList',N'','ZHAO YUAN(G51885176), LIU WEN JIA(E79023583)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_1',N'参加行程的客人',N'MemberList_1',N'','ZHAO YUAN(G51885176,1980-01-01), LIU WEN JIA(E79023583,1980-03-01)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_2',N'参加行程的客人',N'MemberList_2',N'','赵媛(ZHAO YUAN,G51885176,1980-01-01), 刘文佳(LIU WEN JIA,E79023583,1980-03-01)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'MemberList_3',N'参加行程的客人',N'MemberList_3',N'','ZHAO YUAN(G51885176,1980-01-01,F), LIU WEN JIA(E79023583,1980-03-01,M)')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'DivingMember',N'参加深潜的客人',N'DivingMember',N'','LIANRUIQING, 173cm, 66kg, 42<br/>LIANRUILIANG, 171cm, 50kg, 41<br/>WANGHAITANG, 171cm, 60kg, 40')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ServiceTime',N'服务时间',N'ServiceTime',N'营业时间09:00-20:00','16:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'DepartureTime',N'航班起飞时间',N'DepartureTime',N'直接填机票上的时间，千万不要做任何时间换算','2016-01-01 10:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ArrivalTime',N'航班到达时间',N'ArrivalTime',N'直接填机票上的时间，千万不要做任何时间换算','2016-01-01 16:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'PickupTime',N'去酒店接人时间',N'PickTime',N'为保证不延误，至少需要提前3小时接人','2016-01-01 07:00')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'FlightNo',N'航班号',N'FlightNo',N'','CZ3869')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'ServiceDate',N'出行日期',N'ServiceDate',N'','2016-01-01')
INSERT INTO dbo.FormFields( [Key], FieldName, FieldNo, Remark, ExampleStyle )VALUES  ( N'BackDate',N'返回日期',N'BackDate',N'','2016-01-01')

----------------------------------------------------------------------字段-代号-名称-注解End---------------------------------------------------------------------------


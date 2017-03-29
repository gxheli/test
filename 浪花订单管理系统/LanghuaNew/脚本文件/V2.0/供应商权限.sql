

SET IDENTITY_INSERT dbo.SupplierRoles ON
DELETE dbo.SupplierRoles

INSERT INTO dbo.SupplierRoles( SupplierRoleID, [SupplierRoleName], [SupplierRoleEnName], Remark ,[CreateTime],[LastEditTime])
VALUES  ( 1,N'超级管理员',N'SuperAdmin','拥有全部权限', GETDATE(),GETDATE())

SET IDENTITY_INSERT SupplierRoles OFF

SET IDENTITY_INSERT dbo.SupplierRoleRights ON
DELETE dbo.SupplierRoleRights

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 1,N'Home',N'Index','工作台' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 2,N'langhua',N'EditPassWord','修改密码' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 3,N'Users',N'MessageSetting','消息配置' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 4,N'Orders',N'Index','订单列表' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 5,N'Orders',N'Details','订单详情' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 6,N'Orders',N'ExportExcel','导出excel' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 7,N'Orders',N'OrderOperation','订单日志' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 8,N'Product',N'Index','产品列表' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 9,N'BillReports',N'Index','对账单' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 10,N'Users',N'Index','子账号列表' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 11,N'Users',N'Create','新建子账号' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 12,N'Users',N'Edit','修改子账号' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 13,N'Users',N'UsersOperation','用户日志' )

SET IDENTITY_INSERT SupplierRoleRights OFF












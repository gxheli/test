

SET IDENTITY_INSERT dbo.SupplierRoles ON
DELETE dbo.SupplierRoles

INSERT INTO dbo.SupplierRoles( SupplierRoleID, [SupplierRoleName], [SupplierRoleEnName], Remark ,[CreateTime],[LastEditTime])
VALUES  ( 1,N'��������Ա',N'SuperAdmin','ӵ��ȫ��Ȩ��', GETDATE(),GETDATE())

SET IDENTITY_INSERT SupplierRoles OFF

SET IDENTITY_INSERT dbo.SupplierRoleRights ON
DELETE dbo.SupplierRoleRights

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 1,N'Home',N'Index','����̨' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 2,N'langhua',N'EditPassWord','�޸�����' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 3,N'Users',N'MessageSetting','��Ϣ����' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 4,N'Orders',N'Index','�����б�' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 5,N'Orders',N'Details','��������' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 6,N'Orders',N'ExportExcel','����excel' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 7,N'Orders',N'OrderOperation','������־' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 8,N'Product',N'Index','��Ʒ�б�' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 9,N'BillReports',N'Index','���˵�' )

INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 10,N'Users',N'Index','���˺��б�' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 11,N'Users',N'Create','�½����˺�' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 12,N'Users',N'Edit','�޸����˺�' )
INSERT INTO dbo.SupplierRoleRights( SupplierRoleRightID, ControllerName, ActionName, Remark )VALUES  ( 13,N'Users',N'UsersOperation','�û���־' )

SET IDENTITY_INSERT SupplierRoleRights OFF












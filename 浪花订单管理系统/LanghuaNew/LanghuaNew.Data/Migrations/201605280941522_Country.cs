namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        AreaID = c.Int(nullable: false, identity: true),
                        AreaName = c.String(nullable: false),
                        AreaEnName = c.String(nullable: false),
                        CityID = c.Int(nullable: false),
                        AreaEnableState = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AreaID)
                .ForeignKey("dbo.Cities", t => t.CityID, cascadeDelete: true)
                .Index(t => t.CityID);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityID = c.Int(nullable: false, identity: true),
                        CityName = c.String(nullable: false),
                        CityEnName = c.String(nullable: false),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CityID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false),
                        CountryEnName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierID = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(nullable: false),
                        SupplierNo = c.String(nullable: false),
                        CountryID = c.Int(nullable: false),
                        EMail = c.String(),
                        ContactWay = c.String(),
                        SupplierEnableState = c.Int(nullable: false),
                        SupplierSysName = c.String(),
                        SupplierPWD = c.String(),
                    })
                .PrimaryKey(t => t.SupplierID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.ServiceItems",
                c => new
                    {
                        ServiceItemID = c.Int(nullable: false, identity: true),
                        ServiceCode = c.String(),
                        cnItemName = c.String(),
                        enItemName = c.String(),
                        ServiceItemEnableState = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        TravelCompany = c.String(),
                        InsuranceDays = c.Int(nullable: false),
                        ServiceTypeID = c.Int(nullable: false),
                        DefaultSupplierID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceItemID)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeID, cascadeDelete: true)
                .Index(t => t.ServiceTypeID);
            
            CreateTable(
                "dbo.Elements",
                c => new
                    {
                        ElementID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        type = c.String(),
                        title = c.String(),
                        withNull = c.Boolean(nullable: false),
                        admin = c.Boolean(nullable: false),
                        ServiceItem_ServiceItemID = c.Int(),
                    })
                .PrimaryKey(t => t.ElementID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItem_ServiceItemID)
                .Index(t => t.ServiceItem_ServiceItemID);
            
            CreateTable(
                "dbo.items",
                c => new
                    {
                        itemID = c.String(nullable: false, maxLength: 128),
                        text = c.String(),
                        value = c.String(),
                        Element_ElementID = c.Int(),
                    })
                .PrimaryKey(t => t.itemID)
                .ForeignKey("dbo.Elements", t => t.Element_ElementID)
                .Index(t => t.Element_ElementID);
            
            CreateTable(
                "dbo.ExtraServices",
                c => new
                    {
                        ExtraServiceID = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        ServiceEnName = c.String(),
                        ServiceUnit = c.String(),
                        MinNum = c.Int(nullable: false),
                        MaxNum = c.Int(nullable: false),
                        IsNeedDetail = c.Boolean(nullable: false),
                        ServiceItem_ServiceItemID = c.Int(),
                    })
                .PrimaryKey(t => t.ExtraServiceID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItem_ServiceItemID)
                .Index(t => t.ServiceItem_ServiceItemID);
            
            CreateTable(
                "dbo.ServiceTypes",
                c => new
                    {
                        ServiceTypeID = c.Int(nullable: false, identity: true),
                        ServiceTypeName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceTypeID);
            
            CreateTable(
                "dbo.ServiceRules",
                c => new
                    {
                        ServiceRuleID = c.String(nullable: false, maxLength: 128),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        UseState = c.Int(nullable: false),
                        SelectRuleType = c.Int(nullable: false),
                        RangeStart = c.DateTime(nullable: false),
                        RangeEnd = c.DateTime(nullable: false),
                        Week = c.Int(nullable: false),
                        IsDouble = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceRuleID);
            
            CreateTable(
                "dbo.Hotals",
                c => new
                    {
                        HotalID = c.Int(nullable: false, identity: true),
                        HotalName = c.String(),
                        HotalPhone = c.String(),
                        HotalAddress = c.String(),
                        AreaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HotalID)
                .ForeignKey("dbo.Areas", t => t.AreaID, cascadeDelete: true)
                .Index(t => t.AreaID);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        CurrencyID = c.Int(nullable: false, identity: true),
                        CurrencyName = c.String(nullable: false),
                        CurrencyNo = c.String(nullable: false),
                        CurrencyChangeType = c.Int(nullable: false),
                        ExchangeRate = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.CurrencyID);
            
            CreateTable(
                "dbo.SupplierServiceItems",
                c => new
                    {
                        SupplierServiceItemID = c.Int(nullable: false, identity: true),
                        ServiceItemID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        CurrencyID = c.Int(nullable: false),
                        PayType = c.Int(nullable: false),
                        SelectEffectiveWay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierServiceItemID)
                .ForeignKey("dbo.Currencies", t => t.CurrencyID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItemID, cascadeDelete: true)
                .Index(t => t.ServiceItemID)
                .Index(t => t.SupplierID)
                .Index(t => t.CurrencyID);
            
            CreateTable(
                "dbo.ExtraServicePrices",
                c => new
                    {
                        ExtraServicePriceID = c.Int(nullable: false, identity: true),
                        ServicePrice = c.Single(nullable: false),
                        MinNum = c.Int(nullable: false),
                        Service_ExtraServiceID = c.Int(),
                        SupplierServiceItem_SupplierServiceItemID = c.Int(),
                    })
                .PrimaryKey(t => t.ExtraServicePriceID)
                .ForeignKey("dbo.ExtraServices", t => t.Service_ExtraServiceID)
                .ForeignKey("dbo.SupplierServiceItems", t => t.SupplierServiceItem_SupplierServiceItemID)
                .Index(t => t.Service_ExtraServiceID)
                .Index(t => t.SupplierServiceItem_SupplierServiceItemID);
            
            CreateTable(
                "dbo.ItemPriceBySuppliers",
                c => new
                    {
                        ItemPriceBySupplierID = c.Int(nullable: false, identity: true),
                        startTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        AdultNetPrice = c.Single(nullable: false),
                        ChildNetPrice = c.Single(nullable: false),
                        BobyNetPrice = c.Single(nullable: false),
                        Price = c.Single(nullable: false),
                        SupplierServiceItem_SupplierServiceItemID = c.Int(),
                    })
                .PrimaryKey(t => t.ItemPriceBySupplierID)
                .ForeignKey("dbo.SupplierServiceItems", t => t.SupplierServiceItem_SupplierServiceItemID)
                .Index(t => t.SupplierServiceItem_SupplierServiceItemID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        CustomerTBCode = c.String(),
                        CustomerName = c.String(),
                        CustomerEnname = c.String(),
                        Password = c.String(),
                        Tel = c.Int(nullable: false),
                        BakTel = c.Int(nullable: false),
                        Email = c.String(),
                        Wechat = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(),
                        TBOrderID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        state = c.Int(nullable: false),
                        IsPay = c.Boolean(nullable: false),
                        NetPrice = c.Single(nullable: false),
                        UserID = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.TBOrders", t => t.TBOrderID, cascadeDelete: true)
                .Index(t => t.TBOrderID)
                .Index(t => t.CustomerID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        PassWord = c.String(nullable: false),
                        NickName = c.String(nullable: false),
                        LateOnLineTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        RoleEnName = c.String(nullable: false),
                        RoleRemark = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleID);
            
            CreateTable(
                "dbo.CustomerServices",
                c => new
                    {
                        CustomerServiceID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        IsHandle = c.Boolean(nullable: false),
                        Remark = c.String(),
                        HandleRemark = c.String(),
                        CreateUserID = c.Int(nullable: false),
                        CreateUserName = c.String(),
                        HandleUserID = c.Int(nullable: false),
                        HandleUserName = c.String(),
                    })
                .PrimaryKey(t => t.CustomerServiceID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.OrderHistories",
                c => new
                    {
                        OrderHistoryID = c.Int(nullable: false, identity: true),
                        OperTime = c.DateTime(nullable: false),
                        Remark = c.String(),
                        State = c.Int(nullable: false),
                        OperUser_UserID = c.Int(),
                        Order_OrderID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderHistoryID)
                .ForeignKey("dbo.Users", t => t.OperUser_UserID)
                .ForeignKey("dbo.Orders", t => t.Order_OrderID)
                .Index(t => t.OperUser_UserID)
                .Index(t => t.Order_OrderID);
            
            CreateTable(
                "dbo.ServiceItemHistories",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                        ServiceCode = c.String(),
                        cnItemName = c.String(),
                        enItemName = c.String(),
                        SupplierID = c.String(),
                        SupplierName = c.String(),
                        AdultNetPrice = c.Single(nullable: false),
                        ChildNetPrice = c.Single(nullable: false),
                        BobyNetPrice = c.Single(nullable: false),
                        Price = c.Single(nullable: false),
                        CurrencyID = c.Int(nullable: false),
                        CurrencyName = c.String(),
                        CurrencyChangeType = c.Int(nullable: false),
                        ExchangeRate = c.Single(nullable: false),
                        PayType = c.Int(nullable: false),
                        Elements = c.String(),
                        ElementsValue = c.String(),
                        AdultNum = c.Int(nullable: false),
                        ChildNum = c.Int(nullable: false),
                        INFNum = c.Int(nullable: false),
                        RoomNum = c.Int(nullable: false),
                        RightNum = c.Int(nullable: false),
                        TravelDate = c.DateTime(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        GroupNo = c.String(),
                        ReceiveManTime = c.DateTime(nullable: false),
                        TrafficSurcharge = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.ExtraServiceHistories",
                c => new
                    {
                        ExtraServiceHistoryID = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        ServiceEnName = c.String(),
                        ServiceUnit = c.String(),
                        MinNum = c.Int(nullable: false),
                        MaxNum = c.Int(nullable: false),
                        IsNeedDetail = c.Boolean(nullable: false),
                        ServiceNum = c.Int(nullable: false),
                        ServicePrice = c.Single(nullable: false),
                        ServiceItemHistory_OrderID = c.Int(),
                    })
                .PrimaryKey(t => t.ExtraServiceHistoryID)
                .ForeignKey("dbo.ServiceItemHistories", t => t.ServiceItemHistory_OrderID)
                .Index(t => t.ServiceItemHistory_OrderID);
            
            CreateTable(
                "dbo.Travellers",
                c => new
                    {
                        TravellerID = c.Int(nullable: false, identity: true),
                        TravellerName = c.String(),
                        TravellerEnname = c.String(),
                        PassportNo = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        TravellerSex = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        TravellerDetail_Height = c.String(),
                        TravellerDetail_Weight = c.String(),
                        TravellerDetail_ShoesSize = c.String(),
                        TravellerDetail_ClothesSize = c.String(),
                        TravellerDetail_GlassesNum = c.String(),
                        Customer_CustomerID = c.Int(),
                    })
                .PrimaryKey(t => t.TravellerID)
                .ForeignKey("dbo.Customers", t => t.Customer_CustomerID)
                .Index(t => t.Customer_CustomerID);
            
            CreateTable(
                "dbo.TBOrders",
                c => new
                    {
                        TBOrderID = c.Int(nullable: false, identity: true),
                        TBID = c.String(),
                        OrderSourseID = c.Int(nullable: false),
                        IsSend = c.Boolean(nullable: false),
                        TBNum = c.String(),
                        TotalCost = c.Single(nullable: false),
                        TotalReceive = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.TBOrderID)
                .ForeignKey("dbo.OrderSourses", t => t.OrderSourseID, cascadeDelete: true)
                .Index(t => t.OrderSourseID);
            
            CreateTable(
                "dbo.OrderSourses",
                c => new
                    {
                        OrderSourseID = c.Int(nullable: false, identity: true),
                        OrderSourseName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.OrderSourseID);
            
            CreateTable(
                "dbo.ServiceItemSuppliers",
                c => new
                    {
                        ServiceItem_ServiceItemID = c.Int(nullable: false),
                        Supplier_SupplierID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceItem_ServiceItemID, t.Supplier_SupplierID })
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItem_ServiceItemID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_SupplierID, cascadeDelete: true)
                .Index(t => t.ServiceItem_ServiceItemID)
                .Index(t => t.Supplier_SupplierID);
            
            CreateTable(
                "dbo.ServiceRuleServiceItems",
                c => new
                    {
                        ServiceRule_ServiceRuleID = c.String(nullable: false, maxLength: 128),
                        ServiceItem_ServiceItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceRule_ServiceRuleID, t.ServiceItem_ServiceItemID })
                .ForeignKey("dbo.ServiceRules", t => t.ServiceRule_ServiceRuleID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItem_ServiceItemID, cascadeDelete: true)
                .Index(t => t.ServiceRule_ServiceRuleID)
                .Index(t => t.ServiceItem_ServiceItemID);
            
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        Role_RoleID = c.Int(nullable: false),
                        User_UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleID, t.User_UserID })
                .ForeignKey("dbo.Roles", t => t.Role_RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserID, cascadeDelete: true)
                .Index(t => t.Role_RoleID)
                .Index(t => t.User_UserID);
            
            CreateTable(
                "dbo.TravellerServiceItemHistories",
                c => new
                    {
                        Traveller_TravellerID = c.Int(nullable: false),
                        ServiceItemHistory_OrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Traveller_TravellerID, t.ServiceItemHistory_OrderID })
                .ForeignKey("dbo.Travellers", t => t.Traveller_TravellerID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItemHistories", t => t.ServiceItemHistory_OrderID, cascadeDelete: true)
                .Index(t => t.Traveller_TravellerID)
                .Index(t => t.ServiceItemHistory_OrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Travellers", "Customer_CustomerID", "dbo.Customers");
            DropForeignKey("dbo.TBOrders", "OrderSourseID", "dbo.OrderSourses");
            DropForeignKey("dbo.Orders", "TBOrderID", "dbo.TBOrders");
            DropForeignKey("dbo.TravellerServiceItemHistories", "ServiceItemHistory_OrderID", "dbo.ServiceItemHistories");
            DropForeignKey("dbo.TravellerServiceItemHistories", "Traveller_TravellerID", "dbo.Travellers");
            DropForeignKey("dbo.ServiceItemHistories", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.ExtraServiceHistories", "ServiceItemHistory_OrderID", "dbo.ServiceItemHistories");
            DropForeignKey("dbo.OrderHistories", "Order_OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderHistories", "OperUser_UserID", "dbo.Users");
            DropForeignKey("dbo.CustomerServices", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Orders", "UserID", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleID", "dbo.Roles");
            DropForeignKey("dbo.SupplierServiceItems", "ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.SupplierServiceItems", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.ItemPriceBySuppliers", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropForeignKey("dbo.SupplierServiceItems", "CurrencyID", "dbo.Currencies");
            DropForeignKey("dbo.ExtraServicePrices", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropForeignKey("dbo.ExtraServicePrices", "Service_ExtraServiceID", "dbo.ExtraServices");
            DropForeignKey("dbo.Hotals", "AreaID", "dbo.Areas");
            DropForeignKey("dbo.Suppliers", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules");
            DropForeignKey("dbo.ServiceItemSuppliers", "Supplier_SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.ServiceItemSuppliers", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.ServiceItems", "ServiceTypeID", "dbo.ServiceTypes");
            DropForeignKey("dbo.ExtraServices", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.Elements", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.items", "Element_ElementID", "dbo.Elements");
            DropForeignKey("dbo.Cities", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.Areas", "CityID", "dbo.Cities");
            DropIndex("dbo.TravellerServiceItemHistories", new[] { "ServiceItemHistory_OrderID" });
            DropIndex("dbo.TravellerServiceItemHistories", new[] { "Traveller_TravellerID" });
            DropIndex("dbo.RoleUsers", new[] { "User_UserID" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleID" });
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceItem_ServiceItemID" });
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID" });
            DropIndex("dbo.ServiceItemSuppliers", new[] { "Supplier_SupplierID" });
            DropIndex("dbo.ServiceItemSuppliers", new[] { "ServiceItem_ServiceItemID" });
            DropIndex("dbo.TBOrders", new[] { "OrderSourseID" });
            DropIndex("dbo.Travellers", new[] { "Customer_CustomerID" });
            DropIndex("dbo.ExtraServiceHistories", new[] { "ServiceItemHistory_OrderID" });
            DropIndex("dbo.ServiceItemHistories", new[] { "OrderID" });
            DropIndex("dbo.OrderHistories", new[] { "Order_OrderID" });
            DropIndex("dbo.OrderHistories", new[] { "OperUser_UserID" });
            DropIndex("dbo.CustomerServices", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            DropIndex("dbo.Orders", new[] { "TBOrderID" });
            DropIndex("dbo.ItemPriceBySuppliers", new[] { "SupplierServiceItem_SupplierServiceItemID" });
            DropIndex("dbo.ExtraServicePrices", new[] { "SupplierServiceItem_SupplierServiceItemID" });
            DropIndex("dbo.ExtraServicePrices", new[] { "Service_ExtraServiceID" });
            DropIndex("dbo.SupplierServiceItems", new[] { "CurrencyID" });
            DropIndex("dbo.SupplierServiceItems", new[] { "SupplierID" });
            DropIndex("dbo.SupplierServiceItems", new[] { "ServiceItemID" });
            DropIndex("dbo.Hotals", new[] { "AreaID" });
            DropIndex("dbo.ExtraServices", new[] { "ServiceItem_ServiceItemID" });
            DropIndex("dbo.items", new[] { "Element_ElementID" });
            DropIndex("dbo.Elements", new[] { "ServiceItem_ServiceItemID" });
            DropIndex("dbo.ServiceItems", new[] { "ServiceTypeID" });
            DropIndex("dbo.Suppliers", new[] { "CountryID" });
            DropIndex("dbo.Cities", new[] { "CountryID" });
            DropIndex("dbo.Areas", new[] { "CityID" });
            DropTable("dbo.TravellerServiceItemHistories");
            DropTable("dbo.RoleUsers");
            DropTable("dbo.ServiceRuleServiceItems");
            DropTable("dbo.ServiceItemSuppliers");
            DropTable("dbo.OrderSourses");
            DropTable("dbo.TBOrders");
            DropTable("dbo.Travellers");
            DropTable("dbo.ExtraServiceHistories");
            DropTable("dbo.ServiceItemHistories");
            DropTable("dbo.OrderHistories");
            DropTable("dbo.CustomerServices");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
            DropTable("dbo.ItemPriceBySuppliers");
            DropTable("dbo.ExtraServicePrices");
            DropTable("dbo.SupplierServiceItems");
            DropTable("dbo.Currencies");
            DropTable("dbo.Hotals");
            DropTable("dbo.ServiceRules");
            DropTable("dbo.ServiceTypes");
            DropTable("dbo.ExtraServices");
            DropTable("dbo.items");
            DropTable("dbo.Elements");
            DropTable("dbo.ServiceItems");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
            DropTable("dbo.Areas");
        }
    }
}

namespace LanghuaNew.Data
{
    using System.Data.Entity;

    public partial class LanghuaContent : DbContext
    {
        public LanghuaContent()
            : base("name=LanghuaContent")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
        public DbSet<Area> Areas { get; set; }

        public DbSet<Country> Countrys { get; set; }

        public DbSet<Currency> Currencys { get; set; }

        public DbSet<Customer> Customers { get; set; }

     

        public DbSet<Element> Elements{ get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderSourse> OrderSourses { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }

      

        public DbSet<ServiceType> ServiceTypes { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<SupplierServiceItem> SupplierServiceItems { get; set; }

        public DbSet<TBOrder> TBOrders { get; set; }

        public DbSet<Traveller> Travellers { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Hotal> Hotals { get; set; }

        public DbSet<ServiceRule> ServiceRules { get; set; }

        public DbSet<ServiceItemHistory> ServiceItemHistories { get; set; }

        public DbSet<ExtraService> ExtraServices { get; set; }

        public DbSet<ExtraServicePrice> ExtraServicePrices { get; set; }

        public DbSet<ExtraServiceHistory> ExtraServiceHistories { get; set; }

        public DbSet<ServiceItemTemplte> ServiceItemTempltes { get; set; }

        public DbSet<RoleRight> RoleRights { get; set; }

        public DbSet<SendMessage> SendMessages { get; set; }

        public DbSet<TBOrderState> TBOrderStates { get; set; }

        public DbSet<OrderHistory> OrderHistories { get; set; }

        public DbSet<FormField> FormFields { get; set; }

        public DbSet<AlipayTransfer> AlipayTransfers { get; set; }

        public DbSet<AlipayTransferLog> AlipayTransferLogs { get; set; }

        public DbSet<SystemLog> SystemLogs { get; set; }

        public DbSet<SellControl> SellControls { get; set; }

        public DbSet<ServiceRuleLog> ServiceRuleLogs { get; set; }

        public DbSet<UserLog> UserLogs { get; set; }

        public DbSet<UserLoginLog> UserLoginLogs { get; set; }

        public DbSet<CustomerReturnList> CustomerReturnLists { get; set; }

        public DbSet<CustomerBack> CustomerBacks { get; set; }
        public DbSet<WeixinMessage> WeixinMessages { get; set; }
        public DbSet<WorkTableDisplayItem> WorkTableDisplayItems { get; set; }
        public DbSet<OrderTraveller> OrderTravellers { get; set; }
        public DbSet<ExportOrder> ExportOrders { get; set; }
        public DbSet<BillReport> BillReports { get; set; }

        public DbSet<ItemPriceBySupplier> ItemPriceBySuppliers { get; set; }
        public DbSet<ExtraSetting> ExtraSettings { get; set; }
        public DbSet<SupplierRole> SupplierRoles { get; set; }
        public DbSet<SupplierRoleRight> SupplierRoleRights { get; set; }
        public DbSet<SupplierUser> SupplierUsers { get; set; }
        public DbSet<SupplierUserLog> SupplierUserLogs { get; set; }
        public DbSet<CustomerLog> CustomerLogs { get; set; }
        public DbSet<DistributionTally> DistributionTallys { get; set; }
        public DbSet<CancelRegister> CancelRegisters { get; set; }
        public DbSet<OrderSupplierHistory> OrderSupplierHistories { get; set; }
        public DbSet<SalesStatistic> SalesStatistics { get; set; }
        public DbSet<MenuRight> MenuRights { get; set; }
        public DbSet<SupplierServiceItemChange> SupplierServiceItemChanges { get; set; }
        public DbSet<ItemPriceBySupplierChange> ItemPriceBySupplierChanges { get; set; }
        public DbSet<ExtraServicePriceChange> ExtraServicePriceChanges { get; set; }
        public DbSet<ItemPriceLog> ItemPriceLogs { get; set; }

        public DbSet<FliterInfo> FliterInfos { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.QRcodeStatistic> QRcodeStatistics { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.TB_Access_Token> TB_Access_Token { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.CancelRegisterLog> CancelRegisterLogs { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.SellControlClassify> SellControlClassifies { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.SellControlShow> SellControlShows { get; set; }

        public System.Data.Entity.DbSet<LanghuaNew.Data.TBOrderNo> TBOrderNoes { get; set; }
    }
}

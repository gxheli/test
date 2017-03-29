using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using LanghuaNew.Data;
using WXData;

namespace LanghuaNew.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            var Users = builder.EntitySet<User>("Users");
            var Roles = builder.EntitySet<Role>("Roles");
            Users.HasManyBinding<Role>(p => p.UserRole, "Roles");
            Roles.HasManyBinding<User>(p => p.Users, "Users");
            builder.EntitySet<TBOrder>("TBOrders");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<OrderSourse>("OrderSourses");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<OrderHistory>("OrderHistories");
            builder.EntitySet<ServiceItemHistory>("ServiceItemHistories");
            builder.EntitySet<SupplierServiceItem>("SupplierServiceItems");
            builder.EntitySet<ExtraServicePrice>("ExtraServicePrices");
            builder.EntitySet<ExtraServiceHistory>("ExtraServiceHistories");
            builder.EntitySet<ExtraService>("ExtraServices");
            builder.EntitySet<Currency>("Currencies");
            builder.EntitySet<ItemPriceBySupplier>("ItemPriceBySuppliers");
            builder.EntitySet<Supplier>("Suppliers");
            builder.EntitySet<ServiceItem>("ServiceItems");
            builder.EntitySet<ServiceType>("ServiceTypes");
            builder.EntitySet<Traveller>("Travellers");
            builder.EntitySet<Hotal>("Hotals");
            builder.EntitySet<Area>("Areas");
            builder.EntitySet<ServiceItemTemplte>("ServiceItemTempltes");
            builder.EntitySet<Country>("Countries");
            builder.EntitySet<ServiceRule>("ServiceRules");
            builder.EntitySet<FormField>("FormFields");
            builder.EntitySet<City>("Cities");
            builder.EntitySet<AlipayTransferLog>("AlipayTransferLogs");
            builder.EntitySet<AlipayTransfer>("AlipayTransfers");
            builder.EntitySet<TBOrderState>("TBOrderStates");
            builder.EntitySet<ServiceRuleLog>("ServiceRuleLogs");
            builder.EntitySet<SystemLog>("SystemLogs");
            builder.EntitySet<SellControl>("SellControls");
            builder.EntitySet<UserLog>("UserLogs");
            builder.EntitySet<CustomerReturnList>("CustomerReturnLists");
            builder.EntitySet<CustomerBack>("CustomerBacks");
            builder.EntitySet<WeixinMessage>("WeixinMessages");
            builder.EntitySet<WorkTableDisplayItem>("WorkTableDisplayItems");
            builder.EntitySet<OrderTraveller>("OrderTravellers");
            builder.EntitySet<ExportOrder>("ExportOrders");
            builder.EntitySet<BillReport>("BillReports");
            builder.EntitySet<ExtraSetting>("ExtraSettings");
            builder.EntitySet<SupplierRole>("SupplierRoles");
            builder.EntitySet<SupplierRoleRight>("SupplierRoleRights");
            builder.EntitySet<SupplierUser>("SupplierUsers");
            builder.EntitySet<SupplierUserLog>("SupplierUserLogs");
            builder.EntitySet<OrderSupplierHistory>("OrderSupplierHistories");
            builder.EntitySet<CustomerLog>("CustomerLogs");
            builder.EntitySet<CancelRegister>("CancelRegisters");
            builder.EntitySet<DistributionTally>("DistributionTallies");
            builder.EntitySet<MenuRight>("MenuRights");
            builder.EntitySet<RoleRight>("RoleRights");
            builder.EntitySet<SalesStatistic>("SalesStatistics");
            builder.EntitySet<SupplierServiceItemChange>("SupplierServiceItemChanges");
            builder.EntitySet<ExtraServicePriceChange>("ExtraServicePriceChanges");
            builder.EntitySet<ItemPriceBySupplierChange>("ItemPriceBySupplierChanges");
            builder.EntitySet<ItemPriceLog>("ItemPriceLogs");
            builder.EntitySet<FliterInfo>("FliterInfos");
            builder.EntitySet<QRcodeStatistic>("QRcodeStatistics");
            builder.EntitySet<TB_Access_Token>("TB_Access_Token");
            builder.EntitySet<CancelRegisterLog>("CancelRegisterLogs");
            builder.EntitySet<SellControlShow>("SellControlShows");
            builder.EntitySet<SellControlClassify>("SellControlClassifies");
            builder.EntitySet<TBOrderNo>("TBOrderNoes");
            //微信接口
            builder.EntitySet<WeiXinMenu>("WeiXinMenus");
            builder.EntitySet<MenuItem>("MenuItems");
            builder.EntitySet<LHNew>("LHNews");
            builder.EntitySet<LHArticle>("LHArticles");
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}

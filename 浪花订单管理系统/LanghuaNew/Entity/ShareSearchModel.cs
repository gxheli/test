using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ShareSearchModel
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public ShareOrderByModel OrderBy { get; set; }
        public ShareOrderSearchModel OrderSearch { get; set; }
        public ShareCustomerSearchModel CustomerSearch { get; set; }
        public ShareTBOrderStatesSearchModel StateSearch { get; set; }
        public ShareServiceItemSearchModel ItemSearch { get; set; }
        public ShareUsersSearchModel UsersSearch { get; set; }
        public BillSearchSearchModel BillSearch { get; set; }
        public CancelRegisterSearchModel CancelRegisterSearch { get; set; }
        public DistributionTallySearchModel DistributionTallySearch { get; set; }
        public SalesStatisticSearchModel SalesStatisticSearch { get; set; }
        public ItemPriceSearchModel ItemPriceSearch { get; set; }
    }
    public class ShareUsersSearchModel
    {
        public string FuzzySearch { get; set; }
        public int status { get; set; }
        public string searchType { get; set; }
    }
    public class ShareOrderByModel
    {
        public string PropertyName { get; set; }
        public int OrderBy { get; set; }
    }
    public class ShareCustomerSearchModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int status { get; set; }
        public string FuzzySearch { get; set; }
        public bool ReturnList { get; set; }
    }
    //发货查询条件
    public class ShareTBOrderStatesSearchModel
    {
        public string FuzzySearch { get; set; }
        public int StateValue { get; set; }
        public int DateValue { get; set; }
        public int OrderSourseID { get; set; }
        public string TravelDate { get; set; }

    }
    //订单列表搜索条件
    public class ShareOrderSearchModel
    {
        public string status { get; set; }
        public string FuzzySearch { get; set; }
        public int OrderSourseID { get; set; }
        public int SupplierID { get; set; }
        public string ItemName { get; set; }
        public string IsPay { get; set; }
        public string IsNeedCustomerService { get; set; }
        public string isOneself { get; set; }
        public string isUrgent { get; set; }
        public string SupplierEnableOnline { get; set; }
        public string TravelDateBegin { get; set; }
        public string TravelDateEnd { get; set; }
        public string OrderCreateDateBegin { get; set; }
        public string OrderCreateDateEnd { get; set; }
        public string ReturnDateBegin { get; set; }
        public string ReturnDateEnd { get; set; }
        public string ServiceTypeID { get; set; }

        public string SupplierName { get; set; }
        public string OrderSourseName { get; set; }
        public string statusNamae { get; set; }
        public string SupplierEnableOnlineName { get; set; }
        public string ServiceTypeName { get; set; }

        public string searchType { get; set; }

        public string ExportField { get; set; }
        public string IsChangeTravelDate { get; set; }
        public string CreateName { get; set; }
        //订单操作日期
        public string OrderOperDateBegin { get; set; }
        public string OrderOperDateEnd { get; set; }
        //模糊搜索类型
        public string FuzzySearchType { get; set; }
        //订单发送日期
        public string OrderSendDateBegin { get; set; }
        public string OrderSendDateEnd { get; set; }
    }
    //产品查询条件
    public class ShareServiceItemSearchModel
    {
        public int Type { get; set; }
        public int SupplierID { get; set; }
        public string FuzzySearch { get; set; }
    }
    public class BillSearchSearchModel
    {
        public int status { get; set; }
        public int SupplierID { get; set; }
        public int datetype { get; set; }
        public string date { get; set; }
    }
    public class CancelRegisterSearchModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public int SupplierID { get; set; }
    }
    public class DistributionTallySearchModel
    {
        public int SupplierID { get; set; }
        public string ItemName { get; set; }
        public string TravelDateBegin { get; set; }
        public string FuzzySearch { get; set; }
    }
    public class SalesStatisticSearchModel
    {
        public int SalesStatisticID { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
    public class ItemPriceSearchModel
    {
        public int SupplierID { get; set; }
        public int ServiceTypeID { get; set; }
        public string FuzzySearch { get; set; }
        public bool IsChange { get; set; }
        public int CityID { get; set; }
    }
}

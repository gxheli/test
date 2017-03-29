using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class CustomersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/CustomersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CustomersExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CustomersExtend
        public CustomerListModel Post([FromBody]string value)
        {
            ShareSearchModel search = JsonConvert.DeserializeObject<ShareSearchModel>(value);
            int draw = 1;
            int start = 0;
            int length = 50;
            int status = 0;
            string FuzzySearch = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;
            bool ReturnList = false;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.CustomerSearch != null)
            {
                status = search.CustomerSearch.status;
                FuzzySearch = search.CustomerSearch.FuzzySearch;
                StartDate = search.CustomerSearch.StartDate;
                EndDate = search.CustomerSearch.EndDate;
                ReturnList = search.CustomerSearch.ReturnList;
            }
            //如果根据回访名单查找
            if (ReturnList)
            {
                //try
                //{
                //    DateTime.Parse(StartDate);
                //    DateTime.Parse(EndDate);
                //}
                //catch
                //{
                //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "时间格式错误" });
                //}
                DateTimeOffset startdd = DateTimeOffset.Parse(StartDate);
                DateTimeOffset enddd = DateTimeOffset.Parse(EndDate).AddDays(1);
                string sql =
                        "select top " + length + " * from (" +
                        "select *,isnull((select top 1 Remark from dbo.CustomerBacks where CustomerID=x.CustomerID order by CreateData),'') as Remark from (" +
                        "SELECT distinct a.CreateTime,a.CustomerID,a.CustomerTBCode,a.CustomerName,a.CustomerEnname,a.Tel,isnull(a.BakTel,'') as BakTel,a.Wechat as WeixinNo,case isnull(a.openid,'') when '' then cast(0 as bit) else cast(1 as bit) end as BindWeixin,a.IsNeedCustomerService,a.IsBack,count(b.orderid) as OrderNum " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 " +
                        "group by a.CreateTime,a.CustomerID,a.CustomerTBCode,a.CustomerName,a.CustomerEnname,a.Tel,a.BakTel,a.Wechat,case isnull(a.openid,'') when '' then cast(0 as bit) else cast(1 as bit) end,a.IsNeedCustomerService,a.IsBack)x " +
                        ")m where CustomerID not in (" +
                        "select top " + start + " CustomerID from(" +
                        "SELECT distinct a.CreateTime,a.CustomerID " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 " +
                        ")x order by CreateTime desc" +
                        ") order by CreateTime desc";
                var customers = db.Database.SqlQuery<CustomerModel>(sql, startdd, enddd);

                string sqlcount = "select count(*) from (SELECT distinct a.CustomerID " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 )x";
                var count = db.Database.SqlQuery<int>(sqlcount, startdd, enddd).First();

                string sqlNeedServiceCount = "select count(*) from (SELECT distinct a.CustomerID " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 and a.IsNeedCustomerService=1 )x";
                int NeedServiceCount = db.Database.SqlQuery<int>(sqlNeedServiceCount, startdd, enddd).First();

                string sqlBackCount = "select count(*) from (SELECT distinct a.CustomerID " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 and a.IsBack=1 )x";
                int BackCount = db.Database.SqlQuery<int>(sqlBackCount, startdd, enddd).First();

                string sqlWeixinBindCount = "select count(*) from (SELECT distinct a.CustomerID " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<@p1 and isnull(a.OpenID,'')!='' )x";
                int WeixinBindCount = db.Database.SqlQuery<int>(sqlWeixinBindCount, startdd, enddd).First();

                CustomerListModel list = new CustomerListModel();
                list.draw = draw;
                list.recordsFiltered = count;
                list.data = customers.ToList();
                list.NeedServiceCount = NeedServiceCount;
                list.BackCount = BackCount;
                list.WeixinBindCount = WeixinBindCount;
                list.SearchModel = search;
                return list;
            }
            else
            {
                IQueryable<Customer> customers = db.Customers.Include(c => c.Orders).Include(c => c.CustomerBacks);
                if (status == 1)
                {
                    customers = customers.Where(t => t.IsNeedCustomerService);
                }
                else if (status == 2)
                {
                    customers = customers.Where(t => t.IsBack);
                }
                else if (status == 3)
                {
                    customers = customers.Where(t => !t.IsBack);
                }
                if (!string.IsNullOrEmpty(FuzzySearch))
                {
                    customers = customers.Where(t => t.CustomerTBCode.Contains(FuzzySearch) || t.CustomerName.Contains(FuzzySearch) || t.CustomerEnname.Contains(FuzzySearch) || t.Tel.Contains(FuzzySearch) || t.BakTel.Contains(FuzzySearch) || t.Wechat.Contains(FuzzySearch) || t.Email.Contains(FuzzySearch));
                }
                int count = customers.Count();
                int NeedServiceCount = customers.Where(t => t.IsNeedCustomerService).Count();
                int BackCount = customers.Where(t => t.IsBack).Count();
                int WeixinBindCount = customers.Where(t => t.OpenID != null && t.OpenID != "").Count();

                customers = customers.OrderByDescending(c => c.CreateTime).Skip(start).Take(length);
                List<CustomerModel> data = new List<CustomerModel>();
                foreach (var item in customers)
                {
                    CustomerModel customer = new CustomerModel();
                    customer.CustomerID = item.CustomerID;
                    customer.CustomerTBCode = item.CustomerTBCode == null ? "" : item.CustomerTBCode;
                    customer.CustomerName = item.CustomerName == null ? "" : item.CustomerName;
                    customer.CustomerEnname = item.CustomerEnname == null ? "" : item.CustomerEnname;
                    customer.Tel = item.Tel == null ? "" : item.Tel;
                    customer.BakTel = item.BakTel == null ? "" : item.BakTel;
                    customer.WeixinNo = item.Wechat;
                    customer.BindWeixin = !string.IsNullOrEmpty(item.OpenID);
                    customer.IsNeedCustomerService = item.IsNeedCustomerService;
                    customer.IsBack = item.IsBack;
                    customer.OrderNum = item.Orders == null ? 0 : item.Orders.Where(o => o.state != OrderState.Invalid).Count();
                    customer.Remark = item.CustomerBacks.Count() > 0 ? item.CustomerBacks.OrderByDescending(c => c.CreateData).ToList()[0].Remark : "";
                    data.Add(customer);
                }
                CustomerListModel list = new CustomerListModel();
                list.draw = draw;
                list.recordsFiltered = count;
                list.data = data;
                list.NeedServiceCount = NeedServiceCount;
                list.BackCount = BackCount;
                list.WeixinBindCount = WeixinBindCount;
                list.SearchModel = search;
                return list;
            }
        }

        // PUT: api/CustomersExtend/5
        public IEnumerable<CustomerModel> Put([FromBody]string value)
        {
            ShareCustomerSearchModel search = JsonConvert.DeserializeObject<ShareCustomerSearchModel>(value);
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;
            string StartDate = search.StartDate;
            string EndDate = search.EndDate;
            bool ReturnList = search.ReturnList;
            //如果根据回访名单查找
            if (ReturnList)
            {
                DateTimeOffset startdd = DateTimeOffset.Parse(StartDate);
                DateTimeOffset enddd = DateTimeOffset.Parse(EndDate).AddDays(1);

                string sql = "select *,isnull((select top 1 Remark from dbo.CustomerBacks where CustomerID=x.CustomerID order by CreateData),'') as Remark from (" +
                        "SELECT  distinct a.CreateTime,a.CustomerID,a.CustomerTBCode,a.CustomerName,a.CustomerEnname,a.Tel,a.BakTel,a.Wechat as WeixinNo,case isnull(a.openid,'') when '' then cast(0 as bit) else cast(1 as bit) end as BindWeixin,a.IsNeedCustomerService,a.IsBack,count(b.orderid) as OrderNum " +
                        "FROM dbo.Customers a " +
                        "JOIN dbo.Orders b ON a.CustomerID = b.CustomerID and b.state !=13 " +
                        "JOIN dbo.ServiceItemHistories c ON b.OrderID = c.OrderID " +
                        "JOIN dbo.CustomerReturnLists d ON c.ServiceItemID = d.ServiceItemID AND c.SupplierID = d.SupplierID " +
                        "WHERE c.TravelDate>=@p0 AND c.TravelDate<=@p1 " +
                        "group by a.CreateTime,a.CustomerID,a.CustomerTBCode,a.CustomerName,a.CustomerEnname,a.Tel,a.BakTel,a.Wechat,case isnull(a.openid,'') when '' then cast(0 as bit) else cast(1 as bit) end,a.IsNeedCustomerService,a.IsBack)x order by CreateTime desc";
                var customers = db.Database.SqlQuery<CustomerModel>(sql, startdd, enddd);
                return customers;
            }
            else
            {
                IQueryable<Customer> customers = db.Customers.Include(c => c.Orders).Include(c => c.CustomerBacks);
                if (status == 1)
                {
                    customers = customers.Where(t => t.IsNeedCustomerService);
                }
                else if (status == 2)
                {
                    customers = customers.Where(t => t.IsBack);
                }
                else if (status == 3)
                {
                    customers = customers.Where(t => !t.IsBack);
                }
                if (!string.IsNullOrEmpty(FuzzySearch))
                {
                    customers = customers.Where(t => t.CustomerTBCode.Contains(FuzzySearch) || t.CustomerName.Contains(FuzzySearch) || t.CustomerEnname.Contains(FuzzySearch) || t.Tel.Contains(FuzzySearch) || t.BakTel.Contains(FuzzySearch) || t.Wechat.Contains(FuzzySearch) || t.Email.Contains(FuzzySearch));
                }
                int count = customers.Count();
                int NeedServiceCount = customers.Where(t => t.IsNeedCustomerService).Count();
                int BackCount = customers.Where(t => t.IsBack).Count();
                int WeixinBindCount = customers.Where(t => t.OpenID != null && t.OpenID != "").Count();

                customers = customers.OrderByDescending(c => c.CreateTime);
                List<CustomerModel> data = new List<CustomerModel>();
                foreach (var item in customers)
                {
                    CustomerModel customer = new CustomerModel();
                    customer.CustomerID = item.CustomerID;
                    customer.CustomerTBCode = item.CustomerTBCode == null ? "" : item.CustomerTBCode;
                    customer.CustomerName = item.CustomerName == null ? "" : item.CustomerName;
                    customer.CustomerEnname = item.CustomerEnname == null ? "" : item.CustomerEnname;
                    customer.Tel = item.Tel == null ? "" : item.Tel;
                    customer.BakTel = item.BakTel == null ? "" : item.BakTel;
                    customer.WeixinNo = item.Wechat;
                    customer.BindWeixin = !string.IsNullOrEmpty(item.OpenID);
                    customer.IsNeedCustomerService = item.IsNeedCustomerService;
                    customer.IsBack = item.IsBack;
                    customer.OrderNum = item.Orders == null ? 0 : item.Orders.Where(o => o.state != OrderState.Invalid).Count();
                    customer.Remark = item.CustomerBacks.Count() > 0 ? item.CustomerBacks.OrderByDescending(c => c.CreateData).ToList()[0].Remark : "";
                    data.Add(customer);
                }
                return data;
            }
        }

        // DELETE: api/CustomersExtend/5
        public void Delete(int id)
        {
        }
    }
}

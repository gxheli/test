using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class OldOrdersController : ApiController
    {
        public static string Content = System.Configuration.ConfigurationManager.AppSettings["LanghuaContentOld"];
        // GET: api/OldOrders
        public DataSet Get(string TBID)
        {
            try
            {
                SqlConnection lo_conn = new SqlConnection(Content);
                lo_conn.Open();
                SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
                lo_cmd.CommandText = @"
select top 100 ABC.*, Orders.TBID, Orders.cnName, ServiceItem.cnItemName AS Service 
from (select OrderNo, ServiceCode, Adult, Child, INF, CONVERT(VARCHAR(10), DepDate, 120) as DepDate, Status from MeetandSend 
union all select OrderNo, ServiceCode, Adult, Child, INF, CONVERT(VARCHAR(10), TourDate, 120) as DepDate, Status  from TourandShow  
union all select OrderNo, ServiceCode, NumRoom as Adult, Nights as Child, INF, CONVERT(VARCHAR(10), CheckinDate, 120) as DepDate, Status  
from Hotel) as ABC 
left join Orders on ABC.OrderNo=Orders.OrderNo 
left join ServiceItem on ABC.ServiceCode=ServiceItem.ServiceCode where TBID='" + TBID + "' and abc.Status<=3 order by depdate";

                lo_cmd.Connection = lo_conn;            //指定连接对象
                SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd);
                DataSet ds = new DataSet(); //创建数据集对象
                dbAdapter.Fill(ds);
                lo_conn.Close();

                return ds;
            }
            catch
            {
                return null;
            }
        }
    }
}

using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class TBOrderNoesExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/TBOrderNoesExtend
        public IEnumerable<TBOrderSubNo> Get()
        {
            string sql = @"	SELECT DISTINCT d.SubNo
	            FROM dbo.Orders a
	            JOIN dbo.OrderHistories b ON a.OrderID = b.OrderID
	            JOIN dbo.TBOrderNoes c ON a.OrderID = c.OrderID
	            JOIN dbo.TBOrderNoes d ON c.SubNo = d.SubNo AND c.OrderID != d.OrderID
	            WHERE CONVERT(VARCHAR(10), a.CreateTime, 120)=CONVERT(VARCHAR(10), DATEADD(dd,-1,GETDATE()), 120) OR 
	            ((b.State=11 OR b.State=13)AND CONVERT(VARCHAR(10), b.OperTime, 120)=CONVERT(VARCHAR(10), DATEADD(dd,-1,GETDATE()), 120))";

            return db.Database.SqlQuery<TBOrderSubNo>(sql);
        }

    }
}

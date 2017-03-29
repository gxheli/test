using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using LanghuaNew.Data;
using System.Data.SqlClient;
using Entity;

namespace LanghuaNew.Service.Controllers
{
    public class SellControlShowsExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/SellControlShowsExtend
        public IEnumerable<SellControlModel> Get(int id,string StartDate,string EndDate)
        {
            SqlParameter[] sqlparms = new SqlParameter[] {
                new SqlParameter("@id", id),
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate),
            };
            var result = db.Database.SqlQuery<SellControlModel>("EXEC dbo.StatisticsSellControl @id=@id,@StartDate=@StartDate,@EndDate=@EndDate", sqlparms).ToList();
            return result;
        }
    }
}

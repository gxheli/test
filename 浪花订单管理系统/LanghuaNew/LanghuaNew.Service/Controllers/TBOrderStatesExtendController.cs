using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class TBOrderStatesExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // POST: api/TBOrderStatesExtend
        public TBOrderStateListModel Post([FromBody]string value)
        {
            //value = "{\"StateSearch\":{\"FuzzySearch\":\"heli\",\"TravelDate\":\"2016-08-12\",\"StateValue\":0,\"DateValue\":2,\"OrderSourseID\":2}}";
            ShareSearchModel search = JsonConvert.DeserializeObject<ShareSearchModel>(value);
            int draw = 1;
            int start = 0;
            int length = 50;
            string FuzzySearch = string.Empty;
            string TravelDate = string.Empty;
            int StateValue = 0;
            int DateValue = 0;
            int OrderSourseID = 0;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.StateSearch != null)
            {
                FuzzySearch = search.StateSearch.FuzzySearch;
                TravelDate = search.StateSearch.TravelDate;
                StateValue = search.StateSearch.StateValue;
                DateValue = search.StateSearch.DateValue;
                OrderSourseID = search.StateSearch.OrderSourseID;
            }
            DateTimeOffset date = string.IsNullOrEmpty(TravelDate) ? DateTimeOffset.Now.Date.AddDays(-1) : DateTimeOffset.Parse(TravelDate);
            DateTimeOffset nextdate = date.AddDays(1);
            //未发货
            if (StateValue == 1)
            {
                var result = from x in (
                             from a in db.TBOrderStates
                             join b in db.Customers on a.CustomerID equals b.CustomerID
                             where
                               !a.IsSend && !b.IsNeedCustomerService
                             group new { a, b } by new
                             {
                                 a.OrderSourseID,
                                 b.CustomerTBCode
                             } into g
                             select new
                             {
                                 OrderSourseID = g.Key.OrderSourseID,
                                 g.Key.CustomerTBCode,
                                 mindate = g.Min(p => p.a.OrderDate),
                                 maxdate = g.Max(p => p.a.OrderDate)
                             })
                             where
                               ((DateValue == 1 && //x.mindate >= date &&
                               x.mindate < nextdate) || (DateValue == 2 && x.maxdate >= date &&
                               x.maxdate < nextdate)) &&
                               x.OrderSourseID == OrderSourseID
                             select new TBOrderStateModel
                             {
                                 OrderSourseID = x.OrderSourseID,
                                 OrderSourseName = ((from f in db.OrderSourses
                                                     where
                                          f.OrderSourseID == x.OrderSourseID
                                                     select new
                                                     {
                                                         f.OrderSourseName
                                                     }).FirstOrDefault().OrderSourseName),
                                 CustomerTBCode = x.CustomerTBCode,
                                 mindate = x.mindate,
                                 maxdate = x.maxdate,
                                 SendUserName = "",
                                 sendtime = DateTimeOffset.MinValue,
                                 IsSend = 0,
                             };
                int count = result.Count();
                result = result.OrderBy(s => s.mindate).Skip(start).Take(length);
                TBOrderStateListModel list = new TBOrderStateListModel();
                list.draw = draw;
                list.recordsFiltered = count;
                list.SearchModel = search;
                list.data = result.ToList();
                return list;
            }

            //已发货
            if (StateValue == 2)
            {
                var result = from x in (
    (from a in db.TBOrderStates
     join b in db.Customers on a.CustomerID equals b.CustomerID
     group new { a, b } by new
     {
         a.OrderSourseID,
         b.CustomerTBCode,
         a.CustomerID
     } into g
     select new
     {
         OrderSourseID = g.Key.OrderSourseID,
         g.Key.CustomerTBCode,
         CustomerID = g.Key.CustomerID,
         mindate = g.Min(p => p.a.OrderDate),
         maxdate = g.Max(p => p.a.OrderDate),
         sendtime = g.Max(p => p.a.SendTime),
         IsSend = g.Min(p => p.a.IsSend ? 1 : 0)
     }))
                             where
                               x.IsSend == 1 && ((DateValue == 1 && x.mindate >= date &&
                               x.mindate < nextdate) || (DateValue == 2 && x.maxdate >= date &&
                               x.maxdate < nextdate)) &&
                               x.OrderSourseID == OrderSourseID
                             select new TBOrderStateModel
                             {
                                 OrderSourseID = x.OrderSourseID,
                                 OrderSourseName = ((from f in db.OrderSourses
                                                     where
                                          f.OrderSourseID == x.OrderSourseID
                                                     select new
                                                     {
                                                         f.OrderSourseName
                                                     }).FirstOrDefault().OrderSourseName),
                                 CustomerTBCode = x.CustomerTBCode,
                                 mindate = x.mindate,
                                 maxdate = x.maxdate,
                                 sendtime = x.sendtime,
                                 IsSend = x.IsSend,
                                 SendUserName =
                                 ((from f in db.TBOrderStates
                                   where
        f.CustomerID == x.CustomerID &&
        f.OrderSourseID == x.OrderSourseID
                                   orderby
        f.SendTime descending
                                   select new
                                   {
                                       SendUserName = string.IsNullOrEmpty(f.SendUserName) ? "" : f.SendUserName
                                   }).Take(1).FirstOrDefault().SendUserName)
                             };
                int count = result.Count();
                result = result.OrderBy(s => s.mindate).Skip(start).Take(length);
                TBOrderStateListModel list = new TBOrderStateListModel();
                list.draw = draw;
                list.recordsFiltered = count;
                list.SearchModel = search;
                list.data = result.ToList();
                return list;
            }

            //模糊查找
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                var result = (
     from x in (
    (from a in db.TBOrderStates
     join b in db.Customers on a.CustomerID equals b.CustomerID
     group new { a, b } by new
     {
         a.OrderSourseID,
         b.CustomerTBCode,
         a.CustomerID
     } into g
     select new
     {
         OrderSourseID = g.Key.OrderSourseID,
         g.Key.CustomerTBCode,
         CustomerID = g.Key.CustomerID,
         mindate = g.Min(p => p.a.OrderDate),
         maxdate = g.Max(p => p.a.OrderDate),
         sendtime = g.Max(p => p.a.SendTime),
         IsSend = g.Min(p => p.a.IsSend ? 1 : 0)
     }))
     where
       x.IsSend == 1 && ((((DateValue == 1 && x.mindate >= date &&
                               x.mindate < nextdate) || (DateValue == 2 && x.maxdate >= date &&
                               x.maxdate < nextdate)) &&
       x.OrderSourseID == OrderSourseID && string.IsNullOrEmpty(FuzzySearch)) || (!string.IsNullOrEmpty(FuzzySearch) && x.CustomerTBCode.Contains(FuzzySearch)))
     select new TBOrderStateModel
     {
         OrderSourseID = x.OrderSourseID,
         OrderSourseName = ((from f in db.OrderSourses
                             where
                  f.OrderSourseID == x.OrderSourseID
                             select new
                             {
                                 f.OrderSourseName
                             }).FirstOrDefault().OrderSourseName),
         CustomerTBCode = x.CustomerTBCode,
         mindate = x.mindate,
         maxdate = x.maxdate,
         sendtime = x.sendtime,
         IsSend = x.IsSend,
         SendUserName =
         ((from f in db.TBOrderStates
           where
f.CustomerID == x.CustomerID &&
f.OrderSourseID == x.OrderSourseID
           orderby
f.SendTime descending
           select new
           {
               SendUserName = string.IsNullOrEmpty(f.SendUserName) ? "" : f.SendUserName
           }).Take(1).FirstOrDefault().SendUserName)
     }
 ).Union
 (
     from x in (
                            (from a in db.TBOrderStates
                             join b in db.Customers on a.CustomerID equals b.CustomerID
                             where
                               !a.IsSend
                             group new { a, b } by new
                             {
                                 a.OrderSourseID,
                                 b.CustomerTBCode
                             } into g
                             select new
                             {
                                 OrderSourseID = g.Key.OrderSourseID,
                                 g.Key.CustomerTBCode,
                                 mindate = g.Min(p => p.a.OrderDate),
                                 maxdate = g.Max(p => p.a.OrderDate)
                             }))
     where
(((DateValue == 1 && x.mindate >= date &&
                               x.mindate < nextdate) || (DateValue == 2 && x.maxdate >= date &&
                               x.maxdate < nextdate)) &&
       x.OrderSourseID == OrderSourseID && string.IsNullOrEmpty(FuzzySearch)) || (!string.IsNullOrEmpty(FuzzySearch) && x.CustomerTBCode.Contains(FuzzySearch))
     select new TBOrderStateModel
     {
         OrderSourseID = x.OrderSourseID,
         OrderSourseName = ((from f in db.OrderSourses
                             where
                  f.OrderSourseID == x.OrderSourseID
                             select new
                             {
                                 f.OrderSourseName
                             }).FirstOrDefault().OrderSourseName),
         CustomerTBCode = x.CustomerTBCode,
         mindate = x.mindate,
         maxdate = x.maxdate,
         sendtime = DateTimeOffset.MinValue,
         IsSend = 0,
         SendUserName = "",
     }
 );
                int count = result.Count();
                result = result.OrderBy(s => s.mindate).Skip(start).Take(length);
                TBOrderStateListModel list = new TBOrderStateListModel();
                list.draw = draw;
                list.recordsFiltered = count;
                list.SearchModel = search;
                list.data = result.ToList();
                return list;
            }
            TBOrderStateListModel nulllist = new TBOrderStateListModel();
            nulllist.draw = draw;
            nulllist.recordsFiltered = 0;
            nulllist.SearchModel = search;
            nulllist.data = new List<TBOrderStateModel>();
            return nulllist;
        }
    }
}

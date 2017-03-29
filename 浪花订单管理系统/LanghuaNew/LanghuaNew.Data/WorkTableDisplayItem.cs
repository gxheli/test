using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    public class WorkTableDisplayItem
    {
        public int WorkTableDisplayItemID { get; set; }
        public int UserID { get; set; }
        public User users { get; set; }
        //我的订单
        public bool MyNotfilledCount { get; set; }
        public bool MyFilledCount { get; set; }
        public bool MyNoPayCount { get; set; }
        public bool MySencondFullCount { get; set; }
        public bool MyTodayOrderCount { get; set; }
        public bool MyTodaySales { get; set; }
        public bool MyTodayProfits { get; set; }

        //整店订单
        public bool TodayOrderCount { get; set; }
        public bool OnCheckCount { get; set; }
        public bool CheckCount { get; set; }
        public bool NeedServiceCount { get; set; }
        public bool TodayTravelNum { get; set; }
        public bool TodayProfits { get; set; }
        public bool TodaySales { get; set; }
        public bool ThisMonthTravelNum { get; set; }
        public bool WeixinBindCount { get; set; }
    }
}

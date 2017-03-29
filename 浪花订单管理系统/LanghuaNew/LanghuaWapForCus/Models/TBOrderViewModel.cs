using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaWapForCus.Models
{
    public class TBOrderViewModel
    {
        public int TBOrderID { get; set; }
        public bool IsCommit { get; set; }
        public virtual Customer customer { get; set; }
        public List<OrderViewModel> orders { get; set; }

    }
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        //public string Elements { get; set; }
        public string ElementsValue { get; set; }
        public string ServiceItemTemplteValue { get; set; }
        public DateTimeOffset TravelDate { get; set; }
        public DateTimeOffset ReturnDate { get; set; }
        public List<OrderTraveller> travellers { get; set; }
    }
}
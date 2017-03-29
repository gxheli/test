using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ProductModel
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public IEnumerable<Product> data { get; set; }
        public ShareSearchModel SearchModel { get; set; }
    }
    public class Product
    {
        public string cnItemName { get; set; }
        public string enItemName { get; set; }
        public string TravelCompany { get; set; }
        public string ServiceTypeName { get; set; }
    }
    public class StatisticsSales
    {
        public string ServiceCode { get; set; }
        public string SupplierNo { get; set; }
        public string SupplierName { get; set; }
        public string ServiceName { get; set; }
        public int OrderCount { get; set; }
        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int INFNum { get; set; }
    }
}

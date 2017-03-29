using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
  
  public  class Element
    {
        public int ElementID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string title { get; set; }

        public List<item> items { get; set; }

        public bool withNull { get; set; }
        public bool admin { get; set; }

      


    }
  public class item
    {
        public string itemID { get; set; }
        public string text { get; set; }
        public string value { get; set; }
    }
}

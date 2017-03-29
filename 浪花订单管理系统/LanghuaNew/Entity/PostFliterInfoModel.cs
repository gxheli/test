using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class PostFliterInfoModel
    {
        public string FilterDeparture { get; set; }
        public string FilterArrival { get; set; }

        public List<FliterInfo> FliterInfos { get; set; }
    }
}

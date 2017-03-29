using LanghuaNew.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commond.Splider
{
    public class FliterInfoComparer : IEqualityComparer<FliterInfo>
    {
        public bool Equals(FliterInfo x, FliterInfo y)
        {
            return x.FliterNum == y.FliterNum && x.DepartureCity == y.DepartureCity && x.FilterDeparture == y.FilterDeparture && 
                x.ArrivalCity == y.ArrivalCity && x.FilterArrival == y.FilterArrival;
        }

        public int GetHashCode(FliterInfo obj)
        {
            return obj.FliterNum.GetHashCode() ^ obj.DepartureCity.GetHashCode() ^ obj.FilterDeparture.GetHashCode() ^ obj.ArrivalCity.GetHashCode() ^ obj.FilterArrival.GetHashCode();
        }
    }
}

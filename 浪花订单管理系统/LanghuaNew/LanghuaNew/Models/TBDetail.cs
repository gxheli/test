using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaNew.Models
{
    public class TBDetail : Top.Api.Response.AlitripTravelTradeQueryResponse.TopTripOrderResultDomain
    {
        public string PicPath { get; set; }
    }
}
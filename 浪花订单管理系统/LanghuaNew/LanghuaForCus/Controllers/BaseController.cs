using Commond;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForCus.Controllers
{
    public class BaseController : Controller
    {
        //（全局可用）根据登录用户获取用户名与微信二维码图片
        public BaseController()
        {
            var userFromAuthCookie = System.Threading.Thread.CurrentPrincipal;
            string TBID = string.Empty;
            if (userFromAuthCookie != null && userFromAuthCookie.Identity.IsAuthenticated)
            {
                TBID = userFromAuthCookie.Identity.Name;
            }
            if (!string.IsNullOrEmpty(TBID))
            {
                JObject open = JsonConvert.DeserializeObject<JObject>(HttpHelper.GetActionForOdata("odata/Customers?$filter=CustomerTBCode eq '" + TBID + "'").Result);
                string str = open["value"].ToString();
                List<Customer> Customers = JsonConvert.DeserializeObject<List<Customer>>(str);
                Customer customer = (Customers != null && Customers.Count > 0) ? Customers[0] : null;
                //OpenID为空表示未绑定微信，未绑定微信的需要显示微信二维码图片
                if (customer != null && string.IsNullOrEmpty(customer.OpenID))
                {
                    var CustomerID = customer.CustomerID;
                    ViewBag.ImageUrl = WeiXinHelper.GetImageUrlByID(CustomerID, systemType.costomer);
                }
            }
            ViewBag.TBID = TBID;
        }

    }
}
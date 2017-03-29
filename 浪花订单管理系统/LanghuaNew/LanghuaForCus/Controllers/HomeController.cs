using Commond;
using LanghuaNew.Data;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForCus.Controllers
{
    public class HomeController : BaseController
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //个人中心
        public async Task<ActionResult> Index()
        {
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Expand(u=>u.Travellers)
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();
            return View(customer);
        }
    }
}
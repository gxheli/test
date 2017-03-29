using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanghuaNew.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace LanghuaNew.Controllers.Tests
{
    [TestClass()]
    public class ServiceItemsControllerTests
    {
        [TestMethod()]
        public async Task GetItemPricesTest()
        {
            try
            {
                ShareSearchModel share = new ShareSearchModel();
                share.ItemPriceSearch = new ItemPriceSearchModel();
                share.ItemPriceSearch.FuzzySearch = "spa";
                ServiceItemsController server = new ServiceItemsController();
                var temp = await server.GetItemPrices(share);
            }
            catch(Exception ex)
            {
                var a = ex.Message;
            }
            Assert.Fail();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WXData
{
    public  class WeiXinContent : DbContext
    {
        public WeiXinContent()
            : base("name=WeiXinContent")
        {
        }

        public DbSet<LHNew> LHNews { get; set; }

        public DbSet<LHArticle> LHArticles { get; set; }

        public DbSet<WeiXinMenu> WeiXinMenus { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }
    }
}

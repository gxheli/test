using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXData
{
    /// <summary>
    /// 素材内容
    /// </summary>
    public class LHArticle
    {
        public int LHArticleID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Url { get; set; }
        public int LHNewID { get; set; }
        [JsonIgnore]
        public virtual LHNew LHNew{ get; set; }
    }
}

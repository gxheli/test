using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXData
{
    /// <summary>
    /// 素材库
    /// </summary>
    public class LHNew
    {
        public int LHNewID { get; set; }
        public string media_id { get; set; }
        /// <summary>
        /// 这个素材的最后更新时间
        /// </summary>
        public DateTime update_time { get; set; }
        /// <summary>
        /// 素材内容
        /// </summary>
        public List<LHArticle> Articles { get; set; }
    }
}

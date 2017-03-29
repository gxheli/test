using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 淘宝授权
    /// </summary>
    public class TB_Access_Token
    {
        public int ID { get; set; }
        [StringLength(100)]
        public string access_token { get; set; }

        public double refresh_token_valid_time { get; set; }

        //Refresh token，可用来刷新access_token
        [StringLength(100)]
        public string refresh_token { get; set; }

        //Access token的类型目前只支持bearer
        [StringLength(30)]
        public string token_type { get; set; }

        //10（表示10秒后过期）	r2级别API或字段的访问过期时间
        public int r2_expires_in { get; set; }

        public double w1_valid { get; set; }

        public double r2_valid { get; set; }

        public double r1_valid { get; set; }

        //10（表示10秒后过期）	w1级别API或字段的访问过期时间
        public int w1_expires_in { get; set; }

        //10（表示10秒后过期）	w2级别API或字段的访问过期时间
        public int w2_expires_in { get; set; }

        public double w2_valid { get; set; }

        //10（表示10秒后过期）	r1级别API或字段的访问过期时间
        public int r1_expires_in { get; set; }
        //过期时间
        public double expire_time { get; set; }

        public int expires_in { get; set; }

        public int re_expires_in { get; set; }

        //测试账号    淘宝账号
        [StringLength(50)]
        public string taobao_user_nick { get; set; }
        //   string	706388888	淘宝帐号对应id

        [StringLength(30)]
        public string taobao_user_id { get; set; }

    }
}

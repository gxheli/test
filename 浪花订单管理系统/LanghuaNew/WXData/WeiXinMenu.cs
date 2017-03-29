using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXData
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 图文素材
        /// </summary>
        PictrueMenu,
        /// <summary>
        /// 纯文本
        /// </summary>
        TextMenu,
        /// <summary>
        /// 超级链接
        /// </summary>
        ViewMenu,
       
    }
    /// <summary>
    /// 二级菜单
    /// </summary>
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 菜单值.如果是网址,请带上http://
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 菜单显示文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType ItemType { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int RowNo { get; set; }
        /// <summary>
        /// 父菜单
        /// </summary>
        public int WeiXinMenuID { get; set; }

        public virtual WeiXinMenu WeiXinMenuValue { get; set; }
    }
    /// <summary>
    /// 一级菜单
    /// </summary>
    public class WeiXinMenu
    {
        public int WeiXinMenuID { get; set; }
        /// <summary>
        /// 一级菜单名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 二级子菜单
        /// </summary>
        public  List<MenuItem> Items { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int RowNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commond
{
    public class OfficeHelper
    {
        /// <summary>
        /// 根据html的内容生成html
        /// </summary>
        /// <param name="htmlContent">html内容</param>
        /// <param name="pyPath">html的物理路径</param>
        /// <returns>返回http相对路径</returns>
        public static string CreateHtml(string htmlContent, ref string pyPath)
        {
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/data/PdfHtml/");
            if (!Directory.Exists(savePath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(savePath);//创建文件夹
            }
            string htmlName = Guid.NewGuid().ToString() + ".html";
            StringBuilder htmltext = new StringBuilder();
            using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/data/PdfHtml/Template.html")))//模板页路径
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    htmltext.Append(line);
                }
                sr.Close();
            }
            htmltext.Replace("$htmlformat[0]", htmlContent);
            string html_Path = System.Web.HttpContext.Current.Server.MapPath("~/data/PdfHtml/" + htmlName);
            using (StreamWriter sw = new StreamWriter(html_Path, false, System.Text.Encoding.GetEncoding("UTF-8"))) //保存地址
            {
                sw.WriteLine(htmltext);
                sw.Flush();
                sw.Close();
            }

            pyPath = AppDomain.CurrentDomain.BaseDirectory + "data/PdfHtml/" + htmlName;

            return "/data/PdfHtml/" + htmlName;
        }

        /// <summary>
        /// 将文件流写入内存
        /// </summary>
        /// <param name="htmlPath"></param>
        /// <param name="docPath"></param>
        /// <returns></returns>
        public static MemoryStream WriteToClient(string docPath)
        {
            byte[] data = System.IO.File.ReadAllBytes(docPath);
            using (MemoryStream ms = new MemoryStream(data))
            {
                DeleteFile(docPath);
                return ms;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeleteFile(string path)
        {
            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="filePhysicalPath"></param>
        public static void CreateDirectory(string filePhysicalPath)
        {
            if (!Directory.Exists(filePhysicalPath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(filePhysicalPath);//创建文件夹
            }
        }

        public static void DelteAgoFiles(string path)
        {
            DirectoryInfo dir = Directory.GetParent(path);
            FileInfo[] files=dir.GetFiles("*.pdf");
            //files.Select(p => DateTime.Parse(p.Name.Trim(".pdf".ToCharArray())) < DateTime.Now.AddDays(-1));
            foreach(FileInfo f in files)
            {
                f.Delete();
            }
        }
    }
}

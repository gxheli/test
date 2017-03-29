using LanghuaForSup.App_Code;
using System.Web;
using System.Web.Optimization;

namespace LanghuaForSup
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            var basicStyle = new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      //"~/Content/bootstrap-theme.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/ring.css",
                      "~/Content/fixedForLanghua.css",
                      "~/Content/langhua.css",
                      "~/Content/animate.min.css"
                 );
            basicStyle.Transforms.Add(new VersionBundleTransform());
            bundles.Add(basicStyle);

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/Site.css",
                    "~/Content/font-awesome.css"
                    ));


            //Langhua
            var LangHua = new ScriptBundle("~/bundles/LangHua").Include(
                    "~/Scripts/LangHua/LangHua.jquery.js",
                   "~/Scripts/LangHua/LangHua.common.js",
                   "~/Scripts/LangHua/notification.js",
                   "~/Scripts/LangHua/LangHua.layout.js");
            LangHua.Transforms.Add(new VersionBundleTransform());
            bundles.Add(LangHua);


            //modalExtend
            bundles.Add(new StyleBundle("~/Content/plugins/modalCss").Include(
                  "~/Content/plugins/modalCss/bootstrap-modal-bs3patch.css",
                  "~/Content/plugins/modalCss/bootstrap-modal.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/modalJs").Include(
                     "~/Scripts/plugins/modalJs/bootstrap-modalmanager.js",
                     "~/Scripts/plugins/modalJs/bootstrap-modal.js"));

            //dataTables
            bundles.Add(new StyleBundle("~/Content/plugins/dataTablesCss").Include(
                  "~/Content/plugins/dataTablesCss/dataTables.bootstrap.min.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/dataTablesJs").Include(
                     "~/Scripts/plugins/dataTablesJs/jquery.dataTables.min.js",

                     "~/Scripts/plugins/dataTablesJs/dataTables.bootstrap.js",
                     "~/Scripts/plugins/dataTablesJs/datatables.langhua.js"
                     ));

            //datepicker
            bundles.Add(new StyleBundle("~/Content/plugins/datePickerCss").Include(
                  "~/Content/plugins/datePickerCss/bootstrap-datepicker3.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/datePickerJs").Include(
                     "~/Scripts/plugins/datePickerJs/bootstrap-datepicker.min.js",
                     "~/Scripts/plugins/datePickerJs/bootstrap-datepicker.zh-CN.min.js"
                     ));


            //timePicker
            bundles.Add(new StyleBundle("~/Content/plugins/timePickerCss").Include(
                  "~/Content/plugins/timePickerCss/bootstrap-timepicker.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/timePickerJs").Include(
                     "~/Scripts/plugins/timePickerJs/bootstrap-timepicker.min.js"
                     ));

            //Typeahead
            bundles.Add(new StyleBundle("~/Content/plugins/typeAheadCss").Include(
                  "~/Content/plugins/typeAheadCss/typeahead.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/typeAheadJs").Include(
                     "~/Scripts/plugins/typeAheadJs/handlebars.min.js",
                     "~/Scripts/plugins/typeAheadJs/typeahead.bundle.min.js"
                     ));

            //ZeroCopy
            bundles.Add(new ScriptBundle("~/bundles/plugins/zeroCopy").Include(
                     "~/Scripts/plugins/zeroCopy/jquery.zeroclipboard.min.js"
                     ));

            //htmlToCanvas
            bundles.Add(new ScriptBundle("~/bundles/plugins/htmlToCanvas").Include(
                     "~/Scripts/plugins/htmlToCanvas/html2canvas.js"
                     ));

            //Typeahead
            bundles.Add(new StyleBundle("~/Content/plugins/typeAheadCss").Include(
                  "~/Content/plugins/typeAheadCss/typeahead.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/typeAheadJs").Include(
                     "~/Scripts/plugins/typeAheadJs/handlebars.min.js",
                     "~/Scripts/plugins/typeAheadJs/typeahead.bundle.min.js"
                     ));


            //jqueryUI 
            bundles.Add(new ScriptBundle("~/bundles/plugins/jqueryUIJs").Include(
                      "~/Scripts/plugins/jqueryUIJs/jquery-ui.js"
                 ));
            bundles.Add(new StyleBundle("~/Content/plugins/jqueryUICss").Include(
                      "~/Content/plugins/jqueryUICss/jquery-ui.css",
                      "~/Content/plugins/jqueryUICss/jquery-ui.structure.css"
                 ));

            //es support
            bundles.Add(new ScriptBundle("~/bundles/essupport").Include(
                     "~/Scripts/react_components/IE8/es5-shim.min.js",
                     "~/Scripts/react_components/IE8/es5-sham.min.js",
                     "~/Scripts/react_components/IE8/console-polyfill.js"
              
                ));

            BundleTable.EnableOptimizations = false;












        }
    }
}

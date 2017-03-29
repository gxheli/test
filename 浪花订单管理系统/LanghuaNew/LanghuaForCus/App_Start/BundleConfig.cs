using System.Web;
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace LanghuaForCus
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

           

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));


            var basicStyle = new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.min.css",
                      "~/Content/Site.css",
                      //"~/Content/bootstrap-theme.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/ring.css",
                      "~/Content/fixedForLanghua.css",
                      "~/Content/langhua.css",
                      "~/Content/animate.min.css"
                 );
            basicStyle.Transforms.Add(new VersionBundleTransform());
            bundles.Add(basicStyle);

            //login
            bundles.Add(new StyleBundle("~/Content/login").Include(
                  "~/Content/login.css"
                  ));

            //Langhua
            var LangHua = new ScriptBundle("~/bundles/LangHua").Include(
                    "~/Scripts/LangHua/LangHua.jquery.js",
                   "~/Scripts/LangHua/LangHua.common.js",
                   "~/Scripts/LangHua/LangHua.layout.js");
            LangHua.Transforms.Add(new VersionBundleTransform());
            bundles.Add(LangHua);


            //modalExtend
            bundles.Add(new StyleBundle("~/Content/plugins/modal_Css").Include(
                  "~/Content/plugins/modalCss/bootstrap-modal-bs3patch.css",
                  "~/Content/plugins/modalCss/bootstrap-modal.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/modalJs").Include(
                     "~/Scripts/plugins/modalJs/bootstrap-modalmanager.js",
                     "~/Scripts/plugins/modalJs/bootstrap-modal.js"));

            //dataTables
            bundles.Add(new StyleBundle("~/Content/plugins/dataTables_Css").Include(
                  "~/Content/plugins/dataTablesCss/dataTables.bootstrap.min.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/dataTablesJs").Include(
                     "~/Scripts/plugins/dataTablesJs/jquery.dataTables.min.js",

                     "~/Scripts/plugins/dataTablesJs/dataTables.bootstrap.js",
                     "~/Scripts/plugins/dataTablesJs/datatables.langhua.js"
                     ));

            //datepicker
            bundles.Add(new StyleBundle("~/Content/plugins/datePicker_Css").Include(
                  "~/Content/plugins/datePickerCss/bootstrap-datepicker3.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/datePickerJs").Include(
                     "~/Scripts/plugins/datePickerJs/bootstrap-datepicker.min.js",
                     "~/Scripts/plugins/datePickerJs/bootstrap-datepicker.zh-CN.min.js"
                     ));


            //timePicker
            bundles.Add(new StyleBundle("~/Content/plugins/timePicker_Css").Include(
                  "~/Content/plugins/timePickerCss/bootstrap-timepicker.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/timePickerJs").Include(
                     "~/Scripts/plugins/timePickerJs/bootstrap-timepicker.min.js"
                     ));

            //Typeahead
            bundles.Add(new StyleBundle("~/Content/plugins/typeAhead_Css").Include(
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
            //bundles.Add(new StyleBundle("~/Content/plugins/typeAhead_Css").Include(
            //      "~/Content/plugins/typeAheadCss/typeahead.css"
            //      ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/typeAheadJs").Include(
                     "~/Scripts/plugins/typeAheadJs/handlebars.min.js",
                     "~/Scripts/plugins/typeAheadJs/typeahead.bundle.min.js"
                     ));


            //jqueryUI 
            bundles.Add(new ScriptBundle("~/bundles/plugins/jqueryUIJs").Include(
                      "~/Scripts/plugins/jqueryUIJs/jquery-ui.js"
                 ));
            bundles.Add(new StyleBundle("~/Content/plugins/jqueryUI_Css").Include(
                      "~/Content/plugins/jqueryUICss/jquery-ui.css",
                      "~/Content/plugins/jqueryUICss/jquery-ui.structure.css"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/plugins/bootboxJs").Include(
                     "~/Scripts/plugins/bootboxJs/bootbox.min.js"));


           
            //react
            var reactbase =
                (new ScriptBundle("~/bundles/reactbase").Include(
                      "~/Scripts/react_components/react.min.js",
                      "~/Scripts/react_components/react-dom.min.js"
                ));
            reactbase.Transforms.Add(new VersionBundleTransform());
            bundles.Add(reactbase);
            var reactcomponent =
                (new BabelBundle("~/bundles/reactcomponents").Include(
                      "~/Scripts/react_components/Mixin.jsx",
                      "~/Scripts/react_components/titleRechange.jsx",
                      "~/Scripts/react_components/title.jsx",
                      "~/Scripts/react_components/DatePicker.jsx",
                      "~/Scripts/react_components/PersonPicker.jsx",
                      "~/Scripts/react_components/FlightNo.jsx",
                      "~/Scripts/react_components/Hotel_name_zone_tel.jsx",
                      "~/Scripts/react_components/Date_setoutdate_returndate.jsx",
                      "~/Scripts/react_components/Flight_takeofftime_arrivaltime.jsx",
                      "~/Scripts/react_components/Flight_takeofftime_pickuptime.jsx",
                      "~/Scripts/react_components/Destination.jsx",
                      "~/Scripts/react_components/HotelName.jsx",
                      "~/Scripts/react_components/BusRoute.jsx",
                      "~/Scripts/react_components/TEXTinput.jsx",
                      "~/Scripts/react_components/SelectMenu.jsx",
                      "~/Scripts/react_components/ServiceTime.jsx",
                      "~/Scripts/react_components/allcomponents.jsx",
                      "~/Scripts/react_components/BaseInfo.jsx"

                ));
            reactcomponent.Transforms.Add(new VersionBundleTransform());
            bundles.Add(reactcomponent);


            //es support
            bundles.Add(new ScriptBundle("~/bundles/essupport").Include(
                     "~/Scripts/react_components/IE8/es5-shim.min.js",
                     "~/Scripts/react_components/IE8/es5-sham.min.js",
                     "~/Scripts/react_components/IE8/console-polyfill.js"
                //"~/Scripts/essupport/json3.min.js",
                //"~/Scripts/essupport/es6-shim.min.js",
                //"~/Scripts/essupport/es6-sham.min.js",
                //"~/Scripts/essupport/es7-shim.latest.js"
                ));





            BundleTable.EnableOptimizations = true;

        }
    }
}

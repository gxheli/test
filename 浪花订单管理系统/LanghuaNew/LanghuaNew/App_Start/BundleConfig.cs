using System.Web;
using System.Web.Optimization;

namespace LanghuaNew
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
                      "~/Scripts/respond.js",
                      "~/Scripts/react_components/axios.min.js"));


            var basicStyle = new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      //"~/Content/bootstrap-theme.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/ring.css",
                      "~/Content/fixedForLanghua.css",
                      "~/Content/langhua.css",
                      "~/Content/new.css",
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
                   "~/Scripts/LangHua/LangHua.layout.js",
                   "~/Scripts/LangHua/notification.js");
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
                  "~/Content/plugins/dataTablesCss/datatables.min.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/plugins/dataTablesJs").Include(
                     "~/Scripts/plugins/dataTablesJs/jszip.min.js",
                     "~/Scripts/plugins/dataTablesJs/datatables.min.js",
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

            bundles.Add(new ScriptBundle("~/bundles/plugins/bootboxJs").Include(
                     "~/Scripts/plugins/bootboxJs/bootbox.min.js"));


            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                   "~/Content/bootstrap.css",
                   "~/Content/Site.css",
                   "~/Content/font-awesome.css"
                   ));

            //react
            var reactbase =
                (new ScriptBundle("~/bundles/reactbase").Include(
                      "~/Scripts/react_components/react.js",
                      "~/Scripts/react_components/react-dom.js"
                ));
            reactbase.Transforms.Add(new VersionBundleTransform());
            bundles.Add(reactbase);
            var reactcomponent=
                (new ScriptBundle("~/bundles/reactcomponents").Include(
                      "~/Scripts/react_components/react-copy-to-clipboard.js",
                      "~/Scripts/react_components/autosuggest.js",
                      "~/Scripts/react_components/Mixin.jsx",
                      "~/Scripts/react_components/title.jsx",
                      "~/Scripts/react_components/titleRechange.jsx",
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
                      "~/Scripts/react_components/Copy.jsx",
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

            BundleTable.EnableOptimizations = false;

        }
    }
}

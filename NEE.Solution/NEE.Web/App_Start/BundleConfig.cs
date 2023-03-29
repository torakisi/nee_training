using System.Web;
using System.Web.Optimization;

namespace NEE.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/neeApplication").Include(
                "~/Scripts/neeApplication/app.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*")
            );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*")
            );

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-select.js",
                "~/Scripts/respond.js")
            );


            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/app.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
               "~/Scripts/kendo/2019.1.115/kendo.ui.core.min.js",
               "~/Scripts/kendo/2019.1.115/cultures/kendo.culture.el-GR.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/editSupport").Include(
                "~/Scripts/app/editSupport.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/editManualContractSupport").Include(
            "~/Scripts/app/editManualContractSupport.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/devEx").Include(
               "~/Scripts/devEx/dxDashboard.js")
           );

            bundles.Add(new StyleBundle("~/Content/dxstyles")
               .Include("~/Content/dxstyles.css?v=3")
           );

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/app-bootstrap.css")
                .Include("~/Content/bootstrap-select.css")
                .Include("~/Content/site.css")
            );


            bundles.Add(new StyleBundle("~/Content/css/font-awesome")
                //.Include("~/Content/fontawesome/font-awesome.css")
                .Include("~/Content/fontawesome/font-awesome.min.css")
            );

            bundles.Add(new StyleBundle("~/Content/kendo/2019.1.115/css").Include(
                  "~/Content/kendo/2019.1.115/kendo.common.min.css",
                  "~/Content/kendo/2019.1.115/kendo.common-bootstrap.core.min.css",
                  "~/Content/kendo/2019.1.115/kendo.bootstrap.min.css"));


        }
    }
}

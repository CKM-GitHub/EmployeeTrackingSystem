using System.Web;
using System.Web.Optimization;

namespace EmployeeTrackingSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-1.13.2.js")); // Load UI right after jQuery));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.js",                 // Load Bootstrap first
            "~/Scripts/moment.min.js",                // Load Moment second
            "~/Scripts/select2.min.js",               // Load Select2 third
            "~/Scripts/bootstrap-datetimepicker.js"   // Load Datetimepicker last
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
            "~/Content/css/select2.min.css",
            "~/Content/bootstrap-datetimepicker.min.css",
            "~/Content/themes/base/jquery-ui.css",     
            "~/Content/site.css"));
        }
    }
}

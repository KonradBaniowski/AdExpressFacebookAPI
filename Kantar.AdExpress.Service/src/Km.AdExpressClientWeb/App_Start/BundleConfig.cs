using System.Web.Optimization;

namespace Km.AdExpressClientWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/media-selection").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/media-selection/media.js"));
            bundles.Add(new ScriptBundle("~/bundles/media-selection-single").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/media-selection/media-single.js"));
            bundles.Add(new ScriptBundle("~/bundles/media-compare-group").Include(
                        "~/Scripts/media-selection/add-tree.js"));


            bundles.Add(new ScriptBundle("~/bundles/module-selection").Include(
                        "~/Scripts/module-selection/module-selection.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/bootstrap-select.js",
                        "~/Scripts/spin.js",
                        "~/Scripts/bootbox.js"));

            bundles.Add(new ScriptBundle("~/bundles/period-selector").Include(
                      "~/Scripts/period-selector.js"));

            bundles.Add(new ScriptBundle("~/bundles/universe").Include(
                       "~/Scripts/universe/listbox.js",
                       "~/Scripts/universe/universe.js"));

            bundles.Add(new ScriptBundle("~/bundles/universe-open").Include(
                       "~/Scripts/universe/listbox.js",
                       "~/Scripts/universe/universe-open.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/icon-kantar.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/themes/base/base.css",
                      "~/Content/themes/base/theme.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/treegrid.css",
                      "~/Content/options-control.css"));
        }
    }
}

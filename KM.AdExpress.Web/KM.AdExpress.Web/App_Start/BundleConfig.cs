using System.Web;
using System.Web.Optimization;

namespace KM.AdExpress.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/custom/app.min.js",
                      "~/Scripts/custom/app.src.js",
                      "~/Scripts/custom/ui-accordion.js",
                      "~/Scripts/custom/ui-client.js",
                      "~/Scripts/custom/ui-count.js",
                      "~/Scripts/custom/ui-jp.config.js",
                      "~/Scripts/custom/ui-jp.js",
                      "~/Scripts/custom/ui-load.js",
                      "~/Scripts/custom/ui-nav.js",
                      "~/Scripts/custom/ui-toggle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/animate.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/app.css",
                      "~/Content/css/count.css",
                      "~/Content/css/font.css"));

        }
    }
}

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
                      "~/Scripts/jquery-{version}.js",
                      "~/libs/jquery/jquery/dist/jquery.js",
                      "~/libs/jquery/bootstrap/dist/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/js/app.min.js",
                      "~/Scripts/js/app.src.js",
                      "~/Scripts/js/ui-accordion.js",
                      "~/Scripts/js/ui-client.js",
                      "~/Scripts/js/ui-count.js",
                      "~/Scripts/js/ui-jp.config.js",
                      "~/Scripts/js/ui-jp.js",
                      "~/Scripts/js/ui-load.js",
                      "~/Scripts/js/ui-nav.js",
                      "~/Scripts/js/ui-toggle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/app.css",
                      "~/Content/css/count.css",
                      "~/Content/css/font.css"));

            bundles.Add(new StyleBundle("~/lib/css").Include(
                      "~/libs/assets/animate.css/animate.css",
                      "~/libs/assets/font-awesome/css/font-awesome.min.css",
                      "~/libs/assets/simple-line-icons/css/simple-line-icons.css",
                      "~/libs/jquery/bootstrap/dist/css/bootstrap.css"));

        }
    }
}

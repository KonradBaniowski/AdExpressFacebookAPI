using System.Web.Optimization;

namespace Km.AdExpressClientWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;
            #region jQuery & Bootstrap bundles
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                        "~/Scripts/app/layout.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            #endregion

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/countUp.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/bootstrap-select.js",
                        "~/Scripts/spin.js",
                        "~/Scripts/bootbox.js"));

            bundles.Add(new ScriptBundle("~/bundles/period-selector").Include(
                      "~/Scripts/period-selector.js"));

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
                      "~/Content/options-control.css",
                      "~/Content/spinner.css"));

            bundles.Add(new ScriptBundle("~/bundles/module-selection").Include(
                  "~/Scripts/module-selection/module-selection.js"));
            bundles.Add(new ScriptBundle("~/bundles/account").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/Account/account.js"));

            bundles.Add(new ScriptBundle("~/bundles/myAdExpress").Include(
                        "~/Scripts/Account/myAdExpress.js"));

            #region Javascript for Market & Media
            #region Plan Media
            bundles.Add(new ScriptBundle("~/bundles/planmedia-market").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-required.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-market.js"));

            bundles.Add(new ScriptBundle("~/bundles/planmedia-media").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-optional.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-media.js",
                        "~/Scripts/media/media-media.js"));
            #endregion
            #region Portfolio
            bundles.Add(new ScriptBundle("~/bundles/portfolio-market").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-unrequired.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-market.js"));

            bundles.Add(new ScriptBundle("~/bundles/portfolio-media").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/media/media-portfolio.js",
                        "~/Scripts/universe/universe-media.js"));
            #endregion
            #region LostWon
            bundles.Add(new ScriptBundle("~/bundles/lostWon-market").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-unrequired.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-market.js"));

            bundles.Add(new ScriptBundle("~/bundles/lostwon-media").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/media/media-lost-won.js",
                        "~/Scripts/universe/universe-media.js"));
            #endregion
            #region PresentAbsent
            bundles.Add(new ScriptBundle("~/bundles/presentabsent-market").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-unrequired.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-market.js"));

            bundles.Add(new ScriptBundle("~/bundles/presentabsent-media").Include(
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/media/media-present-absent.js",
                        "~/Scripts/media/add-tree.js",
                        "~/Scripts/universe/universe-media.js"));
            #endregion
            #endregion

            #region Javascript Event for detail Selection 
            bundles.Add(new ScriptBundle("~/bundles/detail-selection").Include(
                        "~/Scripts/detail-selection/detail-selection.js"));
            #endregion
        }
    }
}

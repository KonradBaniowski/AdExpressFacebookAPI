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
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/selectableScroll.js"));

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
                        "~/Scripts/bootbox.js",
                        "~/Scripts/tree-view/tree-view.js"));

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
                      "~/Content/treeview.css",
                      "~/Content/options-control.css",
                      "~/Content/spinner.css"));


            #region CSS spécifique par langues
            //Francais 
            bundles.Add(new StyleBundle("~/Content/css/33/FR"));

            //Anglais 
            bundles.Add(new StyleBundle("~/Content/css/33/EN"));
            bundles.Add(new StyleBundle("~/Content/css/35/EN").Include(
                "~/Content/site-Fi_en.css"
            ));

            // Finnois
            bundles.Add(new StyleBundle("~/Content/css/35/FI").Include(
                "~/Content/site-Fi.css"
            ));
            #endregion

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

            bundles.Add(new ScriptBundle("~/bundles/portfolio-vehicle-view").Include(
                       "~/Scripts/Results/portfolio-vehicle-view.js"));
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

            bundles.Add(new ScriptBundle("~/bundles/results").Include(
                        "~/Scripts/Results/result.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/language-choice").Include(
                  "~/Scripts/app/language-choice.js"));

            bundles.Add(new ScriptBundle("~/bundles/analysis").Include(
                        "~/Scripts/market/market-analysis.js",
                        "~/Scripts/universe/listbox.js",
                        "~/Scripts/universe/universe-loading.js",
                        "~/Scripts/market/market-required.js",
                        "~/Scripts/component-selectable/common-actions.js",
                        "~/Scripts/universe/universe-market.js"));

            bundles.Add(new ScriptBundle("~/bundles/analysis-results").Include(
                        //"~/Scripts/Results/result.js",
                        "~/Scripts/Results/result-analysis.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/detail-selection/detail-selection.js",
                        "~/Scripts/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/newCreatives-results").Include(
            "~/Scripts/Results/result-newCreatives.js",
            "~/Scripts/bootstrap-datetimepicker.js",
            "~/Scripts/detail-selection/detail-selection.js"));

            bundles.Add(new ScriptBundle("~/bundles/advertisingAgency-results").Include(
            "~/Scripts/Results/result-advertisingAgency.js",
            "~/Scripts/bootstrap-datetimepicker.js",
            "~/Scripts/detail-selection/detail-selection.js"));

            bundles.Add(new ScriptBundle("~/bundles/campaignAnalysis-results").Include(
            "~/Scripts/Results/result-campaignAnalysis.js",
            "~/Scripts/bootstrap-datetimepicker.js",
            "~/Scripts/detail-selection/detail-selection.js"));

            bundles.Add(new ScriptBundle("~/bundles/socialMedia-results").Include(
            "~/Scripts/Results/socialMedia-results.js"));

            bundles.Add(new ScriptBundle("~/bundles/socialMedia-creative").Include(
            "~/Scripts/creative/socialMediaCreative.js",
            "~/Scripts/creative/socialMediaZoom.js"));
        }
    }
}

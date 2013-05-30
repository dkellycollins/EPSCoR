using System.Web;
using System.Web.Optimization;

namespace EPSCoR
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/js/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                        "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/content/jqueryui").Include(
                        "~/Content/jQueryUI/jquery.ui.core.css",
                        "~/Content/jQueryUI/jquery.ui.resizable.css",
                        "~/Content/jQueryUI/jquery.ui.selectable.css",
                        "~/Content/jQueryUI/jquery.ui.accordion.css",
                        "~/Content/jQueryUI/jquery.ui.autocomplete.css",
                        "~/Content/jQueryUI/jquery.ui.button.css",
                        "~/Content/jQueryUI/jquery.ui.dialog.css",
                        "~/Content/jQueryUI/jquery.ui.slider.css",
                        "~/Content/jQueryUI/jquery.ui.tabs.css",
                        "~/Content/jQueryUI/jquery.ui.datepicker.css",
                        "~/Content/jQueryUI/jquery.ui.progressbar.css",
                        "~/Content/jQueryUI/jquery.ui.theme.css"));

            //Bootstrap bundles.

            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/bootstrap/bootstrap.js",
                "~/Scripts/jquery.validate.js",
                "~/scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                ));

            bundles.Add(new StyleBundle("~/content/bootstrap").Include(
                "~/Content/bootstrap/bootstrap.css",
                "~/Content/bootstrap/body.css",
                "~/Content/bootstrap/bootstrap-responsive.css",
                "~/Content/bootstrap/bootstrap-mvc-validation.css"
                ));

            //jQueryFileUpload bundles

            bundles.Add(new ScriptBundle("~/js/jQueryFileUpload").Include(
                ));

            bundles.Add(new StyleBundle("~/content/jQueryFileUpload").Include(
                ));
        }
    }
}
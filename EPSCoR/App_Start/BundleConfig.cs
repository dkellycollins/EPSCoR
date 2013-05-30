using System.Web;
using System.Web.Optimization;

namespace EPSCoR
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Custom bundles

            bundles.Add(new StyleBundle("~/content/css").Include(
                       "~/Content/Site.css"));

            //jQuery bundles

            bundles.Add(new ScriptBundle("~/scripts/jquery/jquery").Include(
                        "~/Scripts/jQuery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery/jqueryval").Include(
                        "~/Scripts/jQuery/jquery.validate.js",
                        "~/scripts/jQuery/jquery.validate.unobtrusive.js",
                        "~/Scripts/jQuery/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                        ));

            bundles.Add(new ScriptBundle("~/scripts/jquery/jquery-migrate").Include(
                        "~/Scripts/jQuery/jquery-migrate-{version}.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/scripts/modernizr/modernizr").Include(
                        "~/Scripts/Modernizr/modernizr-*"));

            //jQuery ui bundles

            bundles.Add(new ScriptBundle("~/scripts/jquery/jqueryui-js").Include(
                        "~/Scripts/jQuery/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/content/jqueryui/jqueryui-css").Include(
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
                        "~/Content/jQueryUI/jquery.ui.theme.css"
                        ));

            //Bootstrap bundles

            bundles.Add(new ScriptBundle("~/scripts/bootstrap/bootstrap-js").Include(
                "~/Scripts/jQuery/jquery-{version}.js",
                "~/Scripts/jQuery/jquery-migrate-{version}.js",
                "~/Scripts/Bootstrap/bootstrap.js",
                "~/Scripts/jQuery/jquery.validate.js",
                "~/scripts/jQuery/jquery.validate.unobtrusive.js",
                "~/Scripts/jQuery/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                ));

            bundles.Add(new StyleBundle("~/content/bootstrap/bootstrap-css").Include(
                "~/Content/Bootstrap/bootstrap.css",
                "~/Content/Bootstrap/body.css",
                "~/Content/Bootstrap/bootstrap-responsive.css",
                "~/Content/Bootstrap/bootstrap-mvc-validation.css"
                ));

            //jQueryFileUpload bundles

            bundles.Add(new ScriptBundle("~/scripts/jqueryfileupload/fileUpload-js").Include(
                "~/Scripts/jQueryFileUpload/jquery.iframe-transport.js",
                "~/Scripts/jQueryFileUpload/jquery.fileupload.js",
                "~/Scripts/jQueryFileUpload/jquery.fileupload-process.js",
                "~/Scripts/jQueryFileUpload/jquery.fileupload-resize.js",
                "~/Scripts/jQueryFileUpload/jquery.fileupload-validate.js",
                "~/Scripts/jQueryFileUpload/jquery.fileupload-ui.js"
                ));

            bundles.Add(new StyleBundle("~/content/jQueryFileUpload/fileUpload-css").Include(
                "~/Content/jQueryFileUpload/jquery.fileupload-ui.css",
                "~/Content/jQueryFileUpload/style.css"
                ));

            //Fine uploader bundles.

            bundles.Add(new StyleBundle("~/content/fineuploader/fineuploader-css").Include(
                        "~/Content/FineUploader/fineuploader-{version}.css"));

            bundles.Add(new ScriptBundle("~/scripts/fineuploader/fineuploader-js").Include(
                        "~/Scripts/FineUploader/jquery.fineuploader-{version}.js"));
        }
    }
}
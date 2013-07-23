using System.Web;
using System.Web.Optimization;

namespace EPSCoR
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/scripts/modernizr/modernizr").Include(
                        "~/Scripts/Modernizr/modernizr-*"));

            #region Custom bundles

            bundles.Add(new StyleBundle("~/content/css").Include(
                        "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/scripts/EPSCoR.js").Include(
                        "~/Scripts/jQueryExt.js",
                        "~/Scripts/EPSCoR-{version}.js"));

            #endregion Custom bundles

            #region jQuery bundles

            bundles.Add(new ScriptBundle("~/scripts/jquery/jquery").Include(
                        "~/Scripts/jQuery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery/jqueryval").Include(
                        "~/Scripts/jQuery/jquery.validate.js",
                        "~/scripts/jQuery/jquery.validate.unobtrusive.js"
                        ));

            bundles.Add(new ScriptBundle("~/scripts/jquery/jquery-migrate").Include(
                        "~/Scripts/jQuery/jquery-migrate-{version}.js"
                        ));

            #endregion jQuery bundles

            #region jQuery ui bundles

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

            #endregion jQuery ui bundles

            #region Bootstrap bundles

            bundles.Add(new ScriptBundle("~/scripts/bootstrap/bootstrap-js").Include(
                "~/Scripts/Bootstrap/bootstrap.js",
                "~/Scripts/jQuery/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                ));

            bundles.Add(new StyleBundle("~/content/bootstrap/bootstrap-css").Include(
                "~/Content/Bootstrap/bootstrap.css",
                "~/Content/Bootstrap/body.css",
                "~/Content/Bootstrap/bootstrap-responsive.css",
                "~/Content/Bootstrap/bootstrap-mvc-validation.css"
                ));

            #endregion Bootstrap bundles

            #region jQueryFileUpload bundles

            bundles.Add(new ScriptBundle("~/scripts/fileupload/fileUpload.js").Include(
                "~/Scripts/FileUpload/jquery.iframe-transport.js",
                "~/Scripts/FileUpload/jquery.fileupload.js",
                "~/Scripts/FileUpload/jquery.fileupload-process.js",
                "~/Scripts/FileUpload/jquery.fileupload-validate.js",
                "~/Scripts/FileUpload/jquery.fileupload-ui.js",
                "~/Scripts/jQueryFileUploadSetup.js"
                ));

            bundles.Add(new StyleBundle("~/content/fileupload/fileUpload.css").Include(
                "~/Content/FileUpload/css/jquery.fileupload-ui.css"
                ));

            #endregion jQueryFileUpload bundles

            #region jQuery Datatables bundles
            
            bundles.Add(new ScriptBundle("~/scripts/DataTables/datatables.js").Include(
                "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js"
                ));

            bundles.Add(new StyleBundle("~/content/DataTables/datatables.css").Include(
                "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css"
                ));

            #endregion jQuery Datatables bundles

            #region JavaScript Templates bundles

            bundles.Add(new ScriptBundle("~/scripts/JavaScriptTemplates/javascripttemplates.js").Include(
                "~/Scripts/JavaScriptTemplates/tmpl.js"));

            #endregion JavaScript Templates bundles

            #region SignalR bundles

            bundles.Add(new ScriptBundle("~/scripts/SignalR/signalr.js").Include(
                "~/Scripts/SignalR/jquery.signalR-{version}.js"));

            #endregion SignalR bundles
        }
    }
}
using System.Web;
using System.Web.Optimization;

namespace MeetupMeetingManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            var paths = new[]
            {
                "~/Scripts/application/app.js",
                "~/Scripts/application/*Service.js",
                "~/Scripts/application/MemberListCtrl.js"
            };
#if DEBUG
            bundles.Add(new ScriptBundle("~/bundles/app").Include(paths));
#else
            bundles.Add(new Bundle("~/bundles/app").Include(paths));
#endif
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

        }
    }
}

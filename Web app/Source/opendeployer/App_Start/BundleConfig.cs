using System.Web;
using System.Web.Optimization;

namespace opendeployer
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-2.2.3.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/opendeployer.css"));
        }
    }
}

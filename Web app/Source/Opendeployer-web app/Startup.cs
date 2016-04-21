using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Opendeployer_web_app.Startup))]
namespace Opendeployer_web_app
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

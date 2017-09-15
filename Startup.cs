using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bwarrickblog.Startup))]
namespace bwarrickblog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

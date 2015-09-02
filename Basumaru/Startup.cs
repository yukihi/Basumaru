using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Basumaru.Startup))]
namespace Basumaru
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

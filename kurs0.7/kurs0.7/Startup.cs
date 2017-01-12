using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(kurs0._7.Startup))]
namespace kurs0._7
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

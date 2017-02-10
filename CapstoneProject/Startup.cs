using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CapstoneProject.Startup))]
namespace CapstoneProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

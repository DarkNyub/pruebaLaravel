using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Nodo.Startup))]
namespace Nodo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

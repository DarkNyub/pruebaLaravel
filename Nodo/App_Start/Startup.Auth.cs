using System;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.Google;
using Owin;

namespace Nodo
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.MapSignalR();
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}

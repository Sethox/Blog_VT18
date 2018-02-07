using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blog_VT18.Startup))]
namespace Blog_VT18
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

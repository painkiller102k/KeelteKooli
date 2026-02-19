using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KeelteKooli.Startup))]
namespace KeelteKooli
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

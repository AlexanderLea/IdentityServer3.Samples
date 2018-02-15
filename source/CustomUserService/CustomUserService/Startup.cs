using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using Owin;
using SampleApp.Config;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Serilog;
using IdentityServer3.Host.Config;
using SampleApp.Middleware;

namespace SampleApp
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            app.Map("/core",
                coreApp =>
                {
                    coreApp.Use<IdentityServerWithTenantSwitchingMiddleware>(coreApp.Properties);
                   // coreApp.UseIdentityServer(new IdentityServerWithTenantSwitchingMiddleware(null, null).GetOptions());
                });
        }
    }
}
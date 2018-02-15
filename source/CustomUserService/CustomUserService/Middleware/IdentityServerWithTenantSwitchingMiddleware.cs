
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Host.Config;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using Owin;
using SampleApp.Config;

namespace SampleApp.Middleware
{
    /// <summary>
    /// Dynamic middleware which rebuilds the Owin pipeline and Identity Server configuration on
    /// a per-request basis such that tenant-specific configuration can be applied.
    /// </summary>
    public class IdentityServerWithTenantSwitchingMiddleware : OwinMiddleware
    {
        private static IDictionary<string, object> _properties;

        public IdentityServerWithTenantSwitchingMiddleware(OwinMiddleware next, IDictionary<string, object> properties)
            : base(next)
        {
            _properties = properties;
        }

        public override Task Invoke(IOwinContext context)
        {
            var app = new AppBuilder();

            //Do some magic based on current request

            var options = GetOptions();

            _properties.ForEach(kvp =>
            {
                if (!app.Properties.ContainsKey(kvp.Key))
                {
                    app.Properties.Add(kvp.Key, kvp.Value);
                }
            });

            app.UseIdentityServer(options);

            return app.Build()(context.Environment);
        }

        public IdentityServerOptions GetOptions()
        {
            var factory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            var userService = new EulaAtLoginUserService();

            // note: for the sample this registration is a singletone (not what you want in production probably)
            factory.UserService = new Registration<IUserService>(resolver => userService);

            var options = new IdentityServerOptions
            {
                SiteName = "IdentityServer3 - CustomUserService",

                SigningCertificate = Certificate.Get(),
                Factory = factory,

                AuthenticationOptions = new AuthenticationOptions
                {
                    LoginPageLinks = new LoginPageLink[] {
                            new LoginPageLink{
                                Text = "Register",
                                Href = "localregistration"
                            }
                        }
                },

                EventsOptions = new EventsOptions
                {
                    RaiseSuccessEvents = true,
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true
                }
            };

            return options;
        }
    }


}
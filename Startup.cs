using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using Microsoft.Owin.Extensions;
using System.Security.Claims;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web;
using Microsoft.Owin.Host.SystemWeb;
using System.Configuration;

[assembly: OwinStartup(typeof(EPiServer.Startup))]

namespace EPiServer
{
    public class Startup
    {
        const string LogoutUrl = "/util/logout.aspx";

        private static string realm = ConfigurationManager.AppSettings["ida:Wtrealm"];

        private static string adfsMetadata = ConfigurationManager.AppSettings["ida:ADFSMetadata"];

        public void Configuration(IAppBuilder app)
        {
            //Enable cookie authentication, used to store the claims between requests
            app.SetDefaultSignInAsAuthenticationType(WsFederationAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {                
                AuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType,
                //CookieManager = new SystemWebCookieManager()
            });

            //Enable federated authentication
            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions()
            {                
                //Trusted URL to federation server meta data
                MetadataAddress = adfsMetadata,
                //Value of Wtreal must *exactly* match what is configured in the federation server
                Wtrealm = realm,
                //CallbackPath = new PathString("/SSO/LoginCallBack.aspx"),
                Notifications = new WsFederationAuthenticationNotifications()
                {                                        
                    RedirectToIdentityProvider = (ctx) =>
                    {
                        //To avoid a redirect loop to the federation server send 403 when user is authenticated but does not have access
                        if (ctx.OwinContext.Response.StatusCode == 401 && ctx.OwinContext.Authentication.User.Identity.IsAuthenticated)
                        {
                            ctx.OwinContext.Response.StatusCode = 403;
                            ctx.HandleResponse();
                        }
                        return Task.FromResult(0);
                    },                    
                    SecurityTokenValidated = (ctx) =>
                    {
                        //Ignore scheme/host name in redirect Uri to make sure a redirect to HTTPS does not redirect back to HTTP
                        var redirectUri = new Uri(ctx.AuthenticationTicket.Properties.RedirectUri, UriKind.RelativeOrAbsolute);                        

                        var userName = ctx.AuthenticationTicket.Identity.Claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                        var roles = ctx.AuthenticationTicket.Identity.Claims.Where(p => p.Type == "https://adfs.bisnode.com/customclaims/groups").Select(p => p.Value);

                        if (redirectUri.IsAbsoluteUri)
                        {
                            ctx.AuthenticationTicket.Properties.RedirectUri = redirectUri.PathAndQuery;//"https://bisnodeintranet.local/SSO/LoginCallBack.aspx?userName=" + userName + "&roles=" + string.Join(",", roles) + "&ReturnUrl=" + redirectUri.PathAndQuery;// redirectUri.PathAndQuery;
                        }

                        AuthenticateUser(userName, roles);                        
                        return Task.FromResult(0);
                    }
                }
            });

            //Add stage marker to make sure WsFederation runs on Authenticate (before URL Authorization and virtual roles)
            app.UseStageMarker(PipelineStage.Authenticate);

            //Remap logout to a federated logout
            app.Map(LogoutUrl, map =>
            {
                map.Run(ctx =>
                {
                    ctx.Authentication.SignOut();
                    return Task.FromResult(0);
                });
            });

            //Tell antiforgery to use the name claim
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }


        private void AuthenticateUser(string userName, IEnumerable<string> roles)
        {
            var user = Membership.GetUser(userName);
            if (user == null)
            {
                user = Membership.CreateUser(userName, "randomPassword", userName);
            }

            // TODO: The code block for role creating for the user is for POC only, we should refactor it when do a implementation.
            roles.ToList().ForEach(role =>
            {
                if (!Roles.RoleExists(role))
                {
                    Roles.CreateRole(role);
                }

                if (!Roles.IsUserInRole(user.UserName, role))
                {
                    Roles.AddUserToRole(user.UserName, role);
                }
            });

            // Add cookie for authenticated user.
            DateTime now = DateTime.Now;
            var formsAuthenticationTicket = new FormsAuthenticationTicket(1, userName, now, now.AddYears(1), true, string.Join(",", roles), FormsAuthentication.FormsCookiePath);
            var cookieName = FormsAuthentication.FormsCookieName;
            FormsAuthentication.Initialize();
            var encryptedTicket = FormsAuthentication.Encrypt(formsAuthenticationTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                Expires = DateTime.Now.AddMinutes(5256000),
                HttpOnly = true
            };

            var owinContext = HttpContext.Current.GetOwinContext();
            var owinResponse = owinContext.Response;

            owinResponse.OnSendingHeaders(state =>
            {
                owinResponse.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions() { Expires = cookie.Expires, HttpOnly = cookie.HttpOnly });
            }, null);
        }
    }
}

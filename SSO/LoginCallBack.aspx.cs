using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Security;
using System.IO;

namespace EPiServer.SSO
{
    public partial class LoginCallBack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get access token
            var code = HttpContext.Current.Request.QueryString["code"];
            var accessToken = GetFromUrl("http://10.20.2.69:8686/fakesso/GetAcessToken?code=" + code);

            // Get user infomations.
            var userInformation = GetFromUrl("http://10.20.2.69:8686/fakesso/GetUserInfomation?accessToken=" + accessToken);
            var datas = userInformation.Split(new char[] { ',' });
            var userName = datas[0];
            var roles = new string[2] { datas[1], datas[2] };

            // Check user and roles.
            var user = Membership.GetUser(userName);
            if (user == null)
            {
                user = Membership.CreateUser(userName, "randomPassword", userName + "@investigate.local");
                Roles.AddUserToRoles(user.UserName, roles);
            }

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

            Response.Cookies.Add(cookie);

            var returnUrl = HttpContext.Current.Request.QueryString["ReturnUrl"];
            Response.Redirect(returnUrl);
        }

        private string GetFromUrl(string url)
        {            
            var client = new WebClient();

            // Add a user agent header in case the  
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string result = reader.ReadToEnd();
            return result;
        }
    }
}
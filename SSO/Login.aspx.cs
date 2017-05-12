using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EPiServer
{
    public partial class LoginHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var returnUrl = HttpContext.Current.Request.QueryString["ReturnUrl"];

            var loginUrl = "http://10.20.2.69:8686/fakesso/login?returnUrl=" + "http://win-ccsid4dubpn:17001/SSO/LoginCallBack.aspx?ReturnUrl=" + returnUrl;
            Response.Redirect(loginUrl);
        }
    }
}
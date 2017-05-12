using System;
using System.Collections.Concurrent;
using System.Web.Mvc;

namespace FakeSSO.App.Controllers
{
    public class FakeSSOController : Controller
    {
        static ConcurrentDictionary<string, string> dicAuthorizationCodes = new ConcurrentDictionary<string, string>();

        static ConcurrentDictionary<string, string> dicAccessTokens = new ConcurrentDictionary<string, string>();

        const string _ROLES = "WebAdmins,WebEditors";

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            // Do the verify username and password here.

            // Generate authorization code here.
            var code = Guid.NewGuid().ToString();
            dicAuthorizationCodes.TryAdd(code, username);
            
            returnUrl = $"{returnUrl}&code={code}";

            return Redirect(returnUrl);
        }

        public string GetAcessToken(string code)
        {
            var userName = dicAuthorizationCodes[code];

            var accessToken = Guid.NewGuid().ToString();
            dicAccessTokens.TryAdd(accessToken, userName);

            return accessToken;
        }

        public string GetUserInfomation(string accessToken)
        {
            var userName = dicAccessTokens[accessToken];
            return $"{userName},{_ROLES}";
        }
    }
}
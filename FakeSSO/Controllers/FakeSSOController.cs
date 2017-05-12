using System;
using System.Collections.Concurrent;
using System.Web.Mvc;

namespace FakeSSO.App.Controllers
{
    public class FakeSSOController : Controller
    {
        static ConcurrentDictionary<string, string> dicAccessKey = new ConcurrentDictionary<string, string>();

        static ConcurrentDictionary<string, string> dicUserInfomations = new ConcurrentDictionary<string, string>();

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
            var guid = Guid.NewGuid().ToString();
            dicAccessKey.TryAdd(guid, _ROLES);

            returnUrl = $"{returnUrl}?code={guid}";

            return Redirect(returnUrl);
        }

        public string GetAcessToken(string code)
        {
            var roles = dicAccessKey[code];

            return roles;
        }
    }
}
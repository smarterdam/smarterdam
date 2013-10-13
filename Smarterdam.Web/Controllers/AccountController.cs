using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Smarterdam.Api.Helpers;

namespace Smarterdam.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public void Login(string username, string password)
        {
            if (username == "admin" && password == PasswordHelper.WebPassword)
            {
                Session["User"] = username;

                FormsAuthentication.SetAuthCookie(username, true);

                string returnUrl = Request.QueryString["ReturnUrl"] as string;

                if (returnUrl != null)
                {
                    Response.Redirect(returnUrl);
                }
                else
                {
                    //no return URL specified so lets kick him to home page
                    Response.Redirect("~/Home/Index");
                }
            }
            else
            {
                Response.Redirect("~/Account/Login");
            }
        }
    }
}

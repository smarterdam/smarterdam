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
	        var login = Request.Cookies["login"];
			var password = Request.Cookies["password"];
	        if (login != null && password != null)
	        {
		        Check(login.Value, password.Value);
	        }

            return View();
        }

        [HttpPost]
        public void Login(string username, string password)
        {
	        Check(username, password);
        }

	    private void Enter(string username, string password)
	    {
			Session["User"] = username;
			
			Response.SetCookie(new HttpCookie("login")
			{
				Value = username,
				Expires = DateTime.Now.AddDays(7),
			});

			Response.SetCookie(new HttpCookie("password")
			{
				Value = password,
				Expires = DateTime.Now.AddDays(7),
			});

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

	    private void Check(string login, string password)
	    {
		    if (login == "admin" && password == PasswordHelper.WebPassword)
		    {
			    Enter(login, password);
		    }
			else
			{
				Response.Redirect("~/Account/Login");
			}
	    }
    }
}

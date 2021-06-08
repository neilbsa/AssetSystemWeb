using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AssetSystemWeb.Controllers
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
   public class AuthorizeRequireBranch : AuthorizeAttribute
    {

        public bool IsAuthorize { get; set; }
        public string  ViewName { get; set; }

        public AuthorizeRequireBranch()
        {
            ViewName = "SystemMessage";
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


            IsAuthorize = base.AuthorizeCore(httpContext);
            return IsAuthorize;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if(filterContext == null)
            {
                return;
            }


            if(!IsAuthorize && filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                ViewDataDictionary MyViewData = new ViewDataDictionary();
                MyViewData.Add("SystemMessage", "You dont have access to this Module");
                var result = new ViewResult() { ViewName = this.ViewName, ViewData = MyViewData };
                filterContext.Result = result;
            }
            else
            {


                var session = filterContext.HttpContext.Session;
                if (session["COMPANY"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "controller", "Account" },
                            { "action", "SelectCompany" },
                            { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                        });
                }
            }


        }
    }





}
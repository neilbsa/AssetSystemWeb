using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemEntities.Models;
using SystemProcedure;

namespace AssetSystemWeb.Controllers
{
    [AuthorizeRequireBranch]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        //public ActionResult About()
        //{
        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}
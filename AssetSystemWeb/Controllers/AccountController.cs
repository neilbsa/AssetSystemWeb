using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using System.Web.Security;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using SystemProcedure;
using SystemEntities.Models;
using SystemEntities.GeneralModels;

namespace AssetSystemWeb.Controllers
{
   [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {


        }


        private List<Company> AssignedCompanies()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            List<Company> AllowedCompanies = new List<Company>();
            if(user != null)
            {
                AllowedCompanies = db.UserCompanies.Where(x => x.UserId == user.Id).Select(x => x.Company).ToList();

            }
            return AllowedCompanies;
        }


        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

     
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        public ActionResult SelectCompany()
        {

            var user = UserManager.FindByName(User.Identity.Name);
            List<Company> companies = new List<Company>();
            companies = AssignedCompanies();
            ViewBag.Company = new SelectList(companies, "Id", "Name");
            return View();
        }



        [HttpPost]
        public ActionResult SelectCompany(int Company,string returnUrl)
        {
            var SelectedCompany = db.Companies.Where(x => x.Id == Company).FirstOrDefault();
     
            Session["COMPANY"] = SelectedCompany;
            if (Url.IsLocalUrl(returnUrl))
            {
             return  Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AuthorizeRequireBranch(Roles = "Administrator")]
        public ActionResult Index()
        {
            List<ApplicationUser> modUsers = new List<ApplicationUser>();
            modUsers = UserManager.Users.ToList();
            return View(modUsers);
        }


        [Authorize(Roles = "Administrator")]
        public PartialViewResult RegisterUser()
        {
            List<string> defaultRole = new List<string>();
            defaultRole.Add("Common");
            RegisterViewModel model = new RegisterViewModel();
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var companies = db.Companies.ToList();

            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            ViewBag.AssignedCompanies = new SelectList(new List<Company>(),"Id","Name");
            //model.Companies = 
            //model.AssignedCompanies = new List<Company>();
            model.SystemRoles = roleManager.Roles.Where(x => x.Name != "Common").Select(x => x.Name).ToList();
            model.IncludedRoles = defaultRole;
            return PartialView("AccessRegistrationView", model);
        }

        [Authorize(Roles = "Administrator")]
        private List<Company> GetAssignedCompany(string UserId)
        {
            var user = UserManager.FindById(UserId);
            IEnumerable<Company> Companies = db.UserCompanies.Where(x => x.UserId == user.Id).Select(x => x.Company).ToList();
            return Companies.ToList();
            //return Companies;
        }


        [Authorize(Roles = "Administrator")]
        public PartialViewResult UpdateUserAccess(string UserId)
        {
         
            RegisterViewModel model = new RegisterViewModel();
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userDetail =  UserManager.Users.Where(x => x.Id == UserId).FirstOrDefault();
            model.Firstname = userDetail.Firstname;
            model.Middlename = userDetail.Middlename;
            model.Surname = userDetail.Surname;
            model.UserName = userDetail.UserName;
            model.Email = userDetail.Email;
            model.UserId = UserId;
            model.IsLocked = userDetail.LockoutEnabled;
            model.IncludedRoles = UserManager.GetRoles(UserId).ToList();
            model.IsChangePassword = false;
            model.SystemRoles = roleManager.Roles.Where(p => !model.IncludedRoles.Contains(p.Name)).Select(x=> x.Name).ToList();
            ViewBag.AssignedCompanies = new SelectList(GetAssignedCompany(UserId), "Id", "Name");
            var companies = db.Companies.Where(x => !db.UserCompanies.Where(c=> c.UserId == UserId).Select(v=> v.Company).Contains(x)).ToList();
            ViewBag.Companies = new SelectList(companies, "Id", "Name");

            return PartialView("AccessUpdateView", model);
        }


        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult UpdateUserAccess(RegisterViewModel model)
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var user = UserManager.FindById(model.UserId);
            if (user!=null)
            {


                user.Firstname = model.Firstname;
                user.Middlename = model.Middlename;
                user.Surname = model.Surname;
                user.Email = model.Email;
                user.LockoutEnabled = model.IsLocked;
                user.LockoutEndDateUtc = model.IsLocked ? new DateTime(9999, 12, 30) : DateTime.Now;

                if (model.IsChangePassword)
                {
                    UserManager.RemovePassword(model.UserId);
                    UserManager.AddPassword(model.UserId, model.Password);
                }

                if (model.AssignedCompanies == null || model.IncludedRoles == null)
                {
                    UserManager.SetLockoutEnabled(user.Id, true);
                    UserManager.SetLockoutEndDate(user.Id, new DateTime(9999, 12, 30));
                }


                if (model.IncludedRoles == null)
                {
                    UserManager.AddToRole(model.UserId, "Common");
                }
                else
                {
                    foreach (var item in model.IncludedRoles)
                    {

                        if (roleManager.RoleExists(item))
                        {
                            UserManager.AddToRole(model.UserId, item);
                        }
                    }
                }


                if (model.SystemRoles != null)
                {
                    foreach (var item in model.SystemRoles)
                    {
                        if (roleManager.RoleExists(item))
                        {
                            UserManager.RemoveFromRole(model.UserId, item);
                        }
                    }
                }
                UserManager.Update(user);



                if (model.Companies != null)
                {
                    foreach (var item in model.Companies)
                    {
                        var AssComp = db.UserCompanies.Where(x => x.UserId == user.Id && x.CompanyId == item).FirstOrDefault();
                        if (AssComp != null)
                        {
                            db.UserCompanies.Remove(AssComp);
                        }
                    }
                }






                if (model.AssignedCompanies != null)
                {
                    foreach (var item in model.AssignedCompanies)
                    {
                        var company = db.UserCompanies.Where(x=> x.UserId == model.UserId && x.CompanyId == item).FirstOrDefault();
                        if (company == null)
                        {
                            db.UserCompanies.Add(new UserCompany() { IsDeleted = false, CompanyId = item, UserId = user.Id });
                        }
                    }
                }





            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RegisterUser(RegisterViewModel model)
        {
         
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
      


           


            ApplicationUser user = new ApplicationUser()
            {
                //LockoutEnabled = lockout,
                //LockoutEndDateUtc = lockdate,
                Email = model.Email,
                Firstname = model.Firstname,
                Middlename = model.Middlename,
                Surname = model.Surname,
                UserName = model.UserName

            };
            var result = await UserManager.CreateAsync(user, model.Password);


            if (model.AssignedCompanies == null || model.IncludedRoles == null)
            {
                UserManager.SetLockoutEnabled(user.Id, true);
                UserManager.SetLockoutEndDate(user.Id, new DateTime(9999, 12, 30));
            }




            if (result.Succeeded)
            {
                if(model.IncludedRoles == null)
                {
                    UserManager.AddToRole(user.Id, "Common");
                }
                else
                {
                    foreach (var item in model.IncludedRoles)
                    {

                        if (roleManager.RoleExists(item))
                        {
                            UserManager.AddToRole(user.Id, item);
                        }
                    }
                }

             
                if(model.SystemRoles != null)
                {
                    foreach (var item in model.SystemRoles)
                    {
                        if (roleManager.RoleExists(item))
                        {
                            UserManager.RemoveFromRole(user.Id, item);
                        }
                    }
                }






                if (model.Companies != null)
                {
                    foreach (var item in model.Companies)
                    {
                        var AssComp = db.UserCompanies.Where(x => x.UserId == user.Id && x.CompanyId == item).FirstOrDefault();
                        if(AssComp != null)
                        {
                            db.UserCompanies.Remove(AssComp);
                        }
                    }
                }






                if (model.AssignedCompanies != null)
                {
                    foreach (var item in model.AssignedCompanies)
                    {
                        var company = db.Companies.Find(item);
                        if (company != null)
                        {
                            db.UserCompanies.Add(new UserCompany() { IsDeleted = false, CompanyId = company.Id, UserId = user.Id });
                        }
                    }
                }




            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }





        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }



            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {


                case SignInStatus.Success:
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                  
                    Session["COMPANY"] = null;
                  

              

                    
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                    ViewBag.LoginError = "Username or Password is incorrect";
                    return View("Login");
                case SignInStatus.LockedOut:
                    ViewBag.LoginError = "User is Locked out";
                    return View("Login");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
        }


    







        [HttpPost]
        [Authorize]
        public ActionResult CheckUsername(string username)
        {
            bool _isExist = false;

            if (!String.IsNullOrEmpty(username) || !String.IsNullOrWhiteSpace(username))
            {
                _isExist = UserManager.Users.Any(x => x.UserName == username);
            }

            return Json( new { IsExist = _isExist });
        }





        [HttpPost]
        [Authorize]
        public ActionResult CheckEmailAddress(string Email)
        {

  
            bool isExist;
            if (!String.IsNullOrEmpty(Email) || !String.IsNullOrWhiteSpace(Email))
            {
                isExist = UserManager.Users.Any(x => x.Email == Email);

                if (!IsValid(Email))
                {
                    return Json(new { ValidationError = "The Email field is a not valid e-mail address" });
                }
                else
                {
                        if (isExist)
                    {
                        return Json(new { ValidationError = "Email is already taken" });
                    }
                }
            }
             return Json(new { ValidationError = "" });
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
           
        }
        

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }








        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }










        //internal class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        #endregion
    }
}
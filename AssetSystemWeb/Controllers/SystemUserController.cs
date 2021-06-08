using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using SystemEntities.ViewModels;
using SystemProcedure;

namespace AssetSystemWeb.Controllers
{



 
    [AuthorizeRequireBranch]
    public class SystemUserController : TransactionBaseControllerMethods<UserProfile>
    {



        //public ActionResult ConsigmentsIndex()
        //{
        //    IEnumerable<UserProfile> list = GetListData(x=> x.CompanyId == SelectedCompany.Id);
        //    return View("ConsignmentIndex",list);
        //}

        public PartialViewResult UserLookup()
        {
            var userList = GetListData<UserProfile>(x => x.CompanyId == SelectedCompany.Id && x.IsDeleted == false);
            return PartialView(userList);
        }


        [AuthorizeRequireBranch(Roles="Administrator")]
        public PartialViewResult Register()
        {
            //var companies = GetListData<Company>();
            var departments = GetListData<Department>();
            var branches = GetListData<Branch>();
            //ViewBag.Companies = new SelectList(companies, "Id","Name");
            ViewBag.CompanyId = SelectedCompany.Id;
            ViewBag.Departments = new SelectList(departments.Where(x=>x.CompanyId == SelectedCompany.Id), "Id", "Description");
            ViewBag.Branches = new SelectList(branches.Where(x => x.CompanyId == SelectedCompany.Id), "Id", "Name");
            return PartialView("UserRegistrationView");
        }

        [AuthorizeRequireBranch(Roles = "Administrator")]
        public PartialViewResult ForUpdateData(int Id)
        {
            //var companies = GetListData<Company>();
            var departments = GetListData<Department>();
            var branches = GetListData<Branch>();
            //ViewBag.Companies = new SelectList(companies, "Id", "Name");
            ViewBag.CompanyId = SelectedCompany.Id;
            ViewBag.Departments = new SelectList(departments.Where(x=>x.CompanyId ==SelectedCompany.Id), "Id", "Description");
            ViewBag.Branches = new SelectList(branches.Where(x=>x.CompanyId == SelectedCompany.Id), "Id", "Name");
            UserProfile user = GetDataInfo(x => x.Id == Id);
            return PartialView("UpdateDetailsView", user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRequireBranch(Roles = "Administrator")]
        public ActionResult ForUpdateData(UserProfile User)
        {
            //var companies = GetListData<Company>();
            //var departments = GetListData<LookUpList>(x => x.GroupOf == "DepartmentList");
            //ViewBag.Companies = new SelectList(companies, "Id", "Name");
            //ViewBag.Departments = new SelectList(departments, "Description", "Name");
            UserProfile user = GetDataInfo(x => x.Id == User.Id);
            user.Firstname = User.Firstname;
            user.Middlename = User.Middlename;
            user.Surname = User.Surname;
            user.DepartmentId = User.DepartmentId;
            user.CompanyId = SelectedCompany.Id;
            user.BranchId = User.BranchId;
            UpdateData<UserProfile>(m => m.Id, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRequireBranch(Roles = "Administrator")]
        public ActionResult Register(UserProfile entity)
        {
            entity.UserType = "Users";
            entity.CompanyId = SelectedCompany.Id;
            return CreateEntity(entity, true, "Index");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        public override ActionResult Index()
        {
            List<UserProfile> users = GetListData().Where(x => x.CompanyId == SelectedCompany.Id).ToList();
            return View(users);
        }




        [HttpPost]
        [AuthorizeRequireBranch(Roles = "Administrator")]
        public ActionResult CheckIdNum(string Id)
        {
            bool _isExist = false;
            if (!String.IsNullOrEmpty(Id) || !String.IsNullOrWhiteSpace(Id))
            {
                _isExist = IsExist<UserProfile>(x => x.CompEmployeeNum == Id);
            }
            return Json(new { IsExist = _isExist });
        }

    }
}
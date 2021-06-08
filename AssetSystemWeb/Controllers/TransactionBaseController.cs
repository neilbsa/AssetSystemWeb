using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SystemEntities;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using SystemEntities.ViewModels;
using SystemProcedure;
using SystemProcedure.Asset;

namespace AssetSystemWeb.Controllers
{
    [Authorize]
    public abstract class TransactionBaseControllerMethods<TEntity> : Controller where TEntity : class, IBaseEntity
    {

        public Company SelectedCompany { get; set; }



        public TransactionBaseControllerMethods()
        {
            SelectedCompany = new Company();
            context = new DbConnect<TEntity>();
            SelectedCompany = System.Web.HttpContext.Current.Session["COMPANY"] as Company;
        }
        public DbConnect<TEntity> context = new DbConnect<TEntity>();

        public virtual bool CanExecuteAdd() { return true; }
        public virtual bool CanExecuteGetList() { return true; }
        public virtual bool CanExecuteDelete() { return true; }
        public virtual bool CanExecuteUpdate() { return true; }
        public virtual void UpdateExtraProcess(TEntity newEntity, TEntity oldEntity) { }
        public virtual void CreateExtraProcessing(TEntity entity) { }
        public virtual void DetailsExtraProcessing(TEntity entity) { }
        private ApplicationUserManager _userManager;

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

        public List<Company> GetAssignedCompany()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            IEnumerable<Company> Companies = GetListData<UserCompany>(x => x.UserId == user.Id).Select(x => x.Company).ToList();
            return Companies.ToList();
            //return Companies;
        }




        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult SystemMessage(string Message)
        {
            if (String.IsNullOrEmpty(Message) || String.IsNullOrWhiteSpace(Message))
            {
                Message = "Null message";
            }
            ViewData["SystemMessage"] = Message;
            return View();
        }


        public virtual ActionResult Details(int Id)
        {
            TEntity ent = GetDataInfo<TEntity>(x => x.Id == Id);
            DetailsExtraProcessing(ent);
            return ViewDetails(ent, HasDetailAccess, "Details");
        }

        public virtual ActionResult Update(int Id)
        {
            TEntity ent = GetDataInfo<TEntity>(x => x.Id== Id);
            return ViewDetails(ent, HasDetailAccess, "Update");
        }

        [HttpPost]
        public virtual ActionResult Update(TEntity ente)
        {
            TEntity ent = GetDataInfo<TEntity>(x => x.Id == ente.Id);
            UpdateExtraProcess(ente,ent);
            UpdateData<TEntity>(x => x.Id, ent);
            return ViewDetails(ente, HasDetailAccess, "Update");
        }

        public virtual bool HasDetailAccess(TEntity T)
        {
            return true;
        }

        public virtual bool HasUpdateAccess(TEntity T)
        {
            return true;
        }

        public ActionResult ViewDetails(TEntity ent, Func<TEntity, bool> HasAccess, string viewName)
        {
            TEntity e = ent;
            if (e == null)
            {
                return RedirectToAction("SystemMessage", new { Message = "Your data is not existing" });
            }
            else
            {
                if (HasAccess(e))
                {
                    return View(viewName, e);
                }
                else
                {
                    return RedirectToAction("SystemMessage", new { Message = "You Don't have access to this Data" });
                }
            }

        }

        public ActionResult CreateReport1(int id, string action)
        {
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ActionAsPdf(action, new { Id = id })
            {
                Cookies = cookieCollection,
                PageSize = Size.Letter,
                CustomSwitches = footer

            };

        }

        public ActionResult CreateReport(int id, string action)
        {

            TEntity ent = GetDataInfo(x => x.Id == id);
            if (HasDetailAccess(ent))
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";




                return new ViewAsPdf(action, ent)
                {

                    Cookies = cookieCollection,
                    PageSize = Size.Letter,
                    CustomSwitches = footer

                };
            }
            else
            {
                return RedirectToAction("SystemMessage", new { Message = "You don't Have access to this report" });
            }
        }


        public virtual ActionResult CreateEntity(TEntity ent, bool CanAdd, string ActionName = null, Action<TEntity> ExtraProcessAfterAdding = null)
        {

            if (CanAdd)
            {
                CreateExtraProcessing(ent);
                AddDataToContext(ent);
                if (ExtraProcessAfterAdding != null)
                {
                    ExtraProcessAfterAdding(ent);
                }
                //ExtraProcessAfterAdding(ent);
            }

            if (ActionName != null)
            {
                return RedirectToAction(ActionName, new { Id = ent.Id });
            }
            else
            {
                return RedirectToAction("Details", new { Id = ent.Id });
            }

        }



        public virtual ActionResult Index()
        {
            List<TEntity> ent = GetListData<TEntity>().ToList();
            return View(ent);
        }
        public List<lookupTentity> GetAssetUserLookup()
        {

            var userList = GetListData<UserProfile>(x => x.CompanyId == SelectedCompany.Id && x.IsDeleted == false);
            var deptList = GetListData<Department>(x => x.IsDeleted == false);
            List<lookupTentity> all = userList.Select(x => new lookupTentity { Id = x.Id, Description = x.Fullname, Description2 = x.CompEmployeeNum, GroupOf = "User" }).Union
           (deptList.Select(y => new lookupTentity { Id = y.Id, Description = y.Description, Description2 = y.DepartmentCode, GroupOf = "Department" })).ToList();
            return all;
        }






        #region GENERIC METHODS
        public static IEnumerable<TCurrent> GetListData<TCurrent>
        (Expression<Func<TCurrent, bool>> query = null)
       where TCurrent : class, IBaseEntity
        {
            DbConnect<TCurrent> data = new DbConnect<TCurrent>();
            return data.GetList(query);
        }


        public bool IsExist<TCurrent>(Expression<Func<TCurrent, bool>> T) where TCurrent : class, IBaseEntity
        {
            bool bu = true;
            DbConnect<TCurrent> context = new DbConnect<TCurrent>();
            bu = context.CheckIfExistToContext(T);
            return bu;
        }

        public TCurrent GetDataInfo<TCurrent>(Expression<Func<TCurrent, bool>> query = null) where TCurrent : class, IBaseEntity
        {
            DbConnect<TCurrent> cont = new DbConnect<TCurrent>();
            return cont.GetInfo(query);
        }



        public TCurrent AddBaseToContext<TCurrent>(TCurrent T) where TCurrent : class, IBaseEntity
        {
            DbConnect<TCurrent> context = new DbConnect<TCurrent>();
            context.AddtoContext(T);
            return T;
        }

        public void UpdateData<TCurrent>(Expression<Func<TCurrent, object>> T, TCurrent newT) where TCurrent : class, IBaseEntity
        {
            DbConnect<TCurrent> context = new DbConnect<TCurrent>();
            context.UpdatetoContext(T, newT);
        }

        #endregion






        public IEnumerable<TEntity> GetListData(Expression<Func<TEntity, bool>> query = null)
        {
            return context.GetList(query);
        }


        public TEntity GetDataInfo(Expression<Func<TEntity, bool>> query = null)
        {
            return context.GetInfo(query);
        }

        public void SaveDatabase()
        {
            context.SaveContextChanges();
        }


        public void AddToLookUp(string GroupOf, string Name, string Desc)
        {
            bool Data = IsExist<LookUpList>(x => x.GroupOf == GroupOf && x.Name == Name && x.Description == Desc);
            if (!Data)
            {
                AddBaseToContext<LookUpList>(new LookUpList() { GroupOf = GroupOf, Description = Desc, Name = Name, IsDeleted = false });
            }
            //SaveDatabase();
        }

        public void AddDataToContext(TEntity T)
        {
            if (CanExecuteAdd())
            {
                context.AddtoContext(T);
            }
        }





    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using SystemEntities.ViewModels;


namespace AssetSystemWeb.Controllers.CSD
{
    [AuthorizeRequireBranch]
    public class ConsignmentController : TransactionBaseControllerMethods<Consignment>
    {

        protected ICollection<AssetItemDetail> ForUpdateAssets { get; set; }
        public ConsignmentController()
        {

        }
        // GET: Consignment
        public override ActionResult Index()
        {
            IEnumerable<Consignment> cons = GetListData(x=> x.CompanyId == SelectedCompany.Id).OrderByDescending(x => x.Id);
            return View(cons);
        }

        public ActionResult UserConsignmentsIndex()
        {
            List<Consignment> consignments = GetListData(x => x.CompanyId == SelectedCompany.Id).ToList();
            List<Consignment> finalCons = new List<Consignment>();
            var cons = consignments.GroupBy(x => x.CurrentUser.Id);
            foreach(var con in cons)
            {
                finalCons.Add(con.First());
            }
            return View(finalCons);
        }

        public ActionResult CreateConsignments()
        {
            Consignment ConModel = new Consignment();
            List<LookUpList> list = GetListData<LookUpList>( x=> x.GroupOf == "ConsignmentType").ToList();
            ViewBag.consignmentTypes = new SelectList(list, "Description", "Name");
            return View(ConModel);
        }


        public ActionResult UploadConsignment(int Id)
        {

            Consignment cons = GetDataInfo(x => x.Id == Id);
            ConsignmentUploadModel mod = new ConsignmentUploadModel();
            if(cons != null)
            {
                mod.Consignment = cons;
                return View(mod);
            }
            return SystemMessage("Error Retrieving Consignment");
        }



        [HttpGet]
        public ActionResult UserConsignments(int UserId)
        {
            IEnumerable<Consignment> consignments = new List<Consignment>();
            consignments = GetListData(x => x.UserId == UserId && x.CompanyId == SelectedCompany.Id);
            return View(consignments);
        }



        [HttpPost]
        public PartialViewResult AddNewAssets(string assetNumber)
        {

            if (assetNumber.ToUpper().StartsWith("2CS"))
            {
                AssetHeaderDetails asset = new AssetHeaderDetails();
                if (IsExist<AssetHeaderDetails>(x => x.AssetNumber == assetNumber && x.IsDeleted == false && x.CompanyId == SelectedCompany.Id))
                {
                    asset = GetDataInfo<AssetHeaderDetails>(
                            x => x.IsDeleted == false
                         && x.AssetNumber == assetNumber);
                    return PartialView("ConsignmentAssetDetails", asset.AssetItemDetails.Where(x=>x.Status == "Available").ToList());
                }
            }
            else
            {
                if (IsExist<AssetItemDetail>(x => x.ItemId == assetNumber && x.IsDeleted == false && x.Status == "Available" && x.CompanyId == SelectedCompany.Id))
                {
                    AssetItemDetail assetitem = new AssetItemDetail();
                    assetitem = GetDataInfo<AssetItemDetail>(
                          x => x.IsDeleted == false
                       && x.ItemId == assetNumber);
                    return PartialView("ConsignmentAssetDetailsItem", assetitem);
                }                
            }


            return null;
        }

        public override bool HasDetailAccess(Consignment T)
        {
            var AssignedCompany = GetAssignedCompany();
            if (AssignedCompany.Where(x => x.Id == T.CompanyId).Any())
            {
                return true;
            }
            return false;
        }
        [HttpPost]
       public ActionResult GetAssetInformation(string assetNumber,int? userId)
        {

                if (!String.IsNullOrWhiteSpace(assetNumber) && IsExist<AssetHeaderDetails>(x => x.IsDeleted != true && x.AssetNumber == assetNumber))
                {
                    AssetHeaderDetails assetHead = GetDataInfo<AssetHeaderDetails>(x => x.IsDeleted != true && x.AssetNumber == assetNumber && x.CompanyId == SelectedCompany.Id);
                    if (assetHead != null)
                    {
                        return Json(assetHead.Description, JsonRequestBehavior.AllowGet);
                    }
                }
        


         
            return null;
        }


        public ActionResult GenerateReport(int Id)
        {
            return CreateReport(Id, "ConsignmentReport");
        }

        public ActionResult ConsignmentReport(int Id)
        {
            Consignment cons = new Consignment();
            cons = GetDataInfo(x => x.Id == Id && x.CompanyId == SelectedCompany.Id);
            return View(cons);
        }



        [HttpPost]
        public ActionResult GetUserInformation(string IdNum)
        {
            if (!String.IsNullOrWhiteSpace(IdNum))
            {
                return Json(GetDataInfo<UserProfile>(x => x.IsDeleted != true && x.CompEmployeeNum == IdNum && x.CompanyId == SelectedCompany.Id), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public override void CreateExtraProcessing(Consignment entity)
        {          
            int Count = GetListData().Select(x => x.ConsignmentNumber).Distinct().Count() + 1;
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.CompanyId = SelectedCompany.Id;
            entity.CurrentUser = null;
            entity.UserId = entity.UserId;
            entity.Status = "Open";
            //entity.LastUpdatedBy = User.Identity.Name;
            entity.ConsignmentNumber = Count.ToString("D5");
            List<ConsignmentAssetItem> assetsCons = new List<ConsignmentAssetItem>();

            
            //all posted asset for update
            foreach(var item in entity.IncludedAssetsDetails)
            {
                
                List<ConsignmentAssetItem> oldConsignRecord = new List<ConsignmentAssetItem>();
                //get all the old records of the said asset id
                oldConsignRecord = GetListData<ConsignmentAssetItem>(x => x.AssetItemId == item.Id && x.Status != "Superseded").ToList();
                foreach(var olditem in oldConsignRecord)
                {
                    olditem.Status = "Superseded";
                }

                //create new record 
                ConsignmentAssetItem ConsItem = new ConsignmentAssetItem
                {
                    AssetItemId = item.Id,
                    Status="Active"
                };
                assetsCons.Add(ConsItem);
            }
            entity.Assets = assetsCons;

            ForUpdateAssets = entity.IncludedAssetsDetails;
         
        }


        [HttpPost]
        public ActionResult AddNewConsignment(Consignment cons)
        {
            return CreateEntity(cons, true,"Details", AfterAdding);
        }

        public void AfterAdding(Consignment ent)
        {
            int i = ent.Id;
           
            foreach (var item in ForUpdateAssets)
            {             
                AssetItemDetail itemDetail = GetDataInfo<AssetItemDetail>(x => x.Id == item.Id);

                if(itemDetail != null)
                {
                    itemDetail.Status = "OnUsed";
                    UpdateData<AssetItemDetail>(x => x.Id, itemDetail);
                }




            }
       
        }
      
    }
}
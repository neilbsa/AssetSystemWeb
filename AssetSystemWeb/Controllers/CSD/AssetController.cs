using Newtonsoft.Json;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using SystemEntities;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using SystemEntities.ViewModels;
using SystemProcedure.Asset;

namespace AssetSystemWeb.Controllers.CSD
{

    [AuthorizeRequireBranch]
    public class AssetController : AssetTransactionBaseController<AssetHeaderDetails>
    {

        public override bool CanExecuteGetList()
        {
            return true;
        }

    
        public ActionResult AddDocuments()
        {
            ViewBag.DocumentTypes = GetListData<LookUpList>().Where(x => x.GroupOf == "AssetDocumentType");
            return View();
        }




        public ActionResult DownloadFile(int ent)
        {
            AssetHeaderFiles assetdetail = GetDataInfo<AssetHeaderFiles>(x => x.Id == ent);
            if (HasDetailAccess(assetdetail.AssetHeaderDetails))
            {
                return File(assetdetail.byteContent, assetdetail.contentType, assetdetail.FileName);
            }
            return null;
        }

        public ActionResult ViewDocument(int Id)
        {
            AssetHeaderFiles assetdetail = GetDataInfo<AssetHeaderFiles>(x => x.Id == Id);

            return View(assetdetail);
        }



        public ActionResult UpdateAssetDetails(int Id)
        {
                        return null;
        }
        public override ActionResult Index()
        {
            SelectedCompany = Session["Company"] as Company;
            string sub = GetSubsidiary();
            var ListOfAsset = GetListData<AssetHeaderDetails>(x => x.Subsidiary == sub && x.IsDeleted == false && x.CompanyId == SelectedCompany.Id && x.AssetType == "BundleAsset").OrderByDescending(x => x.Id).GroupBy(x => x.AssetNumber).Select(x => x.First());
            return View(ListOfAsset);
        }


        public ActionResult TransferAsset(int Id)
        {
            AssetHeaderDetails asset = GetDataInfo(x => x.Id == Id);

            return View(asset);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferAsset(AssetHeaderDetails item)
        {
            AssetHeaderDetails forTrans = new AssetHeaderDetails();
            AssetHeaderDetails newTransFer = new AssetHeaderDetails();
            string user = Request["transUser"].ToString();
            string Remarks = Request["transRemark"].ToString();
            forTrans = GetDataInfo(x => x.Id == item.Id);
   
            //forTrans.Status = "Superseded";
            //newTransFer.Status = "Transfered";
            newTransFer.AssetRemarks = String.Format("Transfer To: {0}, Transfer Remarks: {1}", user, Remarks);
            UpdateData(x => x.Id, forTrans);
            UpdateData(x => x.Id, newTransFer);

            return RedirectToAction("Details",new { Id = newTransFer.Id });
        }


        public override bool HasDetailAccess(AssetHeaderDetails T)
        {
            var AssignedCompany = GetAssignedCompany();
            if (AssignedCompany.Where(x => x.Id == T.CompanyId).Any())
            {
                if(T.CompanyId == SelectedCompany.Id)
                {
                    return true;
                }
            }
            return false;
        }


        public ActionResult UpgradeAsset(int Id)
        {
            AssetHeaderDetails assetData = new AssetHeaderDetails();
            assetData = GetDataInfo(x => x.Id == Id && x.CompanyId == SelectedCompany.Id);
            IEnumerable<LookUpList> Statuses = GetListData<LookUpList>(x => x.GroupOf == "AssetStatus");
            ViewBag.Statuses = Statuses; // new SelectList(Statuses, "Name", "Name");
            ViewBag.DocumentTypes = GetListData<LookUpList>().Where(x => x.GroupOf == "AssetDocumentType");

            return View("UpgradeAssetDetails", assetData);
        }


        public ActionResult CreateAssetReport (int id)
        {
            return CreateReport(id, "AssetReport");
        }
        
   
        public ActionResult AssetReport(int Id)
        {
            AssetHeaderDetails asset = new AssetHeaderDetails();
            asset = GetDataInfo(x => x.Id == Id && x.CompanyId == SelectedCompany.Id);
            return View(asset);
        }    
        public  ActionResult AddNewAsset()
        {
            AssetHeaderDetails newAsset = new AssetHeaderDetails();
            var Branches = GetListData<Branch>(x => x.IsDeleted == false);
            ViewBag.Branches = new SelectList(Branches, "Id", "Name");
            return View(newAsset);
        }    


        public override void CreateExtraProcessing(AssetHeaderDetails entity)
        {

           //Ccheck if supplier exisit , if exist, Get id and tag it to asset, else create new to AssetSupplier Directory
            if (IsExist<Supplier>(x =>
                 x.Name == entity.InvoiceDetails.SupplierDetails.Name
              && x.ContactNumber == entity.InvoiceDetails.SupplierDetails.ContactNumber
              && x.Address == entity.InvoiceDetails.SupplierDetails.Address
              && x.EmailAdd == entity.InvoiceDetails.SupplierDetails.EmailAdd
              && x.IsDeleted == false
             ))
            {
                entity.InvoiceDetails.SupplierId = entity.InvoiceDetails.SupplierDetails.Id;
                entity.InvoiceDetails.SupplierDetails = null;
            }
           
            entity.AssetType = "BundleAsset";
            entity.AssetNumber = GenerateAssetId();
         




            //Create A Lookuplist record for AssetItemDescription in lookuplist table used for intelisense
            AddToLookUp("AssetItemDesc", entity.Description, entity.Description);
            //Create A Lookuplist record for Vendor in lookuplist table used for intelisense
            AddToLookUp("Vendor", entity.Vendor, entity.Vendor);
            entity.PurchaseDate = entity.InvoiceDetails.InvoiceDate;
            entity.BranchId = entity.BranchId;
            entity.Subsidiary = GetSubsidiary();
            ForUpdateAssetItemDetail = entity.AssetItemDetails;
            entity.AssetItemDetails = null;

            if(entity.assetHeaderFiles != null && entity.assetHeaderFiles.Count() > 0)
            {
                foreach (var item in entity.assetHeaderFiles)
                {
                    if (item.ImageModel != null)
                    {
                        item.byteContent = item.convertFileToByte(item.ImageModel);
                        item.contentType = item.ImageModel.ContentType;
                        item.contentLenght = item.ImageModel.ContentLength;
                        item.FileName = item.ImageModel.FileName;
                    }

                }
            }
         


            base.CreateExtraProcessing(entity);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewAsset(AssetHeaderDetails entity)
        {
            return CreateEntity(entity, true, "Details", UpdateChild);
        }


        public void UpdateChild(AssetHeaderDetails enti)
        {
          
            if (ForUpdateAssetItemDetail != null )
            {
                foreach (AssetItemDetail item in ForUpdateAssetItemDetail)
                {
                    item.ItemId = GenerateItemId(item.ItemType, enti.BranchId);
                    item.Status = "Available";            
                    AddToLookUp("AssetItemDetail", item.ItemDescription, item.ItemType);
                    AddToLookUp("AssetItemPartNumber", item.PartNumber, item.ItemType);
                    item.AssetHeaderId = enti.Id;
                    item.Subsidiary = enti.Subsidiary;
                    AddBaseToContext<AssetItemDetail>(item);               
             
                }
            }
        }

        public IEnumerable<AssetItemDetail> ForUpdateAssetItemDetail { get; set; }

 
        public void GetDetails(AssetHeaderDetails entity)
        {
            GetDataInfo(x => x.Id == entity.Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAssetIsExisting(AssetHeaderDetails asset) {

            int error = 0;
            bool IsValid = true;
            if(asset.AssetItemDetails != null)
            {
                foreach (var items in asset.AssetItemDetails)
                {
                    if(items.SerialNumber == "N/A")
                    {
                        items.SerialNumber = "";
                    }
                    if (items.ItemKeyDetail == "N/A")
                    {
                        items.ItemKeyDetail = "";
                    }
                   if(IsExist<AssetItemDetail>(x => x.SerialNumber == items.SerialNumber || x.ItemKeyDetail == items.ItemKeyDetail))
                   {
                        error++;
                   }

                }
            }
         


            if(error > 0)
            {
                IsValid = false;
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAssetIsAvailableForUpgrade(AssetHeaderDetails asset)
        {

            int error = 0;
            bool IsValid = true;
            if (asset.AssetItemDetails != null)
            {
                foreach (var items in asset.AssetItemDetails.Where(x => x.Id == 0))
                {
                    if (IsExist<AssetItemDetail>(x =>x.ItemType == items.ItemType && ((x.ItemKeyDetail.ToUpper() !="N/A" && x.ItemKeyDetail == items.ItemKeyDetail ) || (x.SerialNumber.ToUpper() !="N/A"  && x.SerialNumber == items.SerialNumber))))
                    {
                        error++;
                    }
                }
            }
            if (error > 0)
            {
                IsValid = false;
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAutoCompleteLookUp(string groupOf, string keyword, string Description)
        {
            IEnumerable<LookUpList> AssetItemDesc = GetListData<LookUpList>(x => x.IsDeleted == false && x.GroupOf == groupOf && x.Name.Contains(keyword) && (Description == null || x.Description == Description));
            return Json(AssetItemDesc, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetFullSupplierDetailsList(string SupplierName)
        {
            IEnumerable<Supplier> sup = null;
            IEnumerable<Supplier> SupplierDetail = GetListData<Supplier>(x => x.IsDeleted == false && x.Name.Contains(SupplierName));
            if (SupplierDetail != null)
            {
                sup = SupplierDetail;
            }
            return Json(sup, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckDetailsIsExist(string type, string Details)
        {
            if(type != null || Details != null)
            {

                bool result = false;
                if(Details != "N/A") { 
                        switch (type)
                        {
                            case "SerialNumber":
                                result = IsExist<AssetItemDetail>(x => x.IsDeleted != true && x.SerialNumber == Details);
                                break;
                            case "ItemKeyDetail":
                                result = IsExist<AssetItemDetail>(x => x.IsDeleted != true && x.ItemKeyDetail == Details);
                                break;
                  
                            default:
                                break;
                        }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public JsonResult CheckDetailIsAvailable(int Details)
        {
            bool result = true;
            result= IsExist<AssetItemDetail>(x => x.Id == Details && x.IsDeleted == false && x.Status =="Available" && x.CompanyId == SelectedCompany.Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetFullSupplierDetails(string SupplierName)
        {
            Supplier sup = new Supplier();
            Supplier SupplierDetail = GetListData<Supplier>(x => x.IsDeleted == false && x.Name == SupplierName).FirstOrDefault();
            if (SupplierDetail != null)
            {
                sup = SupplierDetail;
            }
            return Json(sup, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssetItemFullDetail(string Id)
        {
            AssetItemDetail AssetItem = new AssetItemDetail();
    
            AssetItemDetail AssetItemDetail = GetDataInfo<AssetItemDetail>(x => x.IsDeleted == false && x.ItemId == Id && x.Status == "Available" && x.CompanyId == SelectedCompany.Id);
            if (AssetItemDetail != null)
            {
                AssetItem = AssetItemDetail;

            }
            return Json(new { ItemId = AssetItem.Id, SerialNumber = AssetItem.SerialNumber, ItemKeyDetail = AssetItem.ItemKeyDetail, ItemRemarks = AssetItem.ItemRemarks, ItemDescription = AssetItem.ItemDescription, PartNumber = AssetItem.PartNumber }, JsonRequestBehavior.AllowGet); ;
        }

        public PartialViewResult AddNewAssetDetails()
        {
            AssetItemDetail asset = new AssetItemDetail();
            var types = GetListData<LookUpList>(x => x.GroupOf == "AssetItemTypes" && x.IsDeleted==false).OrderBy(x=>x.Name);
            ViewBag.ItemTypes = new SelectList(types,"Description","Name");
            return PartialView("AssetItemDetailsRows", asset);
        }

      [HttpPost]
      [ValidateAntiForgeryToken]
        public ActionResult UpgradeAssetDetail(AssetHeaderDetails Details)
        {
            AssetHeaderDetails asset = new AssetHeaderDetails();
            asset = GetDataInfo(x => x.Id == Details.Id && x.CompanyId == SelectedCompany.Id);
            asset.Description = Details.Description;
            asset.LifeSpanMonths = Details.LifeSpanMonths;
            asset.AssetRemarks = Details.AssetRemarks;
            asset.InvoiceDetails.InvoiceNumber = Details.InvoiceDetails.InvoiceNumber;
            asset.Price = Details.Price;
            asset.InvoiceDetails.SupplierDetails = Details.InvoiceDetails.SupplierDetails;
            asset.PuchaseOrderNumber = Details.PuchaseOrderNumber;
            asset.ControlNumber = Details.ControlNumber;
       

            
            if(Details.AssetItemDetails != null)
            {
                foreach (AssetItemDetail item in Details.AssetItemDetails.Where(x => x.Id != 0))
                {
                    AssetItemDetail ForUpdateItem = GetDataInfo<AssetItemDetail>(x => x.Id == item.Id);
                    if (ForUpdateItem.Status != item.Status)
                    {
                        ForUpdateItem.Status = item.Status;
                    }

                    UpdateData<AssetItemDetail>(m => m.Id, ForUpdateItem);
                }

                foreach (AssetItemDetail item in Details.AssetItemDetails.Where(x => x.Id == 0))
                {
                    //item.AssetHeaderId = asset.Id;
                    item.Status = "Available";
                    item.ItemId = GenerateItemId(item.ItemType, asset.BranchId);
                    asset.AssetItemDetails.Add(item);
                }
            }
          




            if(Details.assetHeaderFiles != null && Details.assetHeaderFiles.Count() > 0)
            {
                int[] existing = asset.assetHeaderFiles.Where(x=>x.IsDeleted == false).Select(x => x.Id).ToArray();
                int[] newlyPosted = Details.assetHeaderFiles.Select(x => x.Id).ToArray();
                foreach (AssetHeaderFiles item in Details.assetHeaderFiles)
                {
                    if (!existing.Contains(item.Id))
                    {
                        item.byteContent = item.convertFileToByte(item.ImageModel);
                        item.contentType = item.ImageModel.ContentType;
                        item.contentLenght = item.ImageModel.ContentLength;
                        item.FileName = item.ImageModel.FileName;
                        asset.assetHeaderFiles.Add(item);
                    }
                    else if(existing.Contains(item.Id) && newlyPosted.Contains(item.Id))
                    {
                        //do nothing
                    }
                    else
                    {
                        AssetHeaderFiles file = asset.assetHeaderFiles.Where(x => x.Id == item.Id).FirstOrDefault();
                        file.IsDeleted = true;
                        UpdateData<AssetHeaderFiles>(x => x.Id, file);

                    }
                }
            }
            else
            {
                foreach(AssetHeaderFiles item in asset.assetHeaderFiles)
                {
                    AssetHeaderFiles file = asset.assetHeaderFiles.Where(x => x.Id == item.Id).FirstOrDefault();
                    file.IsDeleted = true;
                    UpdateData<AssetHeaderFiles>(x => x.Id, file);
                }
            }
          


            UpdateData<AssetHeaderDetails>(m => m.Id, asset);
            SaveDatabase();
            return RedirectToAction("Details", new { Id = asset.Id });
        }

        public PartialViewResult AddNewAssetUpgradeDetail()
        {
            AssetItemDetail asset = new AssetItemDetail();
            var types = GetListData<LookUpList>(x => x.GroupOf == "AssetItemTypes" && x.IsDeleted == false);
            ViewBag.ItemTypes = new SelectList(types, "Description", "Name");
            return PartialView("UpgradeAssetBufferDetails", asset);
        }

       
    }
}
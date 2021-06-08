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
    public class AssetItemController : AssetTransactionBaseController<AssetItemDetail>
    {


        public override void UpdateExtraProcess(AssetItemDetail newEntity, AssetItemDetail oldEntity)
        {
            oldEntity.ItemDescription = newEntity.ItemDescription;
            oldEntity.PartNumber = newEntity.PartNumber;
            oldEntity.SerialNumber = newEntity.SerialNumber;
            oldEntity.ControlNumber = newEntity.ControlNumber;
        }

        public ActionResult ItemHistory(int Id)
        {
            List<Consignment> consignmentItems = new List<Consignment>();
            consignmentItems = GetListData<ConsignmentAssetItem>(x => x.AssetItemId == Id).Select(x=>x.ConsignmentDetails).ToList();

            return View(consignmentItems);
        }


        public override ActionResult Update(AssetItemDetail ente)
        {
            base.Update(ente);
            AssetItemDetail item = GetDataInfo(x => x.Id == ente.Id);
            return RedirectToAction("UpgradeAsset", "Asset", new { Id = item.AssetHeaderDetail.Id });
        }

        ////// GET: AssetItem
        public override ActionResult Index()
        {
            List<AssetItemDetail> buffer = GetListData<AssetItemDetail>(x =>  x.IsDeleted == false && x.CompanyId == SelectedCompany.Id).ToList();
            return View(buffer);
        }


        public ActionResult AddNewItem()
        {
            return View();
        }



        public ActionResult getItemByStatus(string status)
        {
            var data = GetListData(x => x.CompanyId == SelectedCompany.Id && x.Status == status).Select(x => new { x.AssetHeaderDetail.AssetNumber, x.ItemId, x.ItemDescription });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdatedData()
        {
            string[] statuses = new string[] { "Available", "Defective", "OnUsed" };
            IEnumerable<AssetItemDetail> assetItem = GetListData(x => x.CompanyId == SelectedCompany.Id && statuses.Contains(x.Status));
            var data = from item in assetItem
                       group item by item.Status into itemGrouped
                       select new
                       {
                           label = itemGrouped.Key,
                           value = itemGrouped.Count()
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchItem(string ItemId)
        {
            AssetItemDetail item = GetDataInfo(x => x.ItemId == ItemId && x.Status=="OnUsed");
            
            return RedirectToAction("Details","Asset",new {Id = item.AssetHeaderId });
        }

        public override void DetailsExtraProcessing(AssetItemDetail entity)
        {
            base.DetailsExtraProcessing(entity);
        }

        public PartialViewResult NewAssetItem()
        {
            List<LookUpList> ItemTypes = new List<LookUpList>();
            var Branches = GetListData<Branch>(x => x.IsDeleted == false);
            ItemTypes = GetListData<LookUpList>(x => x.GroupOf == "AssetItemTypes" && x.IsDeleted == false).OrderBy(x => x.Name).ToList();
            ViewBag.Branches = new SelectList(Branches, "Id", "Name");
            ViewBag.ItemTypes = new SelectList(ItemTypes, "Description", "Name");
            return PartialView("AssetItemDetailRowsWithPrice");
        }

    }
}
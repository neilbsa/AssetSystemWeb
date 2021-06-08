using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SystemEntities;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using SystemEntities.ViewModels;

namespace AssetSystemWeb.Controllers
{
    public class AssetTransactionBaseController<TEntity> : TransactionBaseControllerMethods<TEntity> where TEntity : class, IAssetBaseDetail
    {
        public virtual string GenerateItemId(string itemType, int? selBranch)
        {
            var selectedBranch = GetDataInfo<Branch>(x => x.Id == selBranch);
            //int currentNum = GetListData<AssetItemDetail>(x => x.ItemType == itemType 
            //                                                && x.CompanyId == SelectedCompany.Id 
            //                                                && x.AssetHeaderDetail.BranchId == selectedBranch.Id
            //                                                ).Select(x => x.ItemId).Distinct().Count() + 1;

            int currentNum;
            string group = String.Format("ItemIdSeries-{0}", SelectedCompany.Id);
            LookUpList ent = GetDataInfo<LookUpList>(x => x.GroupOf == group && x.stringDetails == selectedBranch.AssetTag && x.Name == itemType);
          
            if (ent == null)
            {
                LookUpList newLookup = new LookUpList()
                {
                    GroupOf = String.Format("ItemIdSeries-{0}", SelectedCompany.Id),
                    stringDetails = selectedBranch.AssetTag,
                    Name = itemType,
                    NumericDetail = 1
                };
                AddBaseToContext<LookUpList>(newLookup);
                currentNum = 1;
            }
            else
            {
                currentNum = Convert.ToInt32(ent.NumericDetail) + 1;
                ent.NumericDetail = currentNum;
                UpdateData<LookUpList>(x => x.Id, ent);
            }

            context.SaveContextChanges();

          
            return ItemIdFormat(itemType, selectedBranch, currentNum);
        }


   



        public override ActionResult CreateEntity(TEntity ent, bool CanAdd, string ActionName = null, Action<TEntity> ExtraProcessAfterAdding = null)
        {
            ent.Subsidiary = GetSubsidiary();
            return base.CreateEntity(ent, CanAdd, ActionName, ExtraProcessAfterAdding);
        }

        public virtual string ItemIdFormat(string itemType, Branch selectedBranch, int currentNum)
        {
            return String.Format("{0}{1}{2}-{3}", SelectedCompany.AssetTag, selectedBranch.AssetTag, itemType, currentNum.ToString("D3"));
        }

        public virtual string GetSubsidiary()
        {
            return "COMP-ASSET";
        }    
        public virtual string GenerateAssetId()
        {
            //int CurrentAssetNumber = GetListData<AssetHeaderDetails>().Select(x => x.AssetNumber).Distinct().Count() + 1;
            //string sub = GetSubsidiary();
            LookUpList ent = GetDataInfo<LookUpList>(x => x.GroupOf == "AssetIdCount");
            int CurrentAssetNumber;

            if (ent == null)
            {
                CurrentAssetNumber = 1;
                LookUpList newAssetCount = new LookUpList()
                {
                    GroupOf = "AssetIdCount",
                    NumericDetail = 1
                };
                AddBaseToContext<LookUpList>(newAssetCount);
            }
            else
            {
                CurrentAssetNumber = 0;
                CurrentAssetNumber = Convert.ToInt32(ent.NumericDetail) + 1;
                ent.NumericDetail = CurrentAssetNumber;
                UpdateData<LookUpList>(x => x.Id, ent);
            }


            return AssetIdFormat(CurrentAssetNumber);
        }
        public virtual string AssetIdFormat(int CurrentAssetNumber)
        {
            return "2CS" + CurrentAssetNumber.ToString("D4");
        }

  
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemEntities.GeneralModels;
using SystemEntities.Models;

namespace AssetSystemWeb.Controllers.CSD
{
    public class SalesmanAssetController : AssetController
    {
       

        public override string AssetIdFormat(int CurrentAssetNumber)
        {
            return "2SM" + CurrentAssetNumber.ToString("D4");
        }
        public override string ItemIdFormat(string itemType, Branch selectedBranch, int currentNum)
        {
            return String.Format("{0}{1}-{2}", "SM", itemType,currentNum.ToString("D4"));
        }
        public override string GetSubsidiary()
        {
            return "SM-ASSET";
        }

        


    }
} 
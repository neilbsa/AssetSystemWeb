using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SystemEntities.GeneralModels;
using SystemEntities.Models;

namespace SystemEntities.ViewModels
{

    public class lookupTentity : IEquatable<lookupTentity>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string GroupOf { get; set; }


        public bool Equals(lookupTentity other)
        {
            return Description == other.Description &&
                Description2 == other.Description2 &&
                GroupOf == other.GroupOf;
        }
    }

    public class ConsignmentUploadModel
    {
        public Consignment Consignment { get; set; }
        public HttpPostedFileBase File { get; set; }
    }


    public class ConsignmentViewModel
    {
        public int?  UserId { get; set; }
        public string Remarks { get; set; }
        public IEnumerable<AssetHeaderDetails> Assets { get; set; }
    }

    public class ReportFooterViewModel
    {
        public string RecieverName { get; set; }
    }


    public class BufferStockViewModel
    {
        public AssetInvoice InvoiceDetails { get; set; }
        public IEnumerable<AssetItemDetail> AssetItemDetails { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ControlNumber { get; set; }
        //public int? BranchId { get; set; }
    }




}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SystemEntities.GeneralModels;

namespace SystemEntities.Models
{
    public class FileRepositoryItem : IDataChangedTracker
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }


        public string DocumentType { get; set; }
        public byte[] byteContent { get; set; }
        public string contentType { get; set; }
        public decimal contentLenght { get; set; }
        public string FileName { get; set; }


        [NotMapped]
        public string content64base { get { return Convert.ToBase64String(byteContent); } }
        [NotMapped]
        public HttpPostedFileBase ImageModel { get; set; }


        public DateTime? LastDateUpdate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public int KeyId { get; set; }


        public byte[] convertFileToByte(HttpPostedFileBase item)
        {

            byte[] value = new byte[item.ContentLength];
            if (item.ContentLength > 0 && item != null)
            {
                //this return arrayed Image
                item.InputStream.Read(value, 0, item.ContentLength);
            }
            return value;
        }

    }


    public class AssetHeaderFiles : FileRepositoryItem
    {
        [ForeignKey("KeyId")]
        public virtual AssetHeaderDetails AssetHeaderDetails { get; set; }
    }

    public class ConsignmentFiles : FileRepositoryItem
    {
   
    }


    public class Consignment : IDataChangedTracker
    {
        [Key, Column("ConsignmentId")]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string ConsignmentNumber { get; set; }
        public int? UserId { get; set; }
        public string LastUpdatedBy { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        public DateTime DateIssueDate { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile CurrentUser { get; set; }
        public virtual ICollection<ConsignmentAssetItem> Assets { get; set; }
        public DateTime? LastDateUpdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public virtual ICollection<AssetItemDetail> IncludedAssetsDetails { get; set; }
        public int? fileId{ get; set; }
        [ForeignKey("fileId")]
        public virtual ConsignmentFiles RecievingCopyFile { get; set; }
    }



    public class ConsignmentAssetItem : IBaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("ConsignmentId")]
        public virtual Consignment ConsignmentDetails { get; set; }
        public int ConsignmentId { get; set; }
        public string Status { get; set; }
        [ForeignKey("AssetItemId")]
        public virtual AssetItemDetail AssetItemDetails { get; set; }
        public int AssetItemId { get; set; }

    }
    public class Department : ICompanyTransaction
    {

        public int Id
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }




        public string Description { get; set; }
        public string DepartmentCode { get; set; }

        public int? CompanyId
        {
            get
           ;
            set
            ;
        }
        [ForeignKey("CompanyId")]
        public virtual Company Company
        {
            get
          ;
            set
           ;
        }
    }
    public class AssetHeaderDetails : IAssetTransaction
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a warranty years greater than {1}")]
     
        public int WarrantyMonths { get; set; }
        public double LifeSpanMonths { get; set; }
        public string AssetNumber { get; set; }
        [Required]
        public string Description { get; set; }
        public string AssetRemarks { get; set; }
        public DateTime? DateRecieved { get; set; }
        public DateTime? DateIssued { get; set; }
        //public string Status { get; set; }

        public string Subsidiary { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string PuchaseOrderNumber { get; set; }
        public string ControlNumber { get; set; }
     
        public virtual ICollection<AssetItemDetail> AssetItemDetails { get; set; }

        //public virtual List<FileRepositoryItem> SupportingDocuments { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual AssetInvoice InvoiceDetails { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double DepreciatedValue
        {
            get
            {
                TimeSpan diff = DateTime.Today - PurchaseDate;
                double ExpectedLifeSpanDay = LifeSpanMonths * 30;
                double SalvageValue = Price / ExpectedLifeSpanDay;
                double currentVal = Math.Round(Price - (diff.TotalDays * SalvageValue), 2);
                return currentVal;
            }
        }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        [NotGreatedThanCurrentDate(ErrorMessage = "Cannot Accept dates greater than NOW")]
        public DateTime PurchaseDate { get; set; }
        public DateTime? LastDateUpdate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CompanyId
        {
            get;

            set
           ;
        }
        [ForeignKey("CompanyId")]
        public virtual Company Company
        {
            get
          ;

            set
          ;
        }
        public int? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        [Required]
        public string Vendor { get; set; }
        public string AssetType { get; set; }


        //[Required]
        //public double LifeSpanYear { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a Asset price value greater than {1}")]
        public double Price
        {
            get
           ;
            set;
        }
        public virtual ICollection<AssetHeaderFiles> assetHeaderFiles { get; set; }
    }
    public class AssetItemDetail : IAssetBaseDetail
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string ItemId { get; set; }
        [Required]
        public string ItemType { get; set; }
        [Required]
        public string ItemDescription { get; set; }
        [Required]
        public string PartNumber { get; set; }
        //[Required]
        public string ControlNumber { get; set; }
        [Required]
        public string SerialNumber { get; set; }
        [Required]
        public string ItemKeyDetail { get; set; }
        public string ItemRemarks { get; set; }
        [Required]
        public string Status { get; set; }
        //public int? ConsignmentId { get; set; }
        //[ForeignKey("ConsignmentId")]
        //public virtual Consignment ConsignmentDetails { get; set; }



        public int? AssetHeaderId { get; set; }
        [ForeignKey("AssetHeaderId")]
        public virtual AssetHeaderDetails AssetHeaderDetail { get; set; }
        public DateTime? LastDateUpdate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Subsidiary { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }
    public class AssetInvoice : IDataChangedTracker
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier SupplierDetails { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        [NotGreatedThanCurrentDate(ErrorMessage = "Cannot Accept dates greater than NOW")]
        public DateTime InvoiceDate { get; set; }
        public DateTime? LastDateUpdate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }



    public class NotGreatedThanCurrentDate : ValidationAttribute
    {
        public NotGreatedThanCurrentDate()
        {

        }

        public override bool IsValid(object value)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return date < DateTime.Now;
            }

            return true;
        }


    }



}

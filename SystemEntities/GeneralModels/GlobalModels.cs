using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemEntities.Models;

namespace SystemEntities.GeneralModels
{
    public class LookUpList : IBaseEntity
    {
        [Key, Column("LookUpListId")]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string GroupOf { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double NumericDetail { get; set; }
        public bool  BoolDetail { get; set; }
        public string stringDetails { get; set; }

    }

    public class Branch : ICompanyTransaction
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AssetTag { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }




    public class Company : IBaseEntity
    {
        [Key, Column("CompanyId")]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string AssetTag { get; set; }
  
    }


    //public class Branch : IBaseEntity
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public bool IsDeleted { get; set; }
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public string Code { get; set; }
    //    public string AssetTag { get; set; }
    //}


    public class Supplier : IBaseEntity
    {
        [Key, Column("SupplierId")]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAdd { get; set; }
    }


    public class UserCompany : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

    }




}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemEntities.GeneralModels;

namespace SystemEntities.Models
{
    public class UserProfile : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string CompEmployeeNum { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company CompanyDetail { get; set; }
        [Required]
        public string Firstname { get; set; }

        public string Middlename { get; set; }
        [Required]
        public string Surname { get; set; }



        [Required]
        public int DepartmentId { get; set; }      
        [ForeignKey("DepartmentId")]
        public virtual Department DepartmentDetail { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch BranchDetails { get; set; }


        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [NotMapped]
        public string Fullname
        {
            get
            {
                if(UserType == "Users")
                {
                    return String.Format("{0} {1}, {2}", Firstname,Middlename, Surname);

                }
                else
                {
                    return String.Format("{0}-{1}", Firstname, Surname);
                }
            }
        }
        public string Subsidiary { get; set; }
        public string UserType { get; set; }
    }
}

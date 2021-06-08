using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SystemEntities.GeneralModels;

namespace SystemEntities
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        bool IsDeleted { get; set; }

    }

    public interface IAssetTransaction: IDepreciatingEntity, IAssetBaseDetail
    {
       
        string Vendor { get; set; }
        string AssetType { get; set; }
    }

    public interface IAssetBaseDetail : IDataChangedTracker
    {

        string Subsidiary { get; set; }

    }

    public interface IDataChangedTracker: ICompanyTransaction
    {
        DateTime? LastDateUpdate { get; set; }
        string LastUpdatedBy { get; set; }
        DateTime? CreatedDate { get; set; }
        string CreatedBy { get; set; }
    }

    public interface IBaseMethod<TEntity> where TEntity : IBaseEntity
    {
        IEnumerable<TEntity> GetList(Expression<Func<TEntity,bool>> t);
        void AddtoContext(TEntity t);
        void DeletetoContext(TEntity t);
        void UpdatetoContext(TEntity newT);
    }

    public interface IDepreciatingEntity : IBranchTransaction
    {
        double DepreciatedValue { get; }
        DateTime PurchaseDate { get; set; }
        double LifeSpanMonths { get; set; }
        double Price { get; set; }

    }
    public interface ICompanyTransaction: IBaseEntity
    {
        int? CompanyId { get; set; }
        Company  Company { get; set; }
    }

    public interface IBranchTransaction : IBaseEntity
    {
        int? BranchId { get; set; }
        Branch Branch { get; set; }
    }

  
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SystemEntities;
using SystemEntities.GeneralModels;
using SystemEntities.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;

namespace SystemProcedure.Asset
{
    public class DbConnect<TEntity> : IBaseMethod<TEntity> where TEntity : class,IBaseEntity
    {
        ApplicationDbContext  context{get; set;}
        DbSet<TEntity> dbset;
        public DbConnect()
        {
            this.context = new ApplicationDbContext();
            this.dbset = context.Set<TEntity>();
        }

        public void Dispose()
        {
            context.Dispose();
        }

      
        public void AddtoContext(TEntity t)
        {

            //try
            //{

            //}catch(E)
        
                dbset.Add(t);
                SaveContextChanges();
          
             
        }

     

        public void SaveContextChanges()
        {
            context.SaveChanges();
            //context.Dispose();
        }

        public void DeletetoContext(TEntity t)
        {
            throw new NotImplementedException();
        }

    


        public bool CheckIfExistToContext(Expression<Func<TEntity, bool>> T)
        {
            return dbset.Any(T);
        }


        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> t = null)
        {
            var query = dbset.ToList();
            if(null != t)
            {
               query = dbset.Where(t).ToList();
            }
            return query;
        }




        public TEntity GetInfo(Expression<Func<TEntity, bool>> t = null)
        {
            TEntity ent;
            ent = dbset.Where(t).OrderByDescending(x => x.Id).FirstOrDefault();
            return ent;
        }

        public void UpdatetoContext(Expression<Func<TEntity, object>> T, TEntity newT)
        {
            dbset.AddOrUpdate(T , newT);
            SaveContextChanges();
        }

        public void UpdatetoContext(TEntity newT)
        {
            throw new NotImplementedException();
        }
    }
}

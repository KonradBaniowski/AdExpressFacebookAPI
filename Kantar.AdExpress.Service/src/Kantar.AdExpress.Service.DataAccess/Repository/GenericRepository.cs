using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.Core.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Kantar.AdExpress.Service.DataAccess.Repository
{
    public class GenericRepository<Context, TEntity> : IGenericRepository<TEntity> where TEntity : class where Context : DbContext, new()
    {
        internal Context _context;
        internal DbSet<TEntity> _dbSet;

        protected Context mycontext { get { return _context; } }

        public GenericRepository(Context context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

      
        public IQueryable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Edit(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }
            return query.FirstOrDefault(predicate);
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}

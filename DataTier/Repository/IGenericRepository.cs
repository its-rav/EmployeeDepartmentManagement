using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataTier.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Get();
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        EntityEntry<T> Insert(T obj);
        void Update(T obj);
        void InsertRange(List<T> obj);
        void UpdateRange(List<T> obj);
        IQueryable<T> FindAllByProperty(Expression<Func<T, bool>> predicate);
        T FindFirstByProperty(Func<T, bool> expression);
        int Commit();
    }
}

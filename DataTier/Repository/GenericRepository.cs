
using DataTier.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataTier.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected EDMContext _context;
        protected DbSet<T> table = null;

        public GenericRepository(EDMContext context)
        {
            this._context = context;
            table = _context.Set<T>();
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public IQueryable<T> FindAllByProperty(Expression<Func<T, bool>> predicate)
        {
            return table.Where(predicate);
        }

        public T FindFirstByProperty(Func<T, bool> expression)
        {
            return table.FirstOrDefault(expression);
        }

        public IQueryable<T> Get()
        {
            return table;
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return table.Where(predicate);
        }
        public EntityEntry<T> Insert(T obj)
        {
            var result = table.Add(obj);
            return result;
        }

        public void InsertRange(List<T> obj)
        {
            table.AddRange(obj);
        }

        public void Update(T obj)
        {
            table.Update(obj);
        }

        public void UpdateRange(List<T> obj)
        {
            table.UpdateRange(obj);
        }


    }
}

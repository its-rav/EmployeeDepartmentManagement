using DataTier.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTier.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>()
       where T : class;

        int Commit();

    }
}

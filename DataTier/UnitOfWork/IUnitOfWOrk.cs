using DataTier.Repository;
using System;

namespace DataTier.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>()
       where T : class;

        int Commit();

    }
}

using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Provides access to repository via UnitOfWork
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> GetRepository<T>() where T : class;

        /// <summary>
        /// Applies the changes to the database. Returns an int value, 1 if successful, 0 if unsuccessful.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}

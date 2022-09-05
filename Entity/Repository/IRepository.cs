using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {

        void Add(T entity);

        void Delete(T entity);


        /// <returns></returns>
        T Get(Guid id);

        T Get(Expression<Func<T, bool>> condition);

        void Update(T entity);

        public IQueryable<T> GetAll(Expression<Func<T, bool>> condition);

        public IQueryable<T> GetAll();
    }
}

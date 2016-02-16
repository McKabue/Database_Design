using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Timothy_Anondo
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Edit(T entity);
        T FindBy(Expression<Func<T, bool>> findBy);
        T FindBy(Expression<Func<T, bool>> findBy, string includeProperties);
        List<T> GetAll();
        List<T> SearchBy(Expression<Func<T, bool>> searchBy);
        List<T> SearchBy(Expression<Func<T, bool>> searchBy, string includeProperties);
        List<T> SearchBy(Expression<Func<T, bool>> searchBy, Func<List<T>, IOrderedQueryable<T>> orderBy, int size);
        void Update(T entity);
        void AddRange(IEnumerable<T> entities);
    }
}

using System.Collections.Generic;

namespace WebsiteSellingSport.IReposiory
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetByID(long id);
        int Create(T item);
        int Update(T item);
        int Delete(long id);


    }
}

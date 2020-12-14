using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Infrastructure.Abstract
{
   public interface IRepository<T>
    {
        T GetById(int id);

        List<T> GetAll();

        Task Create(T entity);

        Task Update(T entity);

        Task Delete(T entity);
    }
}

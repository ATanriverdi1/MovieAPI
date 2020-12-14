using MoviesApi.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Infrastructure.Abstract
{
    public interface IGenreRepository : IRepository<Genre>
    {
    }
}

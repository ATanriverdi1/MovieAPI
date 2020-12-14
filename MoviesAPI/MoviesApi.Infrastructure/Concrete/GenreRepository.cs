using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entity;
using MoviesApi.Infrastructure.Abstract;
using MoviesApi.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Infrastructure.Concrete
{
    public class GenreRepository : IGenreRepository
    {
        public Task Create(Genre entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Genre entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Genre>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Genre> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Genre entity)
        {
            throw new NotImplementedException();
        }
    }
}

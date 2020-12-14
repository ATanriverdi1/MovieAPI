using Microsoft.EntityFrameworkCore;
using MoviesApi.Infrastructure.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Infrastructure.Concrete
{
    public class GenericRepository<TEntity, TContext> : IRepository<TEntity>
         where TEntity : class
         where TContext : DbContext, new()
    {
        public async Task Create(TEntity entity)
        {
            using (var _context = new TContext())
            {
                _context.Set<TEntity>().Add(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(TEntity entity)
        {
            using (var _context = new TContext())
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TEntity>> GetAll()
        {
            using (var context = new TContext())
            {
                return await context.Set<TEntity>().ToListAsync();
            }
        }

        public async Task<TEntity> GetById(int id)
        {
            using (var context = new TContext())
            {
                return await context.Set<TEntity>().FindAsync(id);
            }
        }

        public async Task Update(TEntity entity)
        {
            using (var _context = new TContext())
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}

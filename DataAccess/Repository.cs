using BlogPersonal.Data;
using BlogPersonal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogPersonal.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        protected DbSet<T> Entities => _context.Set<T>();

        public async Task AddAsync(T entity)
        {
            await Entities.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            T entidadABorrar = await GetByIdAsync(id);
            Entities.Remove(entidadABorrar);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await Entities.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetAll()
        {
            return Entities;
        }

        public async Task EditAsync(T entity)
        {
            Entities.Update(entity);
        }
    }
}

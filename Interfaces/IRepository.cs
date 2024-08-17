namespace BlogPersonal.Interfaces
{
    public interface IRepository<T>
    {
        public Task AddAsync(T entity);
        public Task DeleteAsync(T entity);
        public IQueryable<T> GetAll();
        public Task<T?> GetByIdAsync(int id);
        public Task EditAsync(T entity);
        public Task SaveChangesAsync();
    }
}

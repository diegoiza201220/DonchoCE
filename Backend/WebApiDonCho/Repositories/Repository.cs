using Microsoft.EntityFrameworkCore;
using WebApiDonCho.Context;
using WebApiDonCho.Interfaces;

namespace WebApiDonCho.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DonchoContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DonchoContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        //var watch = System.Diagnostics.Stopwatch.StartNew();
        // Llamada al método
        //var elapsedMs = watch.ElapsedMilliseconds;
        //elapsedMs = watch.ElapsedMilliseconds;
        //Console.WriteLine($" tiempo REP ALL 00: {elapsedMs}");
        return await _dbSet.AsNoTracking().ToListAsync();
        //elapsedMs = watch.ElapsedMilliseconds;
        //Console.WriteLine($" tiempo REP ALL 01: {elapsedMs}");
        //watch.Stop();
        //return r;
    }

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);
}

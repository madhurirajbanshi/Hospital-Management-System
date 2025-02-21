using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hospital.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    internal DbSet<T> DbSet;


    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }
    public T GetById(object id)
    {
        return DbSet.Find(id);
    }

    public async Task<T> GetByIdAsync(object id)
    {
        return await DbSet.FindAsync(id);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        DbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        DbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public void Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        DbSet.Remove(entity);
    }

    public async Task<T> DeleteAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        DbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IEnumerable<T> GetAll(
             Expression<Func<T, bool>>? filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
             string includeProperties = "")
    {
        IQueryable<T> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (string includeProperty in includeProperties.Split(',' , StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return orderBy(query).ToList();
        }
        else
        {
            return query.ToList();
        }
    }
}


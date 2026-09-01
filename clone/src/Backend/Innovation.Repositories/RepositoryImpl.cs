using System.Linq.Expressions;
using Innovation.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Innovation.Repositories;

// Generic implementation shared by every entity (Backend ROADMAP §4.1). This
// part of the original pattern is not a bug and is preserved as-is:
// AsNoTracking() reads, and it never calls SaveChanges() itself - the
// UnitOfWork owns the transaction boundary and decides when to save.
public class RepositoryImpl<T> : IRepository<T> where T : class, new()
{
    protected readonly DbContext Context;

    public RepositoryImpl(DbContext context)
    {
        Context = context;
    }

    public T? Get(object id)
    {
        var entity = Context.Set<T>().Find(id);
        if (entity != null)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public T? Find(Expression<Func<T, bool>> predicate) =>
        Context.Set<T>().AsNoTracking().Where(predicate).FirstOrDefault();

    public IQueryable<T> GetAll() => Context.Set<T>().AsNoTracking();

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate) =>
        Context.Set<T>().AsNoTracking().Where(predicate);

    public void Add(T entity) => Context.Set<T>().Add(entity);

    public void AddRange(IEnumerable<T> entities) => Context.Set<T>().AddRange(entities);

    public void BulkInsert(List<T> entities) => Context.Set<T>().AddRange(entities);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void TryUpdate(T entity)
    {
        var entries = Context.ChangeTracker.Entries()
            .Where(e => e.Entity.GetType() == entity.GetType())
            .ToList();
        entries.ForEach(x => x.State = EntityState.Detached);
        Context.Set<T>().Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities) => Context.Set<T>().UpdateRange(entities);

    public void TryUpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            TryUpdate(entity);
        }
    }

    public void BulkUpdate(List<T> entities) => Context.Set<T>().UpdateRange(entities);

    public void Delete(T entity) => Context.Set<T>().Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => Context.Set<T>().RemoveRange(entities);

    public void BulkDelete(List<T> entities) => Context.Set<T>().RemoveRange(entities);

    public List<dynamic> GetDynamicBySql(string sql, params object[] parameters) =>
        throw new NotSupportedException(
            "Raw SQL is out of scope for the demo slice - the real system used it in only 12 places " +
            "(Backend ROADMAP §7b.4), none on the SILO tables this clone covers.");

    public int GetMaxPK(Expression<Func<T, int>> predicate)
    {
        var set = Context.Set<T>();
        return set.Any() ? set.Max(predicate) : 0;
    }
}

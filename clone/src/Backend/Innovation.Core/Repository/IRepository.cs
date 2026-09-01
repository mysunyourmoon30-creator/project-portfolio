using System.Linq.Expressions;

namespace Innovation.Core.Repository;

// Copied from Backend ROADMAP §3.1 (the "currently used" interface, as
// opposed to the dead IGenericRepository<T> registration nobody injects).
// RepositoryImpl<T> in Innovation.Repositories implements this once,
// generically, for every entity - see Backend ROADMAP §4.1.
public interface IRepository<T> where T : class, new()
{
    T? Get(object id);
    T? Find(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetAll();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void BulkInsert(List<T> entities);
    void Update(T entity);
    void TryUpdate(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void TryUpdateRange(IEnumerable<T> entities);
    void BulkUpdate(List<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    void BulkDelete(List<T> entities);
    List<dynamic> GetDynamicBySql(string sql, params object[] parameters);
    int GetMaxPK(Expression<Func<T, int>> predicate);
}

namespace Innovation.Core.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    bool Save();
}

public interface IContextUnitOfWork : IDisposable, IUnitOfWork
{
    bool CheckConnection();
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    void DetachAllEntities();
}

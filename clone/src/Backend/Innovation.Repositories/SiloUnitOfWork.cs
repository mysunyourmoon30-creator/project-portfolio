using Innovation.Core.Repository;
using Innovation.Core.UnitOfWork;
using Innovation.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Innovation.Repositories;

public sealed class SiloUnitOfWork : ISiloUnitOfWork
{
    private readonly SiloDbContext _context;
    private IDbContextTransaction? _transaction;

    public SiloUnitOfWork(SiloDbContext context)
    {
        _context = context;
    }

    private IKbTogetherRepository? _kbTogetherRepository;
    public IKbTogetherRepository KbTogetherRepository =>
        _kbTogetherRepository ??= new KbTogetherRepository(_context);

    private IWeightingRepository? _weightingRepository;
    public IWeightingRepository WeightingRepository =>
        _weightingRepository ??= new WeightingRepository(_context);

    private ITotalWeightRepository? _totalWeightRepository;
    public ITotalWeightRepository TotalWeightRepository =>
        _totalWeightRepository ??= new TotalWeightRepository(_context);

    private ISendStepParameterRepository? _sendStepParameterRepository;
    public ISendStepParameterRepository SendStepParameterRepository =>
        _sendStepParameterRepository ??= new SendStepParameterRepository(_context);

    private ITrayPlanRepository? _trayPlanRepository;
    public ITrayPlanRepository TrayPlanRepository =>
        _trayPlanRepository ??= new TrayPlanRepository(_context);

    private ITrayWeightRepository? _trayWeightRepository;
    public ITrayWeightRepository TrayWeightRepository =>
        _trayWeightRepository ??= new TrayWeightRepository(_context);

    private ITwAcceptWeightHisRepository? _twAcceptWeightHisRepository;
    public ITwAcceptWeightHisRepository TwAcceptWeightHisRepository =>
        _twAcceptWeightHisRepository ??= new TwAcceptWeightHisRepository(_context);

    private ITypeTrayRepository? _typeTrayRepository;
    public ITypeTrayRepository TypeTrayRepository =>
        _typeTrayRepository ??= new TypeTrayRepository(_context);

    private ITrayBarcodeRepository? _trayBarcodeRepository;
    public ITrayBarcodeRepository TrayBarcodeRepository =>
        _trayBarcodeRepository ??= new TrayBarcodeRepository(_context);

    private IStationRepository? _stationRepository;
    public IStationRepository StationRepository =>
        _stationRepository ??= new StationRepository(_context);

    private IUsrWtRepository? _usrWtRepository;
    public IUsrWtRepository UsrWtRepository =>
        _usrWtRepository ??= new UsrWtRepository(_context);

    private IRmBalRepository? _rmBalRepository;
    public IRmBalRepository RmBalRepository =>
        _rmBalRepository ??= new RmBalRepository(_context);

    private ISiloApproveRepository? _siloApproveRepository;
    public ISiloApproveRepository SiloApproveRepository =>
        _siloApproveRepository ??= new SiloApproveRepository(_context);

    private IOnHandRepository? _onHandRepository;
    public IOnHandRepository OnHandRepository =>
        _onHandRepository ??= new OnHandRepository(_context);

    private IProdstdMixtempRepository? _prodstdMixtempRepository;
    public IProdstdMixtempRepository ProdstdMixtempRepository =>
        _prodstdMixtempRepository ??= new ProdstdMixtempRepository(_context);

    private IApplicationSettingRepository? _applicationSettingRepository;
    public IApplicationSettingRepository ApplicationSettingRepository =>
        _applicationSettingRepository ??= new ApplicationSettingRepository(_context);

    public bool Save() => _context.SaveChanges() > 0;

    public bool CheckConnection() => _context.Database.CanConnect();

    public void BeginTransaction() => _transaction = _context.Database.BeginTransaction();

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void DetachAllEntities()
    {
        foreach (var entry in _context.ChangeTracker.Entries().ToList())
        {
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

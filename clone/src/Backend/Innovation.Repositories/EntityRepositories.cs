using Innovation.Core.Entities;
using Innovation.Core.Repository;
using Innovation.Data;

namespace Innovation.Repositories;

// Thin shims closing RepositoryImpl<T> to each marker interface. In the
// original system these existed to bind a repository to one of 16 different
// typed DbContexts; here every shim binds to the same SiloDbContext, a
// direct consequence of the Phase 1 single-database decision.
internal sealed class KbTogetherRepository : RepositoryImpl<KbTogether>, IKbTogetherRepository
{
    public KbTogetherRepository(SiloDbContext context) : base(context) { }
}

internal sealed class WeightingRepository : RepositoryImpl<Weighting>, IWeightingRepository
{
    public WeightingRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TotalWeightRepository : RepositoryImpl<TotalWeight>, ITotalWeightRepository
{
    public TotalWeightRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TwAcceptWeightHisRepository : RepositoryImpl<TwAcceptWeightHis>, ITwAcceptWeightHisRepository
{
    public TwAcceptWeightHisRepository(SiloDbContext context) : base(context) { }
}

internal sealed class SendStepParameterRepository : RepositoryImpl<SendStepParameter>, ISendStepParameterRepository
{
    public SendStepParameterRepository(SiloDbContext context) : base(context) { }
}

internal sealed class StationRepository : RepositoryImpl<Station>, IStationRepository
{
    public StationRepository(SiloDbContext context) : base(context) { }
}

internal sealed class UsrWtRepository : RepositoryImpl<UsrWt>, IUsrWtRepository
{
    public UsrWtRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TrayPlanRepository : RepositoryImpl<TrayPlan>, ITrayPlanRepository
{
    public TrayPlanRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TrayWeightRepository : RepositoryImpl<TrayWeight>, ITrayWeightRepository
{
    public TrayWeightRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TrayBarcodeRepository : RepositoryImpl<TrayBarcode>, ITrayBarcodeRepository
{
    public TrayBarcodeRepository(SiloDbContext context) : base(context) { }
}

internal sealed class TypeTrayRepository : RepositoryImpl<TypeTray>, ITypeTrayRepository
{
    public TypeTrayRepository(SiloDbContext context) : base(context) { }
}

internal sealed class RmBalRepository : RepositoryImpl<RmBal>, IRmBalRepository
{
    public RmBalRepository(SiloDbContext context) : base(context) { }
}

internal sealed class SiloApproveRepository : RepositoryImpl<SiloApprove>, ISiloApproveRepository
{
    public SiloApproveRepository(SiloDbContext context) : base(context) { }
}

internal sealed class OnHandRepository : RepositoryImpl<OnHand>, IOnHandRepository
{
    public OnHandRepository(SiloDbContext context) : base(context) { }
}

internal sealed class ProdstdMixtempRepository : RepositoryImpl<ProdstdMixtemp>, IProdstdMixtempRepository
{
    public ProdstdMixtempRepository(SiloDbContext context) : base(context) { }
}

internal sealed class ApplicationSettingRepository : RepositoryImpl<ApplicationSetting>, IApplicationSettingRepository
{
    public ApplicationSettingRepository(SiloDbContext context) : base(context) { }
}

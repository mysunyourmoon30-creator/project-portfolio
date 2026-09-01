using Innovation.Core.Repository;

namespace Innovation.Core.UnitOfWork;

// Trimmed vs. the original 13-property ISiloUnitOfWork (Backend ROADMAP
// §3.2): RmConfirmMst/KbPlcSort dropped (T3-only). Extended with 5
// properties for tables consolidated from other databases (see
// ConsolidatedTables.cs).
public interface ISiloUnitOfWork : IContextUnitOfWork
{
    IKbTogetherRepository KbTogetherRepository { get; }
    IWeightingRepository WeightingRepository { get; }
    ITotalWeightRepository TotalWeightRepository { get; }
    ISendStepParameterRepository SendStepParameterRepository { get; }
    ITrayPlanRepository TrayPlanRepository { get; }
    ITrayWeightRepository TrayWeightRepository { get; }
    ITwAcceptWeightHisRepository TwAcceptWeightHisRepository { get; }
    ITypeTrayRepository TypeTrayRepository { get; }
    ITrayBarcodeRepository TrayBarcodeRepository { get; }
    IStationRepository StationRepository { get; }
    IUsrWtRepository UsrWtRepository { get; }

    IRmBalRepository RmBalRepository { get; }
    ISiloApproveRepository SiloApproveRepository { get; }
    IOnHandRepository OnHandRepository { get; }
    IProdstdMixtempRepository ProdstdMixtempRepository { get; }
    IApplicationSettingRepository ApplicationSettingRepository { get; }
}

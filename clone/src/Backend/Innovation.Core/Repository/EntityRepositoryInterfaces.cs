using Innovation.Core.Entities;

namespace Innovation.Core.Repository;

// Marker interfaces, one per entity - each just closes IRepository<T> to a
// concrete type (Backend ROADMAP §3.3: "แทบทุกตัวว่างเปล่าแบบนี้"). Bundled
// into one file here purely for brevity; the pattern itself is preserved.
public interface IKbTogetherRepository : IRepository<KbTogether> { }
public interface IWeightingRepository : IRepository<Weighting> { }
public interface ITotalWeightRepository : IRepository<TotalWeight> { }
public interface ITwAcceptWeightHisRepository : IRepository<TwAcceptWeightHis> { }
public interface ISendStepParameterRepository : IRepository<SendStepParameter> { }
public interface IStationRepository : IRepository<Station> { }
public interface IUsrWtRepository : IRepository<UsrWt> { }
public interface ITrayPlanRepository : IRepository<TrayPlan> { }
public interface ITrayWeightRepository : IRepository<TrayWeight> { }
public interface ITrayBarcodeRepository : IRepository<TrayBarcode> { }
public interface ITypeTrayRepository : IRepository<TypeTray> { }
public interface IRmBalRepository : IRepository<RmBal> { }
public interface ISiloApproveRepository : IRepository<SiloApprove> { }
public interface IOnHandRepository : IRepository<OnHand> { }
public interface IProdstdMixtempRepository : IRepository<ProdstdMixtemp> { }
public interface IApplicationSettingRepository : IRepository<ApplicationSetting> { }

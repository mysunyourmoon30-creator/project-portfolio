using Innovation.Core.Domain;
using Innovation.Core.Entities;
using Innovation.Core.UnitOfWork;
using Innovation.Services.Contracts;
using Innovation.Services.Errors;
using Innovation.Services.Security;

namespace Innovation.Services.Implementations;

// Constructor takes only interfaces resolved through DI - no static factory
// call anywhere (contrast with Backend ROADMAP §5's
// InnovationSiloApproveService, whose parameterless constructor calls
// UnitOfWorkFactory.GetDBTransectionUnitOfWork() directly).
public sealed class TotalWeightPlcService : ITotalWeightPlcService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly UsrWtPasswordHasher _passwordHasher;

    public TotalWeightPlcService(IUnitOfWorkFactory unitOfWorkFactory, UsrWtPasswordHasher passwordHasher)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _passwordHasher = passwordHasher;
    }

    public LoginResultDto Login(LoginRequestDto request)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        var user = uow.UsrWtRepository.Find(x => x.LoginName == request.Username)
            ?? throw new InvalidCredentialsException();

        if (!_passwordHasher.Verify(user, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        return new LoginResultDto(user.Id, user.LoginName, user.FullName);
    }

    public KanbanDetailDto GetKanban(string barcode)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        var kanban = uow.KbTogetherRepository.Find(x => x.Barcode == barcode)
            ?? throw new BarcodeNotFoundException(barcode);

        var (minSetting, maxSetting) = GetToleranceSettings(uow);

        var steps = uow.WeightingRepository.GetWhere(x => x.KbTogetherId == kanban.Id)
            .OrderBy(x => x.StepNo)
            .ToList()
            .Select(w =>
            {
                var (min, max) = WeightToleranceCalculator.Calculate(w.StepNo, kanban.Number, w.TargetWeight, minSetting, maxSetting);
                return new KanbanStepDto(w.StepNo, w.RawMaterialCode, w.TargetWeight, min, max, w.ActualWeight, w.Accepted);
            })
            .ToList();

        return new KanbanDetailDto(kanban.Id, kanban.Barcode, kanban.PlanId, kanban.Number, kanban.Status, steps);
    }

    public SaveTotalWeightResultDto SaveTotalWeight(SaveTotalWeightRequestDto request)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();

        if (uow.TotalWeightRepository.GetWhere(x => x.KbTogetherId == request.KbTogetherId).Any())
        {
            throw new TotalWeightAlreadyExistsException(request.KbTogetherId);
        }

        var weighingRows = uow.WeightingRepository.GetWhere(x => x.KbTogetherId == request.KbTogetherId).ToList();

        foreach (var stepWeight in request.Steps)
        {
            var row = weighingRows.FirstOrDefault(x => x.StepNo == stepWeight.StepNo)
                ?? throw new StepNotAcceptedException(stepWeight.StepNo);
            row.ActualWeight = stepWeight.ActualWeight;
            uow.WeightingRepository.Update(row);

            // Withdraw the raw material balance in the SAME unit of work as
            // the weight save below - both hit the same SiloDbContext, so
            // uow.Save() commits them together. This is the direct payoff
            // of Phase 1's single-database consolidation: the original
            // system's two-UnitOfWork, no-distributed-transaction bug
            // (Backend ROADMAP §7b.3) cannot occur here.
            var rmBal = uow.RmBalRepository.Find(x => x.RawMaterialBarcode == row.RawMaterialCode)
                ?? throw new RmBalNotFoundException(row.RawMaterialCode);
            rmBal.Balance -= stepWeight.ActualWeight;
            rmBal.UpdatedAt = DateTime.UtcNow;
            uow.RmBalRepository.Update(rmBal);
        }

        var totalActualWeight = weighingRows.Sum(x => x.ActualWeight ?? 0m);
        var totalWeight = new TotalWeight
        {
            KbTogetherId = request.KbTogetherId,
            TotalActualWeight = totalActualWeight,
            SavedAt = DateTime.UtcNow,
        };
        uow.TotalWeightRepository.Add(totalWeight);

        uow.Save();

        return new SaveTotalWeightResultDto(totalWeight.Id, totalWeight.TotalActualWeight);
    }

    public void Accept(AcceptStepRequestDto request)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        var step = uow.WeightingRepository.Find(x => x.KbTogetherId == request.KbTogetherId && x.StepNo == request.StepNo)
            ?? throw new StepNotAcceptedException(request.StepNo);

        if (step.ActualWeight is null)
        {
            // Guard: the operator must submit the current step's weight
            // before Accept is allowed (README §4.1).
            throw new StepNotAcceptedException(request.StepNo);
        }

        step.Accepted = true;
        uow.WeightingRepository.Update(step);
        uow.TwAcceptWeightHisRepository.Add(new TwAcceptWeightHis
        {
            StepNo = step.StepNo,
            AcceptedWeight = step.ActualWeight.Value,
            AcceptedAt = DateTime.UtcNow,
        });
        uow.Save();
    }

    public bool TotalWeightExists(int kbTogetherId) =>
        WithUnitOfWork(uow => uow.TotalWeightRepository.GetWhere(x => x.KbTogetherId == kbTogetherId).Any());

    public RmBalDto GetRmBal(string barcode) => WithUnitOfWork(uow =>
    {
        var rmBal = uow.RmBalRepository.Find(x => x.RawMaterialBarcode == barcode)
            ?? throw new RmBalNotFoundException(barcode);
        return new RmBalDto(rmBal.RawMaterialBarcode, rmBal.Balance);
    });

    public void ExecuteRmBalWithdraw(string barcode, decimal amount)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        var rmBal = uow.RmBalRepository.Find(x => x.RawMaterialBarcode == barcode)
            ?? throw new RmBalNotFoundException(barcode);
        rmBal.Balance -= amount;
        rmBal.UpdatedAt = DateTime.UtcNow;
        uow.RmBalRepository.Update(rmBal);
        uow.Save();
    }

    public FeeddoorStepDto GetFeeddoorStep(int lineId) => WithUnitOfWork(uow =>
    {
        var step = uow.SendStepParameterRepository.Find(x => x.LineId == lineId && x.Description == "Feeddoor Step")
            ?? throw new SettingNotFoundException("Feeddoor Step");
        return new FeeddoorStepDto(step.StepNo, step.PlcAddress, step.Description);
    });

    public MixTempDto? GetMixTemp(int planId) => WithUnitOfWork(uow =>
    {
        // No row here is NOT an error (README §8, Phase 3 scenario 6):
        // weighing must continue normally without warning.
        var mix = uow.ProdstdMixtempRepository.Find(x => x.PlanId == planId);
        return mix is null ? null : new MixTempDto(mix.PlanId, mix.MixPattern, mix.Temperature);
    });

    private static (decimal Min, decimal Max) GetToleranceSettings(ISiloUnitOfWork uow)
    {
        var min = uow.ApplicationSettingRepository.Find(x => x.Id == 4)?.Value;
        var max = uow.ApplicationSettingRepository.Find(x => x.Id == 5)?.Value;
        return (
            decimal.TryParse(min, out var minValue) ? minValue : 0m,
            decimal.TryParse(max, out var maxValue) ? maxValue : 0m);
    }

    private TResult WithUnitOfWork<TResult>(Func<ISiloUnitOfWork, TResult> action)
    {
        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        return action(uow);
    }
}

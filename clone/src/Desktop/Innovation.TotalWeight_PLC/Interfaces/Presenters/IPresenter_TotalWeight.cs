using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.Interfaces.Presenters;

public interface IPresenter_TotalWeight : IGeneralViewPresenter<IView_TotalWeight>
{
    Task LoadKanbanAsync(string barcode);
    Task SubmitStepWeightAsync(int stepNo, decimal actualWeight);
    Task SaveAsync();
    Task AcceptStepAsync(int stepNo);
}

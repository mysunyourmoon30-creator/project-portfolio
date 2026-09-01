using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Interfaces.Views;

public interface IView_SelectKB : IViewBase, IView<IPresenter_SelectKB>
{
    List<KanbanSummary> AvailableKanbans { get; set; }
    KanbanSummary? SelectedKanban { get; set; }
}

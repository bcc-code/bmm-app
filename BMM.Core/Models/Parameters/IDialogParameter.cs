using MvvmCross.Commands;

namespace BMM.Core.Models.Parameters;

public interface IDialogParameter
{
    string Title { get; }
    string Subtitle { get; }
    string CloseButtonText { get; }
    string Header { get; }
    Action CloseAction { get; set; }
    MvxCommand CloseCommand { get; }
}
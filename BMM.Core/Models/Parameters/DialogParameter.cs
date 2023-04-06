using MvvmCross.Commands;

namespace BMM.Core.Models.Parameters;

public class DialogParameter : IDialogParameter
{
    public DialogParameter(
        string title,
        string subtitle,
        string header,
        string closeButtonText)
    {
        Title = title;
        Subtitle = subtitle;
        Header = header;
        CloseButtonText = closeButtonText;
        CloseCommand = new MvxCommand(() =>
        {
            CloseAction?.Invoke();
        });
    }

    public string Title { get; }
    public string Subtitle { get; }
    public string CloseButtonText { get; }
    public string Header { get; }
    public Action CloseAction { get; set; }
    public MvxCommand CloseCommand { get; set; }
}
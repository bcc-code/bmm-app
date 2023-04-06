using BMM.Core.Models.Parameters;

namespace BMM.Core.Implementations.UI;

public interface IDialogService
{
    Task ShowDialog(IDialogParameter dialog);
}
using System.Diagnostics.CodeAnalysis;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Utils;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public static class ErrorPresenter
{
    private static TaskCompletionSource<bool> _taskCompletionSource;
    
    public static async Task<bool> ShowError(CPInterfaceController cpInterfaceController)
    {
        if (_taskCompletionSource != null)
            return await _taskCompletionSource.Task;
        
        _taskCompletionSource = new TaskCompletionSource<bool>();
        var languageBinder = Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>()!;

        var errorAlert = new CPAlertTemplate(
            [languageBinder[Translations.CarPlay_ErrorTitle]],
            [
                new CPAlertAction(languageBinder[Translations.CarPlay_TryAgainButton],
                    CPAlertActionStyle.Default,
                    async _ =>
                    {
                        await cpInterfaceController.DismissTemplateAsync(true);
                        _taskCompletionSource?.TrySetResult(true);
                        _taskCompletionSource = null;
                    })
            ]);

        await cpInterfaceController.PresentTemplateAsync(errorAlert, true);
        return await _taskCompletionSource.Task;
    }
}
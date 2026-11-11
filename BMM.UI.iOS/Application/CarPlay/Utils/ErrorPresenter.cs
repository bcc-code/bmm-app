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
        if (cpInterfaceController == null)
            return false;

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
                        try
                        {
                            await cpInterfaceController.DismissTemplateAsync(true);
                        }
                        catch
                        {
                            // ignore dismiss error if already dismissed
                        }
                        finally
                        {
                            var tcs = _taskCompletionSource;
                            _taskCompletionSource = null;
                            tcs?.TrySetResult(true);
                        }
                    })
            ]);

        try
        {
            await cpInterfaceController.PresentTemplateAsync(errorAlert, true);
        }
        catch
        {
            var tcs = _taskCompletionSource;
            _taskCompletionSource = null;
            tcs?.TrySetResult(false);
            return false;
        }

        return await _taskCompletionSource.Task;
    }
}
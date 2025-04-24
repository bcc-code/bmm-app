using System.Diagnostics.CodeAnalysis;
using BMM.Core.Extensions;
using BMM.UI.iOS.CarPlay.Utils;
using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Base;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public abstract class BaseLayoutCreator
{
    protected abstract CPInterfaceController CpInterfaceController { get; }
    
    protected async Task SafeLoad()
    {
        try
        {
            await Load();
        }
        catch
        {
            bool shouldReload = await ErrorPresenter.ShowError(CpInterfaceController);
            if (shouldReload)
                SafeLoad().FireAndForget();
        }
    }

    public abstract Task Load();
}
using System.Diagnostics.CodeAnalysis;
using BMM.Api.Framework;
using BMM.Core.Extensions;
using BMM.UI.iOS.CarPlay.Utils;
using CarPlay;
using MvvmCross;

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
        catch (Exception ex)
        {
            try
            {
                var logger = Mvx.IoCProvider?.Resolve<ILogger>();
                logger?.Error(GetType().Name, $"CarPlay layout loading failed: {ex.Message}", ex);
            }
            catch
            {
                // ignore logger resolution errors
            }

            Console.WriteLine($"[CarPlay] SafeLoad failed in {GetType().Name}: {ex}");

            bool shouldReload = await ErrorPresenter.ShowError(CpInterfaceController);
            if (shouldReload)
                SafeLoad().FireAndForget();
        }
    }

    public abstract Task Load();
}
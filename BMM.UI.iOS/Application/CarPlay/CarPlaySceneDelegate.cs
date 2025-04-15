using CarPlay;
using System.Diagnostics.CodeAnalysis;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using MvvmCross;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS.CarPlay
{
    [Register("CarPlaySceneDelegate")]
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
    public class CarPlaySceneDelegate : CPTemplateApplicationSceneDelegate
    {
        private IHomeLayoutCreator HomeLayoutCreator => Mvx.IoCProvider!.Resolve<IHomeLayoutCreator>();
        private IBrowseLayoutCreator BrowseLayoutCreator => Mvx.IoCProvider!.Resolve<IBrowseLayoutCreator>();
        private IFavouritesLayoutCreator FavouritesLayoutCreator => Mvx.IoCProvider!.Resolve<IFavouritesLayoutCreator>();
        private IPlaylistsLayoutCreator PlaylistsLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistsLayoutCreator>();
        
        public AppDelegate AppDelegateInstance => (AppDelegate)UIApplication.SharedApplication.Delegate;
        
        private CPInterfaceController _interfaceController;

        public override async void DidConnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = interfaceController;
            
            InitIoCIfNeeded();

            var tabBarTemplate = new CPTabBarTemplate(
            [
                await HomeLayoutCreator.Create(_interfaceController),
                await PlaylistsLayoutCreator.Create(_interfaceController),
                await BrowseLayoutCreator.Create(_interfaceController),
                await FavouritesLayoutCreator.Create(_interfaceController),
            ]);
            
            _interfaceController.SetRootTemplate(tabBarTemplate, true);
        }

        public override void DidDisconnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = null;
        }
        
        private static void InitIoCIfNeeded()
        {
            if (Mvx.IoCProvider != null && Mvx.IoCProvider.CanResolve<IMvxAppStart>())
                return;
            
            var setup = new IosSetup();
            setup.InitializePrimary();
            setup.InitializeSecondary();
        }
    }
}

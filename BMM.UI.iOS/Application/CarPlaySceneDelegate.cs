using CarPlay;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.Extensions;
using FFImageLoading;
using FFImageLoading.Cross;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using ObjCRuntime;
using TagLib.Ape;

namespace BMM.UI.iOS
{
    [Register("CarPlaySceneDelegate")]
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
    public class CarPlaySceneDelegate : CPTemplateApplicationSceneDelegate
    {
        public AppDelegate AppDelegateInstance => (AppDelegate)UIApplication.SharedApplication.Delegate;
        private CPInterfaceController _interfaceController;

        public override async void DidConnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = interfaceController;
            
            if (Mvx.IoCProvider == null || !Mvx.IoCProvider.CanResolve<IMvxAppStart>())
            {
                var setup = new IosSetup();
                setup.InitializePrimary();
                setup.InitializeSecondary();
            }
            
            var podcasts = (await Mvx.IoCProvider.Resolve<IPodcastClient>().GetAll(CachePolicy.UseCacheAndRefreshOutdated)).ToList();
            
            var images = new List<UIImage>();
            foreach (var item in podcasts)
                images.Add(await ImageService.Instance.LoadUrl(item.Cover).AsUIImageAsync());
            
            var podcastsImageRowItem = new CPListImageRowItem("Podcasts", images.ToArray(), podcasts.Select(x => x.Title).ToArray());
            podcastsImageRowItem.Handler = async (item, block) =>
            {
                Console.WriteLine("Item 2 tapped");
                await Task.Delay(2000);
                block();
            };
            
            podcastsImageRowItem.ListImageRowHandler = async (item, index, block) =>
            {
                var loadingItem = new CPListItem(text: "Loading...", detailText: null);
                var section = new CPListSection(loadingItem.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
                var template = new CPListTemplate(title: podcasts.First().Title, sections: [section]);
                    
                await _interfaceController.PushTemplateAsync(template, true);
                var trackPOFactory = Mvx.IoCProvider.Resolve<ITrackPOFactory>();
                var tracks = (await Mvx.IoCProvider.Resolve<IPodcastClient>().GetTracks(podcasts.First().Id, CachePolicy.UseCacheAndRefreshOutdated)).ToList();
                var tracksPO = tracks.Select(x=> trackPOFactory.Create(
                    new AudiobookPodcastInfoProvider(new DefaultTrackInfoProvider()),
                    null,
                    x));
                
                var tracksListItem = tracksPO.Select(x =>
                {
                    var item = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}");
                    item.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                    item.Handler = async (item, block) =>
                    {
                        Mvx.IoCProvider.Resolve<IMediaPlayer>().Play(tracks.OfType<IMediaTrack>().ToList(), x.Track);
                        var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                        var result = await interfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    };
                    return item;
                });
                var updatedSection = new CPListSection(tracksListItem.OfType<ICPListTemplateItem>().ToArray());
                
                template.UpdateSections(updatedSection.EncloseInArray());
                block();
            };
            
            var homeSection = new CPListSection(podcastsImageRowItem.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
            var homeTemplate = new CPListTemplate("Home", [homeSection]);
            homeTemplate.TabTitle = "Home";
            homeTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());

            var browseListTemplate = new CPListTemplate("Browse", []);
            browseListTemplate.TabTitle = "Browse";
            browseListTemplate.TabImage = UIImage.FromBundle("icon_browse".ToNameWithExtension());
            
            var favouritesListTemplate = new CPListTemplate("Favourites", []);
            favouritesListTemplate.TabTitle = "Favourites";
            favouritesListTemplate.TabImage = UIImage.FromBundle("icon_favorites".ToNameWithExtension());

            var tabBarTemplate = new CPTabBarTemplate(new CPTemplate[] { homeTemplate, browseListTemplate, favouritesListTemplate });
            _interfaceController.SetRootTemplate(tabBarTemplate, true);
        }

        public override void DidDisconnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = null;
        }
    }
}

using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.ViewModels;
namespace BMM.Core.ViewModels
{
    public class LibraryViewModel : BaseViewModel, IMvxViewModel<LibraryViewModel.Tab>
    {
        public PodcastsViewModel ViewModelPodcasts { get; set; }
        public LibraryArchiveViewModel ViewModelArchive { get; set; }
        /// <summary>
        /// Which tab will be shown when opening the view
        /// </summary>
        public Tab TabAtOpen { get; private set; }
        public LibraryViewModel()
        {
            ViewModelPodcasts = Mvx.IoCProvider.IoCConstruct<PodcastsViewModel>();
            ViewModelArchive = Mvx.IoCProvider.IoCConstruct<LibraryArchiveViewModel>();
        }
        public void Prepare(Tab openingTab)
        {
            TabAtOpen = openingTab;
        }
        public enum Tab
        {
            Podcasts = 0,
            Archive = 1
        }
    }
}
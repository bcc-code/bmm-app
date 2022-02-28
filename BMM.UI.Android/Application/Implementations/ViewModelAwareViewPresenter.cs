using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.UI.Droid.Application.Fragments;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Implementations
{
    public class ViewModelAwareViewPresenter : MvxAndroidViewPresenter, IViewModelAwareViewPresenter
    {
        public ViewModelAwareViewPresenter(IEnumerable<Assembly> androidViewAssemblies) : base(androidViewAssemblies)
        { }
        
        public override Task<bool> ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is CloseFragmentsOverPlayerHint)
                CloseFragmentsOverPlayer();
                
            return base.ChangePresentation(hint);
        }

        private void CloseFragmentsOverPlayer()
        {
            var playerFragment = CurrentFragmentManager
                !.Fragments
                .FirstOrDefault(x => x is PlayerFragment);
            
            if (playerFragment == null)
                return;

            int playerIndex = CurrentFragmentManager.Fragments.IndexOf(playerFragment);

            var fragmentsToClose = new List<MvxDialogFragment>();

            for (int i = playerIndex; i < CurrentFragmentManager.Fragments.Count; i++)
            {
                if (CurrentFragmentManager.Fragments[i] is MvxDialogFragment mvxFragmentView)
                    fragmentsToClose.Add(mvxFragmentView);
            }

            foreach (var viewModel in fragmentsToClose.Select(x => x.ViewModel))
                Mvx.IoCProvider.Resolve<IMvxNavigationService>().Close(viewModel);
        }

        public bool IsViewModelShown<T>()
        {
            if (CurrentFragmentManager == null)
                return false;

            var fragments = CurrentFragmentManager.Fragments;
            var vmType = typeof(T);

            return fragments.Any(f => GetAssociatedViewModelType(f.GetType()) == vmType);
        }

        public bool IsViewModelInStack<T>()
        {
            if (CurrentFragmentManager == null)
                return false;

            var vmType = typeof(T);

            if (CurrentFragmentManager.BackStackEntryCount < 1)
                return false;

            return Enumerable.Range(0, CurrentFragmentManager.BackStackEntryCount - 1)
                    .Reverse()
                    .Any(index => GetAssociatedViewModelType(CurrentFragmentManager.FindFragmentByTag(
                                          CurrentFragmentManager.GetBackStackEntryAt(index).Name
                        ).GetType()) == vmType);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BMM.Core.Implementations;
using MvvmCross.Platforms.Android.Presenters;

namespace BMM.UI.Droid.Application.Implementations
{
    public class ViewModelAwareViewPresenter : MvxAndroidViewPresenter, IViewModelAwareViewPresenter
    {
        public ViewModelAwareViewPresenter(IEnumerable<Assembly> androidViewAssemblies) : base(androidViewAssemblies)
        { }

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
            return Enumerable.Range(0, CurrentFragmentManager.BackStackEntryCount - 1)
                    .Reverse()
                    .Any(index => GetAssociatedViewModelType(CurrentFragmentManager.FindFragmentByTag(
                                          CurrentFragmentManager.GetBackStackEntryAt(index).Name
                        ).GetType()) == vmType);
        }
    }
}
using System;
using MvvmCross.ViewModels;

namespace BMM.Core.Helpers.PresentationHints
{
    public class NavigationRootChangedHint : MvxPresentationHint
    {
        public Type ViewModel { get; private set; }

        public NavigationRootChangedHint(Type viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
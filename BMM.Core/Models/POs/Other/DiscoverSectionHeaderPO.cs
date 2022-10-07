using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other
{
    public class DiscoverSectionHeaderPO : DocumentPO
    {
        private bool _isSeparatorVisible;
        private string _origin;

        public DiscoverSectionHeaderPO(
            IInternalDeepLinkAction internalDeepLinkAction,
            DiscoverSectionHeader discoverSectionHeader) : base(discoverSectionHeader)
        {
            DiscoverSectionHeader = discoverSectionHeader;
            DeepLinkButtonClickedCommand = new MvxCommand(() =>
            {
                internalDeepLinkAction.ExecuteGuarded(new InternalDeepLinkActionParameter(DiscoverSectionHeader.Link, Origin));
            });
            IsSeparatorVisible = true;
        }
        
        public DiscoverSectionHeader DiscoverSectionHeader { get; }
        public IMvxCommand DeepLinkButtonClickedCommand { get; }
        public bool HasLink => string.IsNullOrEmpty(DiscoverSectionHeader.Link);

        public bool IsSeparatorVisible
        {
            get => _isSeparatorVisible;
            set => SetProperty(ref _isSeparatorVisible, value);
        }

        public string Origin
        {
            get => _origin;
            set => SetProperty(ref _origin, value);
        }
    }
}
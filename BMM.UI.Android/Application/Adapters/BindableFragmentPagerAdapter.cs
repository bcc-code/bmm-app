using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AndroidX.Fragment.App;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Binding;
using MvvmCross.Logging;
using MvvmCross.Views;
using MvvmCross.WeakSubscription;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.Adapters
{
    public class BindableFragmentPagerAdapter <TViewModel>: FragmentPagerAdapter
        where TViewModel : class
    {
        private readonly FragmentManager _fragmentManager;
        private readonly Func<TViewModel, BmmViewPagerFragmentInfo> _createViewAction;
        private IEnumerable<TViewModel> _itemsSource;
        private MvxNotifyCollectionChangedEventSubscription _subscription;

        public BindableFragmentPagerAdapter(FragmentManager fragmentManager,
            Func<TViewModel, BmmViewPagerFragmentInfo> createViewAction)
            : base(fragmentManager, BehaviorResumeOnlyCurrentFragment)
        {
            _fragmentManager = fragmentManager;
            _createViewAction = createViewAction;
        }

        public override int GetItemPosition(Object value) => PositionNone;

        public IEnumerable<BmmViewPagerFragmentInfo> Fragments { get; private set; }

        public IEnumerable<TViewModel> ItemsSource
        {
            get => _itemsSource;
            set => SetItemsSource(value);
        }

        public override Fragment GetItem(int position)
        {
            var fragInfo = Fragments.ElementAt(position);
            
            if (fragInfo.CachedFragment == null)
            {
                fragInfo.CachedFragment = _fragmentManager.Instantiate(fragInfo);
                var fragmentAsView = (IMvxView)fragInfo.CachedFragment;
            
                fragmentAsView.ViewModel = fragInfo.ViewModel;
            }
            
            return fragInfo.CachedFragment;
        }

        protected virtual void SetItemsSource(IEnumerable<TViewModel> value)
        {
            if (ReferenceEquals(_itemsSource, value))
            {
                return;
            }

            _subscription?.Dispose();
            _subscription = null;

            _itemsSource = value;

            if (_itemsSource != null && !(_itemsSource is IList))
            {
                MvxBindingLog.Instance?.LogWarning("Binding to IEnumerable rather than IList - this can be inefficient, especially for large lists");
            }

            var newObservable = _itemsSource as INotifyCollectionChanged;
            if (newObservable != null)
            {
                _subscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);
            }

            BuildFragments();
            NotifyDataSetChanged();
        }

        private void BuildFragments()
        {
            var fragments = new List<BmmViewPagerFragmentInfo>();

            foreach (var item in ItemsSource)
            {
                BmmViewPagerFragmentInfo toAdd;

                if (Fragments != null
                    && (toAdd = Fragments.FirstOrDefault(x => Equals(x.ViewModel, item))) != null
                    && toAdd.CachedFragment != null)
                {
                    fragments.Add(toAdd);
                }
                else
                {
                    toAdd = _createViewAction.Invoke((item as TViewModel));
                    fragments.Add(toAdd);
                }
            }

            Fragments = fragments;
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BuildFragments();
            NotifyDataSetChanged();
        }

        public override int Count => ItemsSource?.Count() ?? 0;
    }
}
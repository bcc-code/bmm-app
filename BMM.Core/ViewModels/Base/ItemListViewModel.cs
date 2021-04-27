using System.Collections.Generic;
using BMM.Core.Models;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public class ItemListViewModel : BaseViewModel
    {
        public IMvxCommand ItemSelectedCommand =>
            new MvxCommand<IListItem>(item => (item as IListContentItem)?.OnSelected?.Execute());

        private IEnumerable<ListSection<IListContentItem>> _sections = new List<ListSection<IListContentItem>>();

        public IEnumerable<ListSection<IListContentItem>> Sections
        {
            get { return _sections; }
            set { SetProperty(ref _sections, value); }
        }
    }
}
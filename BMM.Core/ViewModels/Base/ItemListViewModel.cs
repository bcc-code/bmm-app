using System.Collections.Generic;
using System.Collections.ObjectModel;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public class ItemListViewModel : BaseViewModel
    {
        public IMvxCommand ItemSelectedCommand =>
            new MvxCommand<IBasePO>(item => (item as ISelectableListContentItemPO)?.OnSelected?.Execute());

        public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();
    }
}
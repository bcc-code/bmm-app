using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other;

public class SelectableContentItemPO : BasePO, ISelectableListContentItemPO
{
    public SelectableContentItemPO(string title, string text, IMvxCommand onSelected = null)
    {
        Title = title;
        Text = text;
        OnSelected = onSelected;
    }

    public string Title { get; }
    public string Text { get; }
    public IMvxCommand OnSelected { get; }
}
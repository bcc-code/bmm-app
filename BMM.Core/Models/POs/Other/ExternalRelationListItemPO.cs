using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other;

public class ExternalRelationListItemPO : SelectableContentItemPO, IExternalRelationListItemPO
{
    public ExternalRelationListItemPO(
        string title,
        string text, 
        IMvxCommand onSelected = null) : base(title, text, onSelected)
    {
    }
}
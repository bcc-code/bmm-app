using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other;

public class ExternalRelationListItemPO : SelectableContentItemPO, IExternalRelationListItemPO
{
    public ExternalRelationListItemPO(
        string title,
        string text,
        TrackRelationExternal trackRelationExternal, 
        IMvxCommand onSelected = null) : base(title, text, onSelected)
    {
        TrackRelationExternal = trackRelationExternal;
    }
    
    public TrackRelationExternal TrackRelationExternal { get; }
}
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.TrackInfo.Base;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.TrackInfo;

public class BuildExternalRelationsAction
    : BaseTrackInfoAction,
      IBuildExternalRelationsAction
{
    private readonly IUriOpener _uriOpener;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private Track _track;
    protected override Track Track => _track;

    public BuildExternalRelationsAction(
        IUriOpener uriOpener,
        IExceptionHandler exceptionHandler,
        IBMMLanguageBinder bmmLanguageBinder)
    {
        _uriOpener = uriOpener;
        _exceptionHandler = exceptionHandler;
        _bmmLanguageBinder = bmmLanguageBinder;
    }
    protected override async Task<IEnumerable<IBasePO>> Execute(Track parameter)
    {
        _track = parameter;
        
        var items = new List<IBasePO>();
        var externalRelations = BuildExternalRelations().ToList();

        if (externalRelations.Any())
        {
            items.Add(new SectionHeaderPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_ExternalReferences], false));
            items.AddRange(externalRelations);
        }

        return items;
    }
    
    protected IEnumerable<ISelectableListContentItemPO> BuildExternalRelations()
    {
        return GetRelationsOfType<TrackRelationExternal>()
            .Select(relation =>
            {
                return new ExternalRelationListItemPO(
                    relation.Name,
                    string.Empty,
                    relation,
                    new MvxCommand(() => { TryOpenExternalRelation(relation); }));
            });
    }
    
    private void TryOpenExternalRelation(TrackRelationExternal relation)
    {
        try
        {
            _uriOpener.OpenUri(new Uri(relation.Url));
        }
        catch (FormatException ex)
        {
            _exceptionHandler.HandleException(ex);
        }
    }
}
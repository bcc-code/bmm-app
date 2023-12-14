using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IProjectBoxPO : IDocumentPO
{
    ProjectBox ProjectBox { get; }
    IBmmInteraction ExpandOrCollapseInteraction { get; }
    IMvxCommand ExpandOrCollapseCommand { get; }
    IBmmObservableCollection<IAchievementPO> Achievements { get; }
}
using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories
{
    public interface IDocumentsPOFactory
    {
        IEnumerable<IDocumentPO> Create(
            IEnumerable<Document> documents,
            IMvxCommand<IDocumentPO> documentSelectedCommand,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITrackInfoProvider trackInfoProvider);
    }
}
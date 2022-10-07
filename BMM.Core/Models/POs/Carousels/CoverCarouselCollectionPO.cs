using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Carousels
{
    public class CoverCarouselCollectionPO : DocumentPO
    {
        public CoverCarouselCollectionPO(
            IDocumentsPOFactory documentsPOFactory,
            ITrackInfoProvider trackInfoProvider,
            IMvxCommand<IDocumentPO> documentSelectedCommand,
            IMvxAsyncCommand<Document> optionClickedCommand,
            CoverCarouselCollection coverCarouselCollection)
            : base(coverCarouselCollection)
        {
            DocumentSelectedCommand = documentSelectedCommand;
            var coverDocumentPOs = documentsPOFactory.Create(coverCarouselCollection.CoverDocuments, documentSelectedCommand, optionClickedCommand, trackInfoProvider);
            CoverDocuments.AddRange(coverDocumentPOs);
        }

        public IMvxCommand<IDocumentPO> DocumentSelectedCommand { get; }

        public IBmmObservableCollection<IDocumentPO> CoverDocuments { get; } = new BmmObservableCollection<IDocumentPO>();
    }
}
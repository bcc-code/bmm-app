using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Factories;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Documents
{
    public class PrepareContinueListeningCarouselItemsAction
        : GuardedActionWithParameterAndResult<IList<Document>, IList<IDocumentPO>>,
          IPrepareContinueListeningCarouselItemsAction
    {
        private readonly IDocumentsPOFactory _documentsPOFactory;

        public PrepareContinueListeningCarouselItemsAction(IDocumentsPOFactory documentsPOFactory)
        {
            _documentsPOFactory = documentsPOFactory;
        }

        private ExploreNewestViewModel DataContext => this.GetDataContext();
        
        protected override async Task<IList<IDocumentPO>> Execute(IList<Document> docs)
        {
            await Task.CompletedTask;
            var adjustedList = docs.ToList();

            while (true)
            {
                var firstTileElement = adjustedList
                    .FirstOrDefault(CheckIsTile);
                
                if (firstTileElement == null)
                    break;

                int startIndex = adjustedList
                    .IndexOf(firstTileElement);

                var tiles = new List<Document>();
                
                for (int i = startIndex; i < adjustedList.Count; i++)
                {
                    var item = adjustedList[i];
                    
                    if (CheckIsTile(adjustedList[i]))
                        tiles.Add(item);
                    else
                        break;
                }
                
                adjustedList.RemoveRange(startIndex, tiles.Count);
                adjustedList.Insert(startIndex, new TilesCollection(new ObservableCollection<Document>(tiles)));
            }
            
            return _documentsPOFactory.Create(
                adjustedList,
                DataContext.DocumentSelectedCommand,
                DataContext.OptionCommand,
                DataContext.TrackInfoProvider).ToList();
        }

        private static bool CheckIsTile(Document document)
        {
            return document
                .DocumentType
                .IsOneOf(DocumentType.Tile, DocumentType.TileMessage, DocumentType.TileVideo);
        }
    }
}
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
                var firstContinueListeningElement = adjustedList
                    .FirstOrDefault(e => e is ContinueListeningTile);
                
                if (firstContinueListeningElement == null)
                    break;

                int startIndex = adjustedList
                    .IndexOf(firstContinueListeningElement);

                var continueListeningTiles = new List<Document>();
                
                for (int i = startIndex; i < adjustedList.Count; i++)
                {
                    if (adjustedList[i] is ContinueListeningTile itemToAdd)
                        continueListeningTiles.Add(itemToAdd);
                    else
                        break;
                }
                
                adjustedList.RemoveRange(startIndex, continueListeningTiles.Count);
                adjustedList.Insert(startIndex, new ContinueListeningCollection(new ObservableCollection<Document>(continueListeningTiles)));
            }
            
            return _documentsPOFactory.Create(
                adjustedList,
                DataContext.DocumentSelectedCommand,
                DataContext.OptionCommand,
                DataContext.TrackInfoProvider).ToList();
        }
    }
}
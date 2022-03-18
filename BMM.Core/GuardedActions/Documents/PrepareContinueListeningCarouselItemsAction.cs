using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.GuardedActions.Documents
{
    public class PrepareContinueListeningCarouselItemsAction
        : GuardedActionWithParameterAndResult<IList<Document>, IList<Document>>,
          IPrepareContinueListeningCarouselItemsAction
    {
        protected override async Task<IList<Document>> Execute(IList<Document> docs)
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
            
            return adjustedList;
        }
    }
}
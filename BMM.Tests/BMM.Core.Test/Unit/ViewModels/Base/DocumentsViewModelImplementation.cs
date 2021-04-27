using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    public class DocumentsViewModelImplementation : DocumentsViewModel
    {
        public Func<IEnumerable<Document>> LoadItemsAction;

        public override Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return Task.Run(() => LoadItemsAction());
        }
    }
}

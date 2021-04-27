using BMM.Core.ViewModels.Base;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using System.Collections.Generic;
using BMM.Api.Abstraction;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class AlbumsViewModel : DocumentsViewModel, IMvxViewModel<int>
    {
        public int Year { get; private set; }

        public AlbumsViewModel() : base()
        { }

        public void Prepare(int yearParameter)
        {
            Year = yearParameter;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return await Client.Albums.GetPublishedByYear(Year);
        }
    }
}
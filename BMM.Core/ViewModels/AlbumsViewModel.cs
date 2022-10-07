using BMM.Core.ViewModels.Base;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
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

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return (await Client.Albums.GetPublishedByYear(Year))?.Select(a => new AlbumPO(a));
        }
    }
}